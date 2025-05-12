using Google.Cloud.Firestore;

namespace CELERATE.API.CORE.Entities
{
    [FirestoreData]
    public class LogEntry
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Action { get; set; }

        [FirestoreProperty]
        public string Details { get; set; }

        [FirestoreProperty]
        public DateTime StartTime { get; set; }

        [FirestoreProperty]
        public DateTime EndTime { get; set; }

        [FirestoreProperty]
        public string BranchId { get; set; }
    }
}