using Google.Cloud.Firestore;

namespace CELERATE.API.Infrastructure.Firebase.Converters
{
    public class FirestoreDecimalConverter : IFirestoreConverter<decimal>
    {
        public object ToFirestore(decimal value)
        {
            return value.ToString("G29");
        }

        public decimal FromFirestore(object value)
        {
            return value switch
            {
                string stringValue => decimal.Parse(stringValue),
                double doubleValue => (decimal)doubleValue,
                long longValue => (decimal)longValue,
                int intValue => (decimal)intValue,
                _ => throw new ArgumentException($"Cannot convert {value} to decimal")
            };
        }
    }
}
