using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedExchange
{
    public class EncryptedChatMessage
    {
        public string type {  get; set; } = "encryptedMessage";
        public string from { get; set; }
        public string to { get; set; }
        public string payload { get; set; } // Base64-verschlüsselter Text
    }
}
