using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shifr;
using Generation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Тренажёр;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static System.Net.WebRequestMethods;
using Mazunin;
using static System.Windows.Forms.AxHost;

namespace Тренажёр
{
    public partial class Tren : Form
    {
        public int rej; //Режим работы программы (1 - шифр. подст; 2 - дешифр. подст; 3 - шифр. Цез; 4 - дешифр. Цез;)
        string[] kay = new string[2]; //Массив с 2-мя ключами для шифр. и десшифр. методом подстановки
        string[] zadanie = { "привет", "пока", "слово", "программа", "тест" }; //Массив с словами для задания
        string otv; //Переменна для хранения ответа
        int zad = 0; //Переменная с номером задание (всего 5 заданий)
        int countwrong = 0, countwrongglobal = 0; //Переменные для счета попытак
        double stat = 0;


        public Tren()
        {
            InitializeComponent();
        }

        void newDataGr() //Создает таблицу с алфавитом и 2-х ключей для шифровки и дешифровки методом подстановки (НЕТРОГАТЬ)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            string al = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            for (int i = 0; i < 33; i++) dataGridView1.Rows[0].Cells[i].Value = al[i];
            for (int i = 0; i < 2; i++) for (int j = 0; j < 33; j++) dataGridView1.Rows[i + 1].Cells[j].Value = kay[i][j];
            dataGridView1.Rows[0].HeaderCell.Value = "Алфавит";
            dataGridView1.Rows[1].HeaderCell.Value = "Ключ 1";
            dataGridView1.Rows[2].HeaderCell.Value = "Ключ 2";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersWidth = 100;
        }

        public static string ShiftRussianAlphabet(int k) //Сдвиг алфавита на k
        {
            string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            int n = alphabet.Length;
            k %= n; // ensure k is within range
            //string shiftedAlphabet = alphabet.Substring(k) + alphabet.Substring(0, k);
            string shiftedAlphabet = alphabet.Substring(k) + alphabet.ToUpper() + alphabet;
            return shiftedAlphabet;
        }

        void newDataGr2(int k) //Создает таблицу с алфавитом и авфавит с двигом  для шифровки и дешифровки методом Цезаря
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            string al = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            string al1 = ShiftRussianAlphabet(k);
            for (int i = 0; i < 66; i++) dataGridView1.Rows[0].Cells[i].Value = al[i];
            for (int j = 0; j < 66; j++) dataGridView1.Rows[1].Cells[j].Value = al1[j];
            dataGridView1.Rows[0].HeaderCell.Value = "Алфавит";
            dataGridView1.Rows[1].HeaderCell.Value = "Алфавит 2";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersWidth = 100;
        }

        void nextZodan() //Следующее задание
        {
            label1.Text = "Задание " + Convert.ToString(zad + 1);
            switch (rej)
            {
                case 1:
                    label2.Text = "Зашифруйте слово методом подстановки: " + zadanie[zad];
                    GenKay genKay = new GenKay();
                    kay = genKay.ge();
                    Shif shif = new Shif();
                    otv = shif.SHIFRMAIN(kay[0], kay[1], zadanie[zad]); //Присваивание в переменную ответ
                    newDataGr();
                    break;

                case 2:
                    GenKay genKay1 = new GenKay();
                    kay = genKay1.ge();
                    otv = zadanie[zad]; //Присваивание в переменную ответ
                    newDataGr();
                    Shif shif1 = new Shif();
                    label2.Text = "Расшифруйте слово методом подстановки: " + shif1.SHIFRMAIN(kay[0], kay[1], zadanie[zad]);
                    break;

                case 3:
                    Random random = new Random(); //Создание класса рандома
                    int d = random.Next(1, 33); //Генирация рандомного числа (от 1 до 33)
                    mazunin mazunin = new mazunin();
                    otv = mazunin.Code(zadanie[zad], d); //Присваивание в переменную ответ
                    label2.Text = "Зашифруйте слово методом Цезаря шагом " + Convert.ToString(d) + ": " + zadanie[zad];
                    newDataGr2(d);
                    break;

                case 4:
                    Random random1 = new Random(); //Создание класса рандома
                    int d1 = random1.Next(1, 33); //Генирация рандомного числа (от 1 до 33)
                    mazunin mazunin1 = new mazunin();
                    label2.Text = "Расшифруйте слово методом Цезаря шагом " + Convert.ToString(d1) + ": " + mazunin1.Encode(zadanie[zad], d1);
                    otv = zadanie[zad]; //Присваивание в переменную ответ
                    newDataGr2(d1);
                    break;
            }
            textBox1.Width = otv.Length * 12 + 10; //Установка длинны textBox1
            textBox1.Text = "";
            label3.Visible = false; //Скрыть label3 для подсказки
            countwrong = 0; //Обнуление кол. ошибок
            button1.Enabled = false; //Блокировка кнопки "Далее"
        }
        private void Tren_Load(object sender, EventArgs e) //Запус формы
        {
            label1.Text = "Задание 1";
            switch (rej)
            {
                case 1:
                    for (int i = 0; i < 33; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                    for (int i = 0; i < 3; i++) dataGridView1.Rows.Add();
                    nextZodan();
                    newDataGr();
                    break;

                case 2:
                    for (int i = 0; i < 33; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                    for (int i = 0; i < 3; i++) dataGridView1.Rows.Add();
                    nextZodan();
                    newDataGr();
                    break;

                case 3:
                    for (int i = 0; i < 66; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                    for (int i = 0; i < 2; i++) dataGridView1.Rows.Add();
                    
                    //label2.Location = new Point(42, 40);
                    //label3.Location = new Point(42, 80);
                    //textBox1.Location = new Point(46, 120);
                    //button1.Location = new Point(46, 180);
                    //dataGridView1.Visible = false;

                    nextZodan();
                    break;

                case 4:
                    for (int i = 0; i < 66; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                    for (int i = 0; i < 2; i++) dataGridView1.Rows.Add();
                    //label2.Location = new Point(42, 40);
                    //label3.Location = new Point(42, 80);
                    //textBox1.Location = new Point(46, 120);
                    //button1.Location = new Point(46, 180);
                    //dataGridView1.Visible = false;
                    nextZodan();
                    break;
            }
            textBox1.Width = otv.Length * 12 + 10;
            dataGridView1.DefaultCellStyle.Font = new Font("Times New Roman", 13);  // Установка шрифта для ячеек DataGridView
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12); // Установка шрифта для заголовка строк DataGridView
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
        }

        private void button1_Click(object sender, EventArgs e) //Следущее задание (НЕТРОГАТЬ)
        {
            zad++;
            if (zad != 5)
            {
                stat = countwrongglobal;
                nextZodan();
            }
            else
            {
                string[] lines = System.IO.File.ReadAllLines("config.conf");
                switch (rej)
                {
                    case 1: lines[0] = "Shif_P: 1"; break;
                    case 2: lines[1] = "Desh_A: 1"; break;
                    case 3: lines[2] = "Shif_M: 1"; break;
                    case 4: lines[3] = "Desh_M: 1"; break;
                }
                System.IO.File.WriteAllLines("config.conf", lines);

                if (stat != 0)
                {
                    stat = stat / 5;
                }
                Hide();
                StatTren statTren = new StatTren();
                statTren.st = Convert.ToString(stat);
                statTren.rej = rej;
                statTren.ShowDialog();
                this.Close();
            };
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e) //Проверка на правильность введенной буквы (НЕТРОГАТЬ)
        {
            textBox1.MaxLength = otv.Length;
            StringBuilder sb = new StringBuilder(textBox1.Text);
            if (textBox1.Text!="")
            {
                if (textBox1.Text[textBox1.Text.Length-1] != otv[textBox1.Text.Length-1])
                {
                    sb.Remove(sb.Length - 1, 1);
                    textBox1.Text = sb.ToString();
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.SelectionLength = 0;
                    countwrong++;
                    countwrongglobal++;
                }
            }
            if (countwrong == 5)
            {
                label3.Visible = true;
                label3.Text = "Правильный ответ: " + otv;
            }
            if (textBox1.Text == otv)
            {
                button1.Enabled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
    }
}
