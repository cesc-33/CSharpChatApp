using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProject
{
    public class RTFTextFormatter : ITextFormatter
    {
        public Font ApplyFontStyle(Font currentFont, FontStyle style, bool enable)
        {
            if (currentFont == null)
                currentFont = new Font("Arial", 10);

            FontStyle newStyle = enable
                ? currentFont.Style | style      // Stil hinzufügen
                : currentFont.Style & ~style;    // Stil entfernen

            // Alle Fonteigenschaften (currentFont) mit neuem Fontstil übernehmen
            return new Font(currentFont, newStyle);
        }

        public string FormatMessage(string message)
        {
            // Für Später Emojis, etc.
            return message;
        }
    }
}
