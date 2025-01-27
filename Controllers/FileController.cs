using Gacfox.Wealthome.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gacfox.Wealthome.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,MEMBER")]
public class FileController : ControllerBase
{
    private static readonly Dictionary<string, string> MimeMapping = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".webp", "image/webp" }
    };

    private readonly IConfiguration _configuration;

    public FileController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("[action]")]
    public ActionResult DownloadFile([FromQuery] string fileName)
    {
        string avatarSaveDir = _configuration.GetValue<string>("App:AvatarSaveDir");
        string filePath = Path.Combine(avatarSaveDir, fileName);
        string fullFilePath = Path.GetFullPath(filePath);
        if (!fullFilePath.StartsWith(avatarSaveDir, StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("找不到文件");
        if (!System.IO.File.Exists(fullFilePath)) throw new BusinessException("找不到文件");
        string extension = Path.GetExtension(fullFilePath);
        if (!MimeMapping.TryGetValue(extension, out string? mimeType)) throw new BusinessException("找不到文件");
        FileStream fileStream = new FileStream(fullFilePath, FileMode.Open);
        return File(fileStream, mimeType);
    }
}