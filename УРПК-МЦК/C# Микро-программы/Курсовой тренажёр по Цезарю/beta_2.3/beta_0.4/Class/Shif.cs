using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Shifr
{
    internal class Shif
    {
        public string alfMal = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string alfBol = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        public int Rus(char ch)
        {
            for (int i = 0; i < 33; i++)
            {
                if (alfMal[i] == ch) return 1;
                else if (alfBol[i] == ch) return 2;
            }
            return 0;
        }
        public int Position(char ch)
        {
            for (int i = 0; i < 33; i++) if (ch == alfMal[i] || ch == alfBol[i]) return i;
            return -1;
        }
        public string SHIFRMAIN(string key1, string key2, string lineText)
        {
            string lineText2;
            int reg, post;
            char c;
            lineText2 = "";
            for (int i = 0; i < lineText.Length; i++)
            {
                reg = Rus(lineText[i]);
                if (reg != 0)
                {
                    post = Position(lineText[i]);
                    c = key1[post];
                    post = Position(c);
                    c = key2[post];
                    if(reg == 2) c = alfBol[Position(c)];
                } else c = lineText[i];
                lineText2 += c;
            }
            return lineText2;
        }
    }
}
