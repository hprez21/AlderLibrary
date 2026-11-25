using Xunit;

namespace AlderLibrary.Tests;

public sealed class LoanServiceTests : IDisposable
{
    private static readonly DateTimeOffset ReturnTime = new(2026, 2, 3, 12, 0, 0, TimeSpan.Zero);
    private readonly DirectoryInfo tempDirectory = Directory.CreateTempSubdirectory("alder-library-tests-");

    private static string SeedPath => Path.Combine(AppContext.BaseDirectory, "loans.seed.json");

    private string LocalPath => Path.Combine(tempDirectory.FullName, "loans.local.json");

    [Fact]
    public void ReturnLoan_PersistsTheReturnAcrossRepositoryInstances()
    {
        Assert.Equal(ReturnLoanResult.Returned, CreateService().ReturnLoan(1001));

        var storedLoan = Assert.Single(CreateRepository().Load(), loan => loan.Id == 1001);
        Assert.Equal(ReturnTime, storedLoan.ReturnedAt);
        Assert.Equal(ReturnLoanResult.AlreadyReturned, CreateService().ReturnLoan(1001));
    }

    [Fact]
    public void ReturnLoan_ForAnAlreadyReturnedLoan_DoesNotWriteLocalData()
    {
        Assert.Equal(ReturnLoanResult.AlreadyReturned, CreateService().ReturnLoan(1002));

        Assert.False(File.Exists(LocalPath));
    }

    [Fact]
    public void ReturnLoan_ForAnUnknownId_DoesNotWriteLocalData()
    {
        Assert.Equal(ReturnLoanResult.NotFound, CreateService().ReturnLoan(9999));

        Assert.False(File.Exists(LocalPath));
    }

    public void Dispose()
    {
        File.Delete(LocalPath);
        File.Delete(LocalPath + ".lock");
        File.Delete(LocalPath + ".tmp");
        tempDirectory.Delete();
    }

    private JsonLoanRepository CreateRepository() => new(SeedPath, LocalPath);

    private LoanService CreateService() => new(CreateRepository(), new FixedTimeProvider());

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => ReturnTime;
    }
}
