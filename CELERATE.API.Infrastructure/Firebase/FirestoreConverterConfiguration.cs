using Google.Cloud.Firestore;
using CELERATE.API.Infrastructure.Firebase.Converters;

namespace CELERATE.API.Infrastructure.Firebase
{
    public static class FirestoreConverterConfiguration
    {
        public static void RegisterConverters(ConverterRegistry registry)
        {
            registry.Add(new FirestoreDecimalConverter());
        }
    }
}