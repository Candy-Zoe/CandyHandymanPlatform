namespace CandyHandyman.Infrastructure.Services;

public class LocalFileService : CandyHandyman.Application.Interfaces.IFileService
{
    private readonly string _uploadPath;

    public LocalFileService(string uploadPath = "uploads")
    {
        _uploadPath = uploadPath;
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var ext = Path.GetExtension(fileName);
        var newFileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(_uploadPath, newFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return $"/uploads/{newFileName}";
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        var filePath = fileUrl.TrimStart('/');
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }
}