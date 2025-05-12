using CELERATE.API.Infrastructure.Firebase.Repositories;
using CELERATE.API.CORE.Interfaces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using CELERATE.API.Infrastructure.Firebase.Logging;
using CELERATE.API.Infrastructure.Firebase.Services;

namespace CELERATE.API.Infrastructure.Firebase
{
    public static class FirebaseConfiguration
    {
        public static IServiceCollection AddFirebaseServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Firebase credentials
            var projectId = configuration["Firebase:ProjectId"];

            // Önce credential'ı tanımlayın
            var credential = GetFirebaseCredential(configuration);

            // Firestore ayarları
            var firestoreSettings = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = credential
            }.Build();

            services.AddSingleton(firestoreSettings);

            // Firebase Admin SDK
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential
            });

            // Add repositories
            services.AddSingleton<IUserRepository, FirebaseUserRepository>();
            services.AddSingleton<ICardRepository, FirebaseCardRepository>();
            services.AddSingleton<IBranchRepository, FirebaseBranchRepository>();
            services.AddSingleton<ITransactionRepository, FirebaseTransactionRepository>();
            services.AddSingleton<ILogRepository, FirebaseLogRepository>();

            // Add Firebase services
            services.AddSingleton<FirebaseTokenService>();

            return services;
        }

        private static GoogleCredential GetFirebaseCredential(IConfiguration configuration)
        {
            var privateKeyPath = configuration["Firebase:PrivateKey"];

            if (string.IsNullOrEmpty(privateKeyPath))
            {
                throw new ApplicationException("Firebase private key path not found in configuration");
            }

            return GoogleCredential.FromFile(privateKeyPath);
        }
    }
}