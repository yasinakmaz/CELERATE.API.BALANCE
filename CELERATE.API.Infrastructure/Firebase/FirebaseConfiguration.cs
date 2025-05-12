using Google.Cloud.Firestore;
using Google.Cloud.Firestore.Converters;
using CELERATE.API.Infrastructure.Firebase.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Google.Apis.Auth.OAuth2;
using System.Collections.Generic;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.Infrastructure.Firebase.Logging;
using CELERATE.API.Infrastructure.Firebase.Repositories;
using CELERATE.API.Infrastructure.Firebase.Services;
using FirebaseAdmin;

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
            var credential = GetFirebaseCredential(configuration);

            // Create Firestore converter settings
            var converterRegistry = new ConverterRegistry();

            FirestoreConverterConfiguration.RegisterConverters(converterRegistry);

            // Firestore settings with custom converters
            var firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = credential,
                ConverterRegistry = converterRegistry
            }.Build();

            services.AddSingleton(firestoreDb);

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