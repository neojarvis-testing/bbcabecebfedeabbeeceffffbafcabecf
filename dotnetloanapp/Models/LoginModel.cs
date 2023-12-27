namespace BookStoreDBFirst.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AssignPlayerRequest
    {
        public long PlayerId { get; set; }
        public long TeamId { get; set; }
    }

    public class ReleasePlayerRequest
    {
        public long PlayerId { get; set; }
    }
}
