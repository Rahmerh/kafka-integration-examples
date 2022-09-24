namespace nu.example.Shared.Models;

public class BankAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? AccountNumber { get; set; }
}
