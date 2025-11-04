using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Trackstar.Api.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _db;

        public FirestoreService(FirestoreDb db)
        {
            _db = db;
        }

        // --- USERS ---

       
        // Get All Users
        public async Task<List<Dictionary<string, object>>> GetAllUsersAsync()
        {
            var snapshot = await _db.Collection("users").GetSnapshotAsync();
            var users = new List<Dictionary<string, object>>();
            foreach (var doc in snapshot.Documents)
            {
                var dict = doc.ToDictionary();
                dict["id"] = doc.Id;
                users.Add(dict);
            }
            return users;
        }

        // Get User by Id
        public async Task<Dictionary<string, object>?> GetUserByIdAsync(string id)
        {
            var doc = await _db.Collection("users").Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ToDictionary() : null;
        }

        // creating user
        public async Task<string> CreateUserAsync(Dictionary<string, object> userData)
        {
            var docRef = _db.Collection("users").Document(userData.ContainsKey("uid") ? userData["uid"].ToString() : null);
            if (docRef == null)
                docRef = _db.Collection("users").Document();

            await docRef.SetAsync(userData);
            return docRef.Id;
        }

        // Update a user
        public async Task<bool> UpdateUserAsync(string id, Dictionary<string, object> updates)
        {
            var userRef = _db.Collection("users").Document(id);
            var doc = await userRef.GetSnapshotAsync();
            if (!doc.Exists) return false;

            await userRef.UpdateAsync(updates);
            return true;
        }

        // Delete a user
        public async Task<bool> DeleteUserAsync(string id)
        {
            var userRef = _db.Collection("users").Document(id);
            var doc = await userRef.GetSnapshotAsync();

            if (!doc.Exists)
                return false;

            await userRef.DeleteAsync();
            return true;
        }

        // --- PROJECTS ---

        public async Task<string> CreateProjectAsync(string name, string? description)
        {
            var docRef = _db.Collection("projects").Document();
            var data = new Dictionary<string, object>
            {
                ["name"] = name,
                ["description"] = description ?? "",
                ["createdAt"] = Timestamp.GetCurrentTimestamp(),
                ["memberUids"] = new List<string>()
            };
            await docRef.SetAsync(data);
            return docRef.Id;
        }

        /// <summary>
        /// Returns a list of all projects. Each project's Dictionary will include an "id" key.
        /// </summary>
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

        /// <summary>
        /// Returns a single project document by id or null if not found.
        /// </summary>
        public async Task<Dictionary<string, object>?> GetProjectByIdAsync(string id)
        {
            var doc = await _db.Collection("projects").Document(id).GetSnapshotAsync();
            return doc.Exists ? doc.ToDictionary() : null;
        }

        public async Task<bool> UpdateProjectAsync(string id, Dictionary<string, object> updates)
        {
            var refDoc = _db.Collection("projects").Document(id);
            var doc = await refDoc.GetSnapshotAsync();
            if (!doc.Exists) return false;
            await refDoc.UpdateAsync(updates);
            return true;
        }

        public async Task<bool> DeleteProjectAsync(string id)
        {
            var refDoc = _db.Collection("projects").Document(id);
            var doc = await refDoc.GetSnapshotAsync();
            if (!doc.Exists) return false;
            await refDoc.DeleteAsync();
            return true;
        }

        public async Task<bool> AssignUserToProjectAsync(string projectId, string userEmail)
        {
            var projectRef = _db.Collection("projects").Document(projectId);
            var snapshot = await projectRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return false;

            // Query the user document by email
            var userQuery = await _db.Collection("users")
                .WhereEqualTo("email", userEmail)
                .GetSnapshotAsync();

            if (!userQuery.Documents.Any())
                return false; // user not found

            var userDoc = userQuery.Documents.First();
            var userUid = userDoc.Id;

            // Get current members (if any)
            var members = snapshot.ContainsField("memberUids")
                ? snapshot.GetValue<List<string>>("memberUids")
                : new List<string>();

            // Add the user if not already a member
            if (!members.Contains(userUid))
                members.Add(userUid);

            // Update the project document
            await projectRef.UpdateAsync("memberUids", members);
            return true;
        }


        // --- TASKS ---

        public async Task<string> CreateTaskAsync(string projectId, Dictionary<string, object> taskData)
        {
            var taskRef = _db.Collection("projects").Document(projectId).Collection("tasks").Document();
            taskData["createdAt"] = Timestamp.GetCurrentTimestamp();
            await taskRef.SetAsync(taskData);
            return taskRef.Id;
        }

        /// <summary>
        /// Returns all tasks for the given project. Each task Dictionary includes an "id" key.
        /// </summary>
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

        public async Task<Dictionary<string, object>?> GetTaskByIdAsync(string projectId, string taskId)
        {
            var doc = await _db.Collection("projects").Document(projectId).Collection("tasks").Document(taskId).GetSnapshotAsync();
            return doc.Exists ? doc.ToDictionary() : null;
        }

        public async Task<bool> UpdateTaskAsync(string projectId, string taskId, Dictionary<string, object> updates)
        {
            var taskRef = _db.Collection("projects").Document(projectId).Collection("tasks").Document(taskId);
            var doc = await taskRef.GetSnapshotAsync();
            if (!doc.Exists) return false;
            await taskRef.UpdateAsync(updates);
            return true;
        }

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
