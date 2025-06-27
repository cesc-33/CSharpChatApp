namespace SharedExchange
{
    public class EncryptedAesKeyMessage
    {
        public string type { get; set; } = "aesKeyExchange";
        public string from { get; set; }
        public string to { get; set; }
        public string encryptedKey { get; set; }  // Base64
        public string encryptedIV { get; set; }   // Base64
    }
}