using Google.Cloud.Firestore;

namespace CELERATE.API.Infrastructure.Firebase.Optimizations
{
    public static class FirestoreOptimizations
    {
        // Firestore indexleri oluşturmak için Cloud Functions kullanılacak
        public static void EnsureOptimalIndexing()
        {
            // Compound indexes for frequently searched fields
            // Firebase Console veya deployment script üzerinden yapılandırılır
            /* 
             * users collection: [CardId, IsActive]
             * users collection: [UserType, BranchId]
             * cards collection: [NfcId, IsActive]
             * transactions collection: [UserId, CreatedAt]
             * transactions collection: [BranchId, Type, CreatedAt]
             * transactions collection: [OperatorId, Type, CreatedAt]
             */
        }

        // Batch operations için helper
        public static async Task ExecuteBatchAsync<T>(
            FirestoreDb db,
            IEnumerable<T> entities,
            Func<WriteBatch, T, Task> operation)
        {
            var batch = db.StartBatch();
            int operationCount = 0;

            foreach (var entity in entities)
            {
                await operation(batch, entity);
                operationCount++;

                // Firebase batch işlemleri maksimum 500 işlemle sınırlıdır
                if (operationCount >= 499)
                {
                    await batch.CommitAsync();
                    batch = db.StartBatch();
                    operationCount = 0;
                }
            }

            if (operationCount > 0)
            {
                await batch.CommitAsync();
            }
        }
    }
}
