using Gacfox.Wealthome.Exceptions;
using MimeDetective;

namespace Gacfox.Wealthome.Services;

public class FileService
{
    private readonly IConfiguration _configuration;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };

    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetAvatarSaveDir()
    {
        return _configuration.GetValue<string>("App:AvatarSaveDir") ??
               throw new InvalidOperationException("缺少配置: App:AvatarSaveDir");
    }

    public async Task<string> SaveAvatarAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        // 检查扩展名
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension.ToLower())) throw new BusinessException("不支持的文件格式");

        // 检查二进制类型
        var inspector = new ContentInspectorBuilder()
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        }.Build();
        var inspectResult = inspector.Inspect(file.OpenReadStream());
        var mimeResult = inspectResult.ByMimeType();
        string? mimeType = (mimeResult == null || mimeResult.Length == 0) ? null : mimeResult[0].MimeType;
        if (string.IsNullOrEmpty(mimeType) || !AllowedMimeTypes.Contains(mimeType))
        {
            throw new BusinessException("不支持的文件格式");
        }

        string avatarSaveDir = GetAvatarSaveDir();
        if (!Directory.Exists(avatarSaveDir)) Directory.CreateDirectory(avatarSaveDir);
        string fileName = Guid.NewGuid() + extension;
        string filePath = Path.Combine(avatarSaveDir, fileName);
        await using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
            FileShare.None, 4096, true);
        await file.CopyToAsync(fileStream, cancellationToken);

        return fileName;
    }

    public void DeleteAvatar(String fileName)
    {
        string avatarSaveDir = GetAvatarSaveDir();
        if (!Directory.Exists(avatarSaveDir)) Directory.CreateDirectory(avatarSaveDir);
        string filePath = Path.Combine(avatarSaveDir, fileName);
        string fullFilePath = Path.GetFullPath(filePath);
        if (fullFilePath.StartsWith(avatarSaveDir, StringComparison.OrdinalIgnoreCase) && File.Exists(fullFilePath))
        {
            File.Delete(filePath);
        }
    }
}