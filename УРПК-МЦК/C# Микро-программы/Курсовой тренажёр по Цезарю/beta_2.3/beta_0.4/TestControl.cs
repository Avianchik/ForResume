using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Generation;
using Shifr;
using Mazunin;

namespace Тренажёр
{
    public partial class TestControl : Form
    {
        public int rej;
        string[] zadanie = new string[10];
        string[,] otvet = new string[10, 2];
        string[] kay = new string[2];
        string otv;
        int zad = 0;

        public TestControl()
        {
            InitializeComponent();
        }

        void newDataGr()
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

        void NextZad()
        {
            label1.Text = "Задание " + Convert.ToString(zad + 1);
            switch (rej)
            {
                case 1:
                    label2.Text = "Зашифруйте текст методом подстановки: " + zadanie[zad];
                    GenKay genKay = new GenKay();
                    kay = genKay.ge();
                    Shif shif = new Shif();
                    otv = shif.SHIFRMAIN(kay[0], kay[1], zadanie[zad]);
                    newDataGr();
                    break;

                case 2:
                    GenKay genKay1 = new GenKay();
                    kay = genKay1.ge();
                    otv = zadanie[zad];
                    newDataGr();
                    Shif shif1 = new Shif();
                    label2.Text = "Расшифруйте текст методом подстановки: " + shif1.SHIFRMAIN(kay[0], kay[1], zadanie[zad]);
                    break;

                case 3:
                    Random random = new Random(); //Создание класса рандома
                    int d = random.Next(1, 33); //Генирация рандомного числа (от 1 до 33)
                    mazunin mazunin = new mazunin();
                    otv = mazunin.Code(zadanie[zad], d); //Присваивание в переменную ответ
                    label2.Text = "Зашифруйте слово методом Цезаря шагом " + Convert.ToString(d) + ": " + zadanie[zad];
                    newDataGr2(d);

                    //otv = zadanie[zad];
                    break;

                case 4:
                    Random random1 = new Random(); //Создание класса рандома
                    int d1 = random1.Next(1, 33); //Генирация рандомного числа (от 1 до 33)
                    mazunin mazunin1 = new mazunin();
                    label2.Text = "Расшифруйте слово методом Цезаря шагом " + Convert.ToString(d1) + ": " + mazunin1.Encode(zadanie[zad], d1);
                    otv = zadanie[zad]; //Присваивание в переменную ответ
                    newDataGr2(d1);

                    //otv = zadanie[zad];
                    break;
            }
            textBox1.Width = otv.Length * 12 + 10;
            textBox1.Text = "";
        }
        

        private void TestControl_Load(object sender, EventArgs e)
        {
            FileInfo test1 = new FileInfo("Questions.txt");
            if (test1.Exists)
            {
                string[] lines = File.ReadAllLines("Questions.txt");
                lines = lines.Select(line => line.Trim()).ToArray();
                lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
                File.WriteAllLines("Questions.txt", lines);

                if (lines.Length < 10) { MessageBox.Show("Количество строк в файле с вопросами \"Questions.txt\" меньше 10!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); this.Close(); }
                else
                {
                    int numLine;
                    string TextLine = "";
                    Random rand = new Random();
                    int j = 0;
                    while (j != 10)
                    {
                        numLine = rand.Next(0, lines.Length);
                        TextLine = lines[numLine];
                        bool b = true;
                        for (int i = 0; i < j; i++)
                        {
                            if (TextLine == zadanie[i])
                            {
                                b = false;
                                break;
                            }
                        }
                        if (b) { zadanie[j] = TextLine; j++; }
                    }

                    switch (rej)
                    {
                        case 1:
                            for (int i = 0; i < 33; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                            for (int i = 0; i < 3; i++) dataGridView1.Rows.Add();
                            NextZad();
                            newDataGr();
                            break;

                        case 2:
                            for (int i = 0; i < 33; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                            for (int i = 0; i < 3; i++) dataGridView1.Rows.Add();
                            NextZad();
                            newDataGr();
                            break;

                        case 3:
                            for (int i = 0; i < 66; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                            for (int i = 0; i < 2; i++) dataGridView1.Rows.Add();
                            NextZad();
                            break;

                        case 4:
                            for (int i = 0; i < 66; i++) dataGridView1.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
                            for (int i = 0; i < 2; i++) dataGridView1.Rows.Add();
                            NextZad();
                            break;
                    }
                    textBox1.Width = otv.Length * 12 + 10;
                    dataGridView1.DefaultCellStyle.Font = new Font("Times New Roman", 13); // Установка шрифта для ячеек DataGridView
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12); // Установка шрифта для заголовка строк DataGridView
                    dataGridView1.AllowUserToResizeColumns = false;
                    dataGridView1.AllowUserToResizeRows = false;
                }
            }
            else
            {
                MessageBox.Show("Файла \"Questions.txt\" не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            otvet[zad, 0] = otv;
            otvet[zad, 1] = textBox1.Text;
            zad++;
            if (zad != 10) NextZad();
            else
            {
                Hide();
                Stats stats = new Stats();
                switch (rej)
                {
                    case 1: stats.Text = "Шифрование методом подстановки"; break;
                    case 2: stats.Text = "Дешифрование методом подстановки"; break;
                    case 3: stats.Text = "Шифрование методом Цезаря"; break;
                    case 4: stats.Text = "Дешифрование методом Цезаря"; break;
                }
                stats.MasOTV = otvet;
                stats.KR = true;
                stats.ShowDialog();
                this.Close();
            }
            
        }
    }
}
