// File: CELERATE.API.Infrastructure/Firebase/Converters/TransactionConverter.cs
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
// Using alias to avoid name collision with Firestore's Transaction class
using DomainTransaction = CELERATE.API.CORE.Entities.Transaction;
using TransactionType = CELERATE.API.CORE.Entities.TransactionType;

namespace CELERATE.API.Infrastructure.Firebase.Converters
{
    public static class TransactionConverter
    {
        public static Dictionary<string, object> ToFirestore(DomainTransaction transaction)
        {
            return new Dictionary<string, object>
            {
                ["Id"] = transaction.Id,
                ["CardId"] = transaction.CardId,
                ["UserId"] = transaction.UserId,
                ["OperatorId"] = transaction.OperatorId,
                ["BranchId"] = transaction.BranchId,
                ["Type"] = transaction.Type.ToString(),
                // Store decimal values as strings
                ["Amount"] = DecimalConverter.ToString(transaction.Amount),
                ["BalanceAfter"] = DecimalConverter.ToString(transaction.BalanceAfter),
                ["CreatedAt"] = transaction.CreatedAt
            };
        }

        public static DomainTransaction FromFirestore(DocumentSnapshot snapshot)
        {
            var data = snapshot.ToDictionary();
            var transaction = new DomainTransaction();

            transaction.Id = snapshot.Id;
            transaction.CardId = GetStringValue(data, "CardId");
            transaction.UserId = GetStringValue(data, "UserId");
            transaction.OperatorId = GetStringValue(data, "OperatorId");
            transaction.BranchId = GetStringValue(data, "BranchId");

            // Handle enum conversion
            if (data.TryGetValue("Type", out var typeValue) && typeValue is string typeStr)
            {
                if (Enum.TryParse<TransactionType>(typeStr, out var transactionType))
                {
                    transaction.Type = transactionType;
                }
            }

            // Handle decimal conversions with our custom converter
            if (data.TryGetValue("Amount", out var amountValue))
            {
                transaction.Amount = DecimalConverter.FromObject(amountValue);
            }

            if (data.TryGetValue("BalanceAfter", out var balanceAfterValue))
            {
                transaction.BalanceAfter = DecimalConverter.FromObject(balanceAfterValue);
            }

            transaction.CreatedAt = GetDateTimeValue(data, "CreatedAt", DateTime.UtcNow);

            return transaction;
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