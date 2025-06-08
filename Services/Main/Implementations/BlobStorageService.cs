using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const long MaxFileSizeBytes = 1_572_864;
    private readonly string _accountName;

    public BlobStorageService(string storageAccountUri)
    {
        var credential = new DefaultAzureCredential();
        _blobServiceClient = new BlobServiceClient(new Uri(storageAccountUri), credential);
        _accountName = new Uri(storageAccountUri).Host.Split('.')[0];
    }

    public async Task<string> UploadFileAsync(IFormFile file, string containerName, string model, string color)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Archivo no válido.");

        if (!file.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
            file.ContentType != "image/png")
            throw new InvalidOperationException("Solo se permiten archivos PNG.");

        if (file.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("El archivo excede el tamaño máximo permitido de 1.5 MB.");

        using var image = await Image.LoadAsync(file.OpenReadStream());
        if (image.Width <= image.Height)
            throw new InvalidOperationException("La imagen debe ser horizontal (ancho mayor que alto).");

        string fileName = $"{NormalizeBlobName(model)}_{NormalizeBlobName(color)}.png";

        var containerClient = await GetOrCreateContainerAsync(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        file.OpenReadStream().Position = 0;
        await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders
        {
            ContentType = "image/png"
        });

        return blobClient.Uri.ToString();
    }
    public async Task<bool> VehicleImageExists(string model, string color)
    {
        var fileName = $"{NormalizeBlobName(model)}_{NormalizeBlobName(color)}.png";
        var containerClient = _blobServiceClient.GetBlobContainerClient("vehicles");
        var blobClient = containerClient.GetBlobClient(fileName);
        return await blobClient.ExistsAsync();
    }
    public string GenerateVehicleImageUrl(string model, string color)
    {
        var normalizedModel = NormalizeBlobName(model);
        var normalizedColor = NormalizeBlobName(color);
        return $"https://{_accountName}.blob.core.windows.net/vehicles/{normalizedModel}_{normalizedColor}.png";
    }

    public string NormalizeBlobName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "default";

        return Regex.Replace(input.Trim().ToLower(), @"[^a-z0-9]+", "_");
    }

    public async Task<bool> DeleteFileAsync(string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        if (await blobClient.ExistsAsync())
        {
            await blobClient.DeleteIfExistsAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> CreateContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var response = await containerClient.CreateIfNotExistsAsync();
        return response != null;
    }

    public async Task<bool> DeleteContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        if (await containerClient.ExistsAsync())
        {
            await containerClient.DeleteIfExistsAsync();
            return true;
        }

        return false;
    }

    private async Task<BlobContainerClient> GetOrCreateContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient;
    }
}
