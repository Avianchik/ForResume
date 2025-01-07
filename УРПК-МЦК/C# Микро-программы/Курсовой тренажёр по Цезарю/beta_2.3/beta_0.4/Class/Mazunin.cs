using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazunin
{
    internal class mazunin
    {
        public string Code(string s, int num)//Сгенерировать шифровку
        {
            //int number = Convert.ToInt32(num/*Число*/);
            return Encrypt(s, num);
        }

        const string alfabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        int letterQty = alfabet.Length;
        char retVal;
        char c;
        int index;
        string result;
        int codeIndex;

        public string Encrypt(string te, int key)//Шифровка
        =>  CodeEncode(te, key);

        public string Decrypt(string putFi1, int key)//Дешифровка
        => CodeEncode(putFi1, key);

        public int Position(char ch)
        {
            for (int i = 0; i < letterQty; i++) if ( ch == alfabet[i]) return i;
            return -1;
        }

        private string CodeEncode(string lineText, int k)
        {

            //добавляем в алфавит маленькие буквы
            retVal = ' ';
            result = "";
            for (int i = 0; i < lineText.Length; i++)
            {
                c = lineText[i];
                index = Position(c);
                if (index < 0)
                {
                    //если символ не найден, то добавляем его в неизменном виде
                    retVal = c;
                }
                else
                {
                    codeIndex = (letterQty + index + k) % letterQty;
                    retVal = alfabet[codeIndex];
                }
                result += retVal;
            }
            return result;
        }//Мазунин Шифр цезаря

        public string Encode(string putFi1, int num)//Сгенерировать дешифровку
        {
            //int number = Convert.ToInt32(num/*Число*/);
            return Decrypt(putFi1, -num);
        }
    }
}
