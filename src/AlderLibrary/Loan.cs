namespace AlderLibrary;

public sealed record Loan(int Id, string BookTitle, string Borrower, DateTimeOffset? ReturnedAt);
