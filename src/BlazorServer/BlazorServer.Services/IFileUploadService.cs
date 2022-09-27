using Microsoft.AspNetCore.Components.Forms;

namespace BlazorServer.Services;

public interface IFileUploadService
{
    void DeleteFile(string fileName, string webRootPath, string uploadFolder);
    Task<string> UploadFileAsync(IBrowserFile inputFile, string webRootPath, string uploadFolder);
}