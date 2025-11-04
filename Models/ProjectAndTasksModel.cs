using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace Trackstar.Api.Models
{
    [FirestoreData]
    public class ProjectAndTasksModel
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("createdAt")]
        public Timestamp? CreatedAt { get; set; }

        [FirestoreProperty("memberUids")]
        public List<string> MemberUids { get; set; } = new();

        // Not in Firestore, just in C#
        public List<TaskModel> Tasks { get; set; } = new();
        public List<UserInfoModel> Members { get; set; } = new();
    }

    [FirestoreData]
    public class TaskModel
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        // Not in Firestore, just in C#
        public string ProjectId { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("assignedTo")]
        public string AssignedTo { get; set; }

        [FirestoreProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("colorStatus")]
        public string ColorStatus { get; set; }

        [FirestoreProperty("createdAt")]
        public Timestamp? CreatedAt { get; set; }

        [FirestoreProperty("dueDate")]
        public Timestamp? DueDate { get; set; }
    }

    public class UserInfoModel
    {
        public string Uid { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{FirstName} {Surname}".Trim();
        public string Email { get; set; }
    }

}
