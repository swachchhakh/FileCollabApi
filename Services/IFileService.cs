using Microsoft.AspNetCore.Http;

public interface IFileService {
    Task<string> UploadAsync(IFormFile file, string filename);
    Task<Stream> DownloadAsync(string id);
}
