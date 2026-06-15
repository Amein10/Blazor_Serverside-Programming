namespace Blazor_Serverside_Programming.Data;

public class EncryptedFileUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string EncryptedFile { get; set; } = string.Empty;
    public string EncryptedKey { get; set; } = string.Empty;
    public string IV { get; set; } = string.Empty;
}