using Google.Cloud.Firestore;

namespace CELERATE.API.Infrastructure.Firebase.Converters
{
    public class GenericEnumConverter<TEnum> : IFirestoreConverter<TEnum> where TEnum : struct, Enum
    {
        public object ToFirestore(TEnum value)
        {
            return value.ToString();
        }

        public TEnum FromFirestore(object value)
        {
            if (value == null)
                return default;

            string stringValue = value.ToString().Trim();

            if (Enum.TryParse<TEnum>(stringValue, true, out var result))
            {
                return result;
            }

            return default;
        }
    }
}
