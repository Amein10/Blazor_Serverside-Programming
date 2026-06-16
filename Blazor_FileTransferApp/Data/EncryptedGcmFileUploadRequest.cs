namespace Blazor_FileTransferApp.Data;

public class EncryptedGcmFileUploadRequest
{
    public string FileName { get; set; } = "";
    public string FileType { get; set; } = "";
    public string EncryptedFile { get; set; } = "";
    public string EncryptedKey { get; set; } = "";
    public string Nonce { get; set; } = "";
    public string Tag { get; set; } = "";
}