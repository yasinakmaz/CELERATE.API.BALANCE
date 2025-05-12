using Google.Cloud.Firestore;
namespace CELERATE.API.Infrastructure.Firebase.Converters
{
    public class GenderEnumConverter : IFirestoreConverter<CELERATE.API.CORE.Entities.Gender>
    {
        public object ToFirestore(CELERATE.API.CORE.Entities.Gender value)
        {
            return value.ToString();
        }

        public CELERATE.API.CORE.Entities.Gender FromFirestore(object value)
        {
            if (value == null)
                return default;

            string stringValue = value.ToString().Trim();

            if (Enum.TryParse<CELERATE.API.CORE.Entities.Gender>(stringValue, true, out var result))
            {
                return result;
            }

            return CELERATE.API.CORE.Entities.Gender.Other;
        }
    }
}