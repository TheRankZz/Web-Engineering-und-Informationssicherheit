namespace ServiceProvider.Models
{
    public class SendMessageRequest
    {
        public Message inner_envelope { get; set; }
        public string receiver { get; set; }
        public string timestamp { get; set; }
        public string sig_service { get; set; }
    }
}