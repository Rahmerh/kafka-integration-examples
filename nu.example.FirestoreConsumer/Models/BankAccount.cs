namespace nu.example.FirestoreConsumer.Models;

using Google.Cloud.Firestore;

[FirestoreData]
public class BankAccount
{
    [FirestoreProperty("userId")]
    public string? UserId { get; set; }

    [FirestoreProperty("accountNumber")]
    public string? AccountNumber { get; set; }
}
