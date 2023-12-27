using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookStoreDBFirst.Models;
using BookStoreDBFirst.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;

namespace BookStoreDBFirst.Repository
{
    public class AzureStorage : IAzureStorage
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureStorage> _logger;

        // Constructor using IConfiguration
        public AzureStorage(IConfiguration configuration, ILogger<AzureStorage> logger)

        {
            _storageConnectionString = configuration.GetConnectionString("BlobConnectionString");   
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
            _logger = logger;
            _blobServiceClient = new BlobServiceClient(_storageConnectionString);
        }

        // Constructor using explicit parameters
        public AzureStorage(string storageConnectionString, string storageContainerName, ILogger<AzureStorage> logger)
        {
            _storageConnectionString = storageConnectionString;
            _storageContainerName = storageContainerName;
            _logger = logger;
            _blobServiceClient = new BlobServiceClient(_storageConnectionString);
        }

        [HttpGet("list")]
        public async Task<List<BlobDto>> ListAsync()
        {
            try
            {
                // Get a reference to a container named in appsettings.json
                BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(_storageContainerName);

                // Create a new list object for 
                List<BlobDto> files = new List<BlobDto>();

                await foreach (BlobItem blobItem in container.GetBlobsAsync())
                {
                    var blobClient = container.GetBlobClient(blobItem.Name);
                    var blobProperties = await blobClient.GetPropertiesAsync();

                    files.Add(new BlobDto
                    {
                        Uri = blobClient.Uri.ToString(),
                        Name = blobItem.Name,
                        ContentType = blobProperties.Value.ContentType
                    });
                }

                // Return all files to the requesting method
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error listing blobs: {ex.Message}");
                throw;
            }
        }

        public async Task<BlobResponseDto> UploadAsync(List<IFormFile> files)
        {
            var responseDto = new BlobResponseDto { Blobs = new List<BlobDto>() };

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    responseDto.Error = true;
                    responseDto.Status = "File is required";
                    return responseDto;
                }

                try
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(_storageContainerName);

                    // Generate a unique filename or use the original filename
                    var fileName = $"{Guid.NewGuid().ToString()}_{Path.GetFileName(file.FileName)}";

                    var blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = file.OpenReadStream())
                    {
                        var blobResponse = await blobClient.UploadAsync(stream, true);
                        if (blobResponse.GetRawResponse().Status != 201)
                        {
                            responseDto.Error = true;
                            responseDto.Status = "Error uploading file to Azure Blob Storage";
                            return responseDto;
                        }
                    }

                    var filePath = blobClient.Uri.ToString();
                    var blobDto = new BlobDto { FilePath = filePath };
                    responseDto.Blobs.Add(blobDto);
                }
                catch (Exception ex)
                {
                    responseDto.Error = true;
                    responseDto.Status = $"Error uploading file: {ex.Message}";
                    return responseDto;
                }
            }

            responseDto.Error = false;
            responseDto.Status = "Files uploaded successfully";
            return responseDto;
        }
    }
}
