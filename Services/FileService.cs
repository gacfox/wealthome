using Gacfox.Wealthome.Exceptions;

namespace Gacfox.Wealthome.Services;

public class FileService
{
    private readonly IConfiguration _configuration;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public FileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string SaveFile(IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension.ToLower())) throw new BusinessException("不支持的文件格式");

        string avatarSaveDir = _configuration.GetValue<string>("App:AvatarSaveDir");
        if (!Directory.Exists(avatarSaveDir)) Directory.CreateDirectory(avatarSaveDir);
        string fileName = Guid.NewGuid() + extension;
        string filePath = Path.Combine(avatarSaveDir, fileName);
        using FileStream fileStream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(fileStream);
        fileStream.Close();

        return fileName;
    }
}