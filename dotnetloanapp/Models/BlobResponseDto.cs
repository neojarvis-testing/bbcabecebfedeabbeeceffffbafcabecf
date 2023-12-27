namespace BookStoreDBFirst.Models
{
    public class BlobResponseDto
    {

        public string? Status { get; set; }
        public bool Error { get; set; }
        public List<BlobDto> Blobs { get; set; }

        public BlobResponseDto()
        {
            Blobs = new List<BlobDto>();
        }

    }
}

