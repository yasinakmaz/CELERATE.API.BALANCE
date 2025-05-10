// CELERATE.API.Infrastructure/Firebase/FirebaseConfiguration.cs
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

            // Initialize Firestore
            FirestoreDb firestoreDb = FirestoreDb.Create(projectId);
            services.AddSingleton(firestoreDb);

            // Firebase Admin SDK
            var credential = GetFirebaseCredential(configuration);
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential
            });

            // Add repositories
            services.AddScoped<IUserRepository, FirebaseUserRepository>();
            services.AddScoped<ICardRepository, FirebaseCardRepository>();
            services.AddScoped<IBranchRepository, FirebaseBranchRepository>();
            services.AddScoped<ITransactionRepository, FirebaseTransactionRepository>();
            services.AddScoped<ILogRepository, FirebaseLogRepository>();

            // Add Firebase services
            services.AddScoped<FirebaseTokenService>();

            return services;
        }

        private static GoogleCredential GetFirebaseCredential(IConfiguration configuration)
        {
            // Private key'i environment variable veya configuration'dan al
            var privateKeyJson = configuration["Firebase:PrivateKey"];

            if (string.IsNullOrEmpty(privateKeyJson))
            {
                throw new ApplicationException("Firebase private key not found in configuration");
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(privateKeyJson)))
            {
                return GoogleCredential.FromStream(stream);
            }
        }
    }
}