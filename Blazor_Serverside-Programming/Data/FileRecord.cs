namespace Blazor_Serverside_Programming.Data;

public class FileRecord
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string VerificationHash { get; set; } = string.Empty;

    public string VerificationKey { get; set; } = string.Empty;

    public string HashAlgorithm { get; set; } = string.Empty;
}