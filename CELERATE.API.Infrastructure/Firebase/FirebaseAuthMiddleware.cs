using CELERATE.API.Infrastructure.Firebase.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CELERATE.API.Infrastructure.Firebase
{
    public class FirebaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FirebaseTokenService _tokenService;

        public FirebaseAuthMiddleware(RequestDelegate next, FirebaseTokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    var decodedToken = await _tokenService.VerifyIdTokenAsync(token);

                    if (decodedToken != null)
                    {
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                    };

                        foreach (var claim in decodedToken.Claims)
                        {
                            if (claim.Key == "role")
                            {
                                claims.Add(new Claim(ClaimTypes.Role, claim.Value.ToString()));
                            }
                            else if (claim.Key == "permissions" && claim.Value is List<object> permissions)
                            {
                                foreach (var permission in permissions)
                                {
                                    claims.Add(new Claim("Permission", permission.ToString()));
                                }
                            }
                            else
                            {
                                claims.Add(new Claim(claim.Key, claim.Value.ToString()));
                            }
                        }

                        var identity = new ClaimsIdentity(claims, "Firebase");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
                catch (FirebaseAuthException)
                {
                }
            }

            await _next(context);
        }
    }
}
