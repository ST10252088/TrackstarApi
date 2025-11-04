using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using FirebaseAdmin;
using Microsoft.OpenApi.Models;
using Trackstar.Api.Middleware;
using Trackstar.Api.Services;

namespace Trackstar.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Firebase + Firestore setup ---

            // Get Firebase project ID and service account JSON from environment variables
            var projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID")
                ?? throw new Exception("FIREBASE_PROJECT_ID environment variable not set.");

            var serviceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON")
                ?? throw new Exception("FIREBASE_SERVICE_ACCOUNT_JSON environment variable not set.");

            // Write service account JSON to a temp file
            var saFile = Path.Combine(Path.GetTempPath(), $"firebase_sa_{Guid.NewGuid()}.json");
            File.WriteAllText(saFile, serviceAccountJson);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", saFile);

            // Initialize Firebase
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(saFile)
            });

            // Create FirestoreDb instance
            var db = FirestoreDb.Create(projectId);
            builder.Services.AddSingleton(db);

            // Add services and controllers
            builder.Services.AddSingleton<FirestoreService>();
            builder.Services.AddControllers();

            // Swagger (OpenAPI)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Trackstar API (POC)",
                    Version = "v1",
                    Description = "Proof of concept REST API using Firestore"
                });
            });

            var app = builder.Build();

            // Bind to Render port (or default localhost)
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            app.Urls.Clear();
            app.Urls.Add($"http://0.0.0.0:{port}");

            // Middleware
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            // default route to check if API is running
            app.MapGet("/", () => Results.Ok(new { status = "API is running" }));

            app.Run();
        }
    }
}

//using Google.Apis.Auth.OAuth2;
//using Google.Cloud.Firestore;
//using FirebaseAdmin;
//using Microsoft.OpenApi.Models;
//using Trackstar.Api.Middleware;
//using Trackstar.Api.Services;

//namespace Trackstar.Api
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // -------------------------------------------------------
//            // Load environment variables (for local .env support)
//            // -------------------------------------------------------
//            if (builder.Environment.IsDevelopment())
//            {
//                var root = Directory.GetCurrentDirectory();
//                var envPath = Path.Combine(root, ".env");

//                if (File.Exists(envPath))
//                {
//                    foreach (var line in File.ReadAllLines(envPath))
//                    {
//                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
//                        var parts = line.Split('=', 2);
//                        if (parts.Length == 2)
//                            Environment.SetEnvironmentVariable(parts[0], parts[1]);
//                    }
//                }
//            }

//            // -------------------------------------------------------
//            // Firebase + Firestore setup
//            // -------------------------------------------------------

//            var projectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID")
//                ?? throw new Exception("FIREBASE_PROJECT_ID environment variable not set.");

//            // Check if we are in hosted mode or local mode
//            var serviceAccountJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");
//            var serviceAccountPath = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON_PATH");

//            string credentialsPath;

//            if (!string.IsNullOrEmpty(serviceAccountJson))
//            {
//                // Hosted environment (Render, etc.)
//                credentialsPath = Path.Combine(Path.GetTempPath(), $"firebase_sa_{Guid.NewGuid()}.json");
//                File.WriteAllText(credentialsPath, serviceAccountJson);
//            }
//            else if (!string.IsNullOrEmpty(serviceAccountPath))
//            {
//                // Local environment
//                credentialsPath = Path.GetFullPath(serviceAccountPath);
//                if (!File.Exists(credentialsPath))
//                    throw new FileNotFoundException($"Service account file not found at {credentialsPath}");
//            }
//            else
//            {
//                throw new Exception("No Firebase credentials found. Set FIREBASE_SERVICE_ACCOUNT_JSON or FIREBASE_SERVICE_ACCOUNT_JSON_PATH.");
//            }

//            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

//            FirebaseApp.Create(new AppOptions
//            {
//                Credential = GoogleCredential.FromFile(credentialsPath),
//                ProjectId = projectId
//            });

//            var db = FirestoreDb.Create(projectId);
//            builder.Services.AddSingleton(db);

//            // -------------------------------------------------------
//            // Services + Controllers
//            // -------------------------------------------------------
//            builder.Services.AddSingleton<FirestoreService>();
//            builder.Services.AddControllers();

//            // Swagger
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo
//                {
//                    Title = "Trackstar API",
//                    Version = "v1",
//                    Description = "Firestore-backed API for Trackstar"
//                });
//            });

//            var app = builder.Build();

//            // -------------------------------------------------------
//            // App Configuration
//            // -------------------------------------------------------

//            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
//            app.Urls.Clear();
//            app.Urls.Add($"http://0.0.0.0:{port}");

//            // Swagger UI
//            app.UseSwagger();
//            app.UseSwaggerUI();

//            // Controllers
//            app.MapControllers();

//            // Root test endpoint
//            app.MapGet("/", () => Results.Ok(new { status = "API is running" }));

//            app.Run();
//        }
//    }
//}
