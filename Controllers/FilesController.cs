using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StackExchange.Redis;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase {
    private readonly IFileService _fileService;
    private readonly IMongoCollection<FileMeta> _filesCollection;
    private readonly IDatabase _redisDb; // StackExchange.Redis IDatabase via DI

    public FilesController(IFileService fileService, IMongoDatabase db, IConnectionMultiplexer redis) {
        _fileService = fileService;
        _filesCollection = db.GetCollection<FileMeta>("files");
        _redisDb = redis.GetDatabase();
    }

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string folderId) {
        var id = await _fileService.UploadAsync(file, file.FileName);
        var meta = new FileMeta {
            Id = id,
            Name = file.FileName,
            FolderId = folderId,
            OwnerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty,
            Size = file.Length
        };
        await _filesCollection.InsertOneAsync(meta);
        await _redisDb.StringSetAsync($"file:{id}", System.Text.Json.JsonSerializer.Serialize(meta));
        return Ok(meta);
    }

    [Authorize]
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(string id) {
        var stream = await _fileService.DownloadAsync(id);
        if(stream == null) return NotFound();
        stream.Position = 0;
        return File(stream, "application/octet-stream", $"{id}");
    }

    // list files by folder, delete, meta endpoints omitted but similar
}
