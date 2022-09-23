using Google.Cloud.Firestore;

namespace nu.example.FirestoreProducer.Models;

[FirestoreData]
public class User
{
    [FirestoreProperty("id")]
    public string? Id { get; set; }

    [FirestoreProperty("firstName")]
    public string? FirstName { get; set; }

    [FirestoreProperty("lastName")]
    public string? LastName { get; set; }
}
