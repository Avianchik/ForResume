using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Generation
{
    internal class GenKay
    {
        string al = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        string al1 = "";
        int r;
        char c;
        public string[] ge()
        {
            string[] arr = new string[2];
            Random random = new Random();
            for (int i = 0; i < 33; i++)
            {
                r = random.Next(0, al.Length);
                c = al[r];
                al1 += c;
                al = al.Remove(r, 1);
            }
            arr[0] = al1;
            al = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            al1 = "";
            for (int i = 0; i < 33; i++)
            {
                r = random.Next(0, al.Length);
                c = al[r];
                al1 += c;
                al = al.Remove(r, 1);
            }
            arr[1] = al1;
            return arr;
        }
    }
}
