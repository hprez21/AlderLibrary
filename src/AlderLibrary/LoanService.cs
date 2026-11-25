namespace AlderLibrary;

public enum ReturnLoanResult
{
    Returned,
    AlreadyReturned,
    NotFound
}

public sealed class LoanService(JsonLoanRepository repository, TimeProvider clock)
{
    public ReturnLoanResult ReturnLoan(int loanId)
    {
        using var dataLock = repository.Lock();

        var loans = repository.Load();
        var index = loans.FindIndex(loan => loan.Id == loanId);
        if (index < 0)
        {
            return ReturnLoanResult.NotFound;
        }

        var loan = loans[index];
        if (loan.ReturnedAt.HasValue)
        {
            return ReturnLoanResult.AlreadyReturned;
        }

        loans[index] = loan with { ReturnedAt = clock.GetUtcNow() };
        repository.Save(loans);
        return ReturnLoanResult.Returned;
    }
}
