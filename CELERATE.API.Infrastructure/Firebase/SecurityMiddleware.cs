using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog.Context;
using System.Security.Claims;

namespace CELERATE.API.Infrastructure.Firebase
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public SecurityMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Rate limiting kontrol
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            if (await IsRateLimitExceededAsync(clientIp, context.Request.Path))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            // Input validation for query parameters
            if (!ValidateQueryParameters(context.Request.Query))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid query parameters.");
                return;
            }

            // Sensitive data masking from logs
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            using (LogContext.PushProperty("ClientIP", MaskIPAddress(clientIp)))
            {
                await _next(context);
            }
        }

        private async Task<bool> IsRateLimitExceededAsync(string clientIp, string path)
        {
            // IP based rate limiting logic
            // Bu kısım, gerçek implementasyonda Redis veya başka bir cache ile yapılmalı
            return false;
        }

        private bool ValidateQueryParameters(IQueryCollection query)
        {
            // Temel validasyon kontrolleri
            foreach (var param in query)
            {
                if (param.Key.Contains("<") || param.Key.Contains(">") ||
                    param.Value.Any(v => v != null && (v.Contains("<") || v.Contains(">"))))
                {
                    return false;
                }
            }

            return true;
        }

        private string MaskIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return "unknown";

            var parts = ip.Split('.');
            if (parts.Length == 4)
            {
                return $"{parts[0]}.{parts[1]}.*.*";
            }

            return "masked";
        }
    }

    // FirebaseSecurityRules.cs
    public static class FirebaseSecurityRules
    {
        public static string GenerateFirestoreRules()
        {
            return @"
        rules_version = '2';
        service cloud.firestore {
          match /databases/{database}/documents {
            // Common functions for authorization
            function isAuthenticated() {
              return request.auth != null;
            }
            
            function isAdmin() {
              return isAuthenticated() && 
                     get(/databases/$(database)/documents/users/$(request.auth.uid)).data.userRole == 'Administrator';
            }
            
            function isBranchManager() {
              return isAuthenticated() && 
                     (get(/databases/$(database)/documents/users/$(request.auth.uid)).data.userRole == 'BranchManager' ||
                      isAdmin());
            }
            
            function isStaff() {
              return isAuthenticated() && 
                     (get(/databases/$(database)/documents/users/$(request.auth.uid)).data.userRole == 'Staff' ||
                      isBranchManager());
            }
            
            function isFromSameBranch(branchId) {
              return isAuthenticated() && 
                     get(/databases/$(database)/documents/users/$(request.auth.uid)).data.branchId == branchId;
            }
            
            // User collection rules
            match /users/{userId} {
              allow read: if isAuthenticated() && (request.auth.uid == userId || isStaff());
              allow create: if isAdmin() || isBranchManager();
              allow update: if isAdmin() || 
                             (isBranchManager() && isFromSameBranch(resource.data.branchId));
              allow delete: if false; // Nothing is deleted
            }
            
            // Card collection rules
            match /cards/{cardId} {
              allow read: if isAuthenticated();
              allow create: if isStaff();
              allow update: if isStaff() && 
                            (isFromSameBranch(get(/databases/$(database)/documents/users/$(resource.data.userId)).data.branchId) ||
                             isAdmin());
              allow delete: if false; // Nothing is deleted
            }
            
            // Branch collection rules
            match /branches/{branchId} {
              allow read: if isAuthenticated();
              allow create: if isAdmin();
              allow update: if isAdmin();
              allow delete: if false; // Nothing is deleted
            }
            
            // Transaction collection rules
            match /transactions/{transactionId} {
              allow read: if isAuthenticated() && 
                          (isStaff() || 
                           resource.data.userId == request.auth.uid);
              allow create: if isStaff() && 
                            (isFromSameBranch(resource.data.branchId) || isAdmin());
              allow update: if false; // Transactions are immutable
              allow delete: if false; // Nothing is deleted
            }
            
            // Log collection rules
            match /logs/{logId} {
              allow read: if isAuthenticated() && 
                          (isStaff() || 
                           resource.data.userId == request.auth.uid);
              allow create: if isAuthenticated();
              allow update: if false; // Logs are immutable
              allow delete: if false; // Nothing is deleted
            }
          }
        }
        ";
        }
    }
}
