using BookStoreDBFirst.Models;

namespace BookStoreDBFirst.Models
{

    public interface IAzureStorage
    {
        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <returns>Blob with status</returns>
        
        Task<BlobResponseDto> UploadAsync(List<IFormFile> files);

        /// <summary>
        /// This method lists all blobs in the storage
        /// </summary>
        /// <returns>List of blobs</returns>
        Task<List<BlobDto>> ListAsync();
    }
    
}
