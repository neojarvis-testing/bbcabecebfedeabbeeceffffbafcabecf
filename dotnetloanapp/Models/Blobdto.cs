namespace BookStoreDBFirst.Models
{
    public class BlobDto
    {
        public string Uri { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }

        public string FilePath { get; set; }
        // Add any other properties you need for a blob
    }

}
