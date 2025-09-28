using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class FileService : IFileService
{
    private readonly IMongoDatabase _db;
    private readonly GridFSBucket _bucket;

    public FileService(IMongoDatabase db)
    {
        _db = db;
        _bucket = new GridFSBucket(_db);
    }

  public async Task<string> UploadAsync(IFormFile file, string fileName)
{
    using var stream = file.OpenReadStream();
    var objectId = await _bucket.UploadFromStreamAsync(fileName, stream);
    return objectId.ToString(); // convert ObjectId to string
}


    public async Task<Stream> DownloadAsync(string id)
    {
        try
        {
            var objectId = new MongoDB.Bson.ObjectId(id);
            return await _bucket.OpenDownloadStreamAsync(objectId);
        }
        catch
        {
            return null;
        }
    }
}