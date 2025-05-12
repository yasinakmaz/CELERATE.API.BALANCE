// File: CELERATE.API.Infrastructure/Firebase/Converters/CardConverter.cs
using CELERATE.API.CORE.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CELERATE.API.Infrastructure.Firebase.Converters
{
    public static class CardConverter
    {
        public static Dictionary<string, object> ToFirestore(Card card)
        {
            // Create a dictionary to hold serialized values
            var documentData = new Dictionary<string, object>
            {
                ["Id"] = card.Id,
                ["NfcId"] = card.NfcId,
                ["UserId"] = card.UserId,
                // Store Balance as string to preserve decimal precision
                ["Balance"] = DecimalConverter.ToString(card.Balance),
                ["IsAuthorized"] = card.IsAuthorized,
                ["IsActive"] = card.IsActive,
                ["CreatedAt"] = card.CreatedAt,
                ["PermissionStrings"] = card.PermissionStrings ?? new List<string>()
            };

            if (card.LastModifiedAt.HasValue)
            {
                documentData["LastModifiedAt"] = card.LastModifiedAt.Value;
            }

            return documentData;
        }

        public static Card FromFirestore(DocumentSnapshot snapshot)
        {
            var data = snapshot.ToDictionary();
            var card = new Card();

            // Set basic properties
            card.Id = snapshot.Id;
            card.NfcId = GetStringValue(data, "NfcId");
            card.UserId = GetStringValue(data, "UserId");

            // Handle Balance with our custom converter
            if (data.TryGetValue("Balance", out var balanceValue))
            {
                card.Balance = DecimalConverter.FromObject(balanceValue);
            }

            card.IsAuthorized = GetBoolValue(data, "IsAuthorized");
            card.IsActive = GetBoolValue(data, "IsActive", true);
            card.CreatedAt = GetDateTimeValue(data, "CreatedAt", DateTime.UtcNow);

            if (data.TryGetValue("LastModifiedAt", out var lastModifiedAt) && lastModifiedAt != null)
            {
                card.LastModifiedAt = (DateTime)lastModifiedAt;
            }

            // Handle permissions
            if (data.TryGetValue("PermissionStrings", out var permissionsObj) && permissionsObj is IEnumerable<object> permissionsList)
            {
                card.PermissionStrings = permissionsList.Select(p => p.ToString()).ToList();
            }

            return card;
        }

        // Helper methods for different types to avoid ambiguous calls
        private static string GetStringValue(Dictionary<string, object> data, string key, string defaultValue = null)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                return value.ToString();
            }
            return defaultValue;
        }

        private static bool GetBoolValue(Dictionary<string, object> data, string key, bool defaultValue = false)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                return (bool)value;
            }
            return defaultValue;
        }

        private static DateTime GetDateTimeValue(Dictionary<string, object> data, string key, DateTime defaultValue)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                return (DateTime)value;
            }
            return defaultValue;
        }
    }
}