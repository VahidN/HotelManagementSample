using Microsoft.AspNetCore.Components.Forms;

namespace BlazorServer.Services;

public class FileUploadService : IFileUploadService
{
    private const int MaxBufferSize = 0x10000;

    public void DeleteFile(string fileName, string webRootPath, string uploadFolder)
    {
        var path = Path.Combine(webRootPath, uploadFolder, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public async Task<string> UploadFileAsync(IBrowserFile inputFile, string webRootPath, string uploadFolder)
    {
        if (inputFile is null)
        {
            throw new ArgumentNullException(nameof(inputFile));
        }

        CreateUploadDir(webRootPath, uploadFolder);
        var (fileName, imageFilePath) = GetOutputFileInfo(inputFile, webRootPath, uploadFolder);

        await using (var outputFileStream = new FileStream(
                                                           imageFilePath, FileMode.Create, FileAccess.Write,
                                                           FileShare.None, MaxBufferSize, true))
        {
            await using var inputStream = inputFile.OpenReadStream();
            await inputStream.CopyToAsync(outputFileStream);
        }

        return $"{uploadFolder}/{fileName}";
    }

    private static (string FileName, string FilePath) GetOutputFileInfo(
        IBrowserFile inputFile, string webRootPath, string uploadFolder)
    {
        var fileName = Path.GetFileName(inputFile.Name);
        var imageFilePath = Path.Combine(webRootPath, uploadFolder, fileName);
        if (File.Exists(imageFilePath))
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fileExtension = Path.GetExtension(fileName);
            fileName = $"{fileNameWithoutExtension}-{Guid.NewGuid()}{fileExtension}";
            imageFilePath = Path.Combine(webRootPath, uploadFolder, fileName);
        }

        return (fileName, imageFilePath);
    }

    private static void CreateUploadDir(string webRootPath, string uploadFolder)
    {
        var folderDirectory = Path.Combine(webRootPath, uploadFolder);
        if (!Directory.Exists(folderDirectory))
        {
            Directory.CreateDirectory(folderDirectory);
        }
    }
}