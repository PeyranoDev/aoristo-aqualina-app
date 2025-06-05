using Microsoft.AspNetCore.Http;

public interface IBlobStorageService
{
    Task<bool> CreateContainerAsync(string containerName);
    Task<bool> DeleteContainerAsync(string containerName);
    Task<bool> DeleteFileAsync(string fileName, string containerName);
    Task<string> UploadFileAsync(IFormFile file, string containerName, string model, string color);
}