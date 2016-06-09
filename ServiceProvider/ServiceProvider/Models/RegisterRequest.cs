namespace ServiceProvider.Models
{
    public class RegisterRequest
    {
        public string salt_masterkey { get; set; }
        public string pubkey { get; set; }
        public string privkey_enc { get; set; }
    }
}