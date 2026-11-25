using System.Text.Json;
using AlderLibrary;

if (args.Length != 2 || args[0] != "return" || !int.TryParse(args[1], out var loanId) || loanId <= 0)
{
    Console.Error.WriteLine("Usage: return <loan-id>");
    return 2;
}

var repository = new JsonLoanRepository(
    Path.Combine("data", "loans.seed.json"),
    Path.Combine("data", "loans.local.json"));
var service = new LoanService(repository, TimeProvider.System);

try
{
    var result = service.ReturnLoan(loanId);

    if (result == ReturnLoanResult.NotFound)
    {
        Console.Error.WriteLine($"Loan {loanId} was not found.");
        return 1;
    }

    Console.WriteLine(result == ReturnLoanResult.Returned
        ? $"Loan {loanId} returned."
        : $"Loan {loanId} was already returned.");
    return 0;
}
catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException)
{
    Console.Error.WriteLine($"Unable to access the loan data: {error.Message}");
    return 1;
}
