using Google.Cloud.Firestore;
using CELERATE.API.Infrastructure.Firebase.Converters;
using CELERATE.API.CORE.Entities;

namespace CELERATE.API.Infrastructure.Firebase
{
    public static class FirestoreConverterConfiguration
    {
        public static void RegisterConverters(ConverterRegistry registry)
        {
            registry.Add(new FirestoreDecimalConverter());
            registry.Add(new GenericEnumConverter<Gender>());
            registry.Add(new GenericEnumConverter<CompanyType>());
            registry.Add(new GenericEnumConverter<BranchOperationType>());
            registry.Add(new GenericEnumConverter<UserType>());
            registry.Add(new GenericEnumConverter<UserRole>());
            registry.Add(new GenericEnumConverter<TransactionType>());
            registry.Add(new GenericEnumConverter<Permission>());
        }
    }
}