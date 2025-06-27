using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedExchange
{
    public class HandshakeMessage
    {
        public string type { get; set; }
        public string username { get; set; }
        public string publicKey { get; set; }
    }
}
