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

            app.Run();
        }
    }
}
