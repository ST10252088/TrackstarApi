using Google.Cloud.Firestore;

namespace Trackstar.Api.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _db;

        public FirestoreService(FirestoreDb db)
        {
            _db = db;
        }

        // Create project
        public async Task<string> CreateProjectAsync(string name, string? description)
        {
            var docRef = _db.Collection("projects").Document();
            var data = new Dictionary<string, object>
            {
                ["name"] = name,
                ["description"] = description ?? "",
                ["createdAt"] = Timestamp.GetCurrentTimestamp()
            };
            await docRef.SetAsync(data);
            return docRef.Id;
        }

        // Get all projects
        public async Task<List<Dictionary<string, object>>> GetProjectsAsync()
        {
            var snapshot = await _db.Collection("projects").GetSnapshotAsync();
            var projects = new List<Dictionary<string, object>>();
            foreach (var doc in snapshot.Documents)
            {
                var dict = doc.ToDictionary();
                dict["id"] = doc.Id;
                projects.Add(dict);
            }
            return projects;
        }

        // Get a project by id
        public async Task<Dictionary<string, object>?> GetProjectByIdAsync(string id)
        {
            var doc = await _db.Collection("projects").Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ToDictionary() : null;
        }

        // Create task in project's subcollection
        public async Task<string> CreateTaskAsync(string projectId, string title, string? description)
        {
            var tasksCol = _db.Collection("projects").Document(projectId).Collection("tasks");
            var taskDoc = tasksCol.Document();
            var data = new Dictionary<string, object>
            {
                ["title"] = title,
                ["description"] = description ?? "",
                ["createdAt"] = Timestamp.GetCurrentTimestamp()
            };
            await taskDoc.SetAsync(data);
            return taskDoc.Id;
        }

        // Get tasks for a project
        public async Task<List<Dictionary<string, object>>> GetTasksAsync(string projectId)
        {
            var tasksCol = _db.Collection("projects").Document(projectId).Collection("tasks");
            var snapshot = await tasksCol.GetSnapshotAsync();
            var tasks = new List<Dictionary<string, object>>();
            foreach (var doc in snapshot.Documents)
            {
                var dict = doc.ToDictionary();
                dict["id"] = doc.Id;
                tasks.Add(dict);
            }
            return tasks;
        }

        // Update task
        public async Task<bool> UpdateTaskAsync(string projectId, string taskId, Dictionary<string, object> updates)
        {
            var taskRef = _db.Collection("projects").Document(projectId).Collection("tasks").Document(taskId);
            var doc = await taskRef.GetSnapshotAsync();
            if (!doc.Exists) return false;
            await taskRef.UpdateAsync(updates);
            return true;
        }

        // Delete task
        public async Task<bool> DeleteTaskAsync(string projectId, string taskId)
        {
            var taskRef = _db.Collection("projects").Document(projectId).Collection("tasks").Document(taskId);
            var doc = await taskRef.GetSnapshotAsync();
            if (!doc.Exists) return false;
            await taskRef.DeleteAsync();
            return true;
        }
    }
}
