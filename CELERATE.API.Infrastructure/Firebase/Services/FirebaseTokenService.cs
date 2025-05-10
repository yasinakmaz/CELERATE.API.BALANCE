using FirebaseAdmin.Auth;

namespace CELERATE.API.Infrastructure.Firebase.Services
{
    public class FirebaseTokenService
    {
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseTokenService()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }

        public async Task<string> CreateCustomTokenAsync(string uid, Dictionary<string, object> claims)
        {
            return await _firebaseAuth.CreateCustomTokenAsync(uid, claims);
        }

        public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
        {
            return await _firebaseAuth.VerifyIdTokenAsync(idToken);
        }

        public async Task SetCustomUserClaimsAsync(string uid, Dictionary<string, object> claims)
        {
            await _firebaseAuth.SetCustomUserClaimsAsync(uid, claims);
        }
    }
}
