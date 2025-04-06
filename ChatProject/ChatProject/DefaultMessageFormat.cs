using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ChatProject
{
    public class DefaultMessageFormat : MessageFormatting
    {
        public override string FormatMessage(string message)
        {
            return message;
        }
    }
}
