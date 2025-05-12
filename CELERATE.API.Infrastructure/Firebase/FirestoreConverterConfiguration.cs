using Google.Cloud.Firestore;
using CELERATE.API.Infrastructure.Firebase.Converters;

namespace CELERATE.API.Infrastructure.Firebase
{
    public static class FirestoreConverterConfiguration
    {
        public static void RegisterConverters()
        {
            // Register the decimal converter globally
            ConverterRegistry.Instance.Register(new FirestoreDecimalConverter());
        }
    }
}