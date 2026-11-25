using System.Text.Json;

namespace AlderLibrary;

public sealed class JsonLoanRepository(string seedPath, string localPath)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    internal FileStream Lock()
    {
        return new FileStream(localPath + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite,
            FileShare.None, bufferSize: 1, FileOptions.DeleteOnClose);
    }

    public List<Loan> Load()
    {
        var sourcePath = File.Exists(localPath) ? localPath : seedPath;
        var json = File.ReadAllText(sourcePath);
        return JsonSerializer.Deserialize<List<Loan>>(json, JsonOptions)
            ?? throw new JsonException("The loan data must be a JSON array.");
    }

    internal void Save(List<Loan> loans)
    {
        var tempPath = localPath + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(loans, JsonOptions));
        File.Move(tempPath, localPath, overwrite: true);
    }
}
