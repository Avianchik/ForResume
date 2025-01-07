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
using Тренажёр;

namespace beta_0._4
{
    public partial class Form2 : Form
    {
        public int rej;

        public Form2()
        {
            InitializeComponent();
        }

        void conFil()
        {
            if (File.Exists("config.conf") == true)
            {
                int linesCount = 1;
                StreamReader testCont = new StreamReader("config.conf");
                while (!testCont.EndOfStream) if (testCont.Read() == '\n') linesCount++;
                testCont.Close();
                if (linesCount == 5)
                {
                    string[] lines = File.ReadAllLines("config.conf");
                    switch (rej)
                    {
                        case 1: if (lines[0] == "Shif_P: 1") button3.Enabled = true; else lines[0] = "Shif_P: 0"; break;
                        case 2: if (lines[1] == "Desh_A: 1") button3.Enabled = true; else lines[1] = "Desh_A: 0"; break;
                        case 3: if (lines[2] == "Shif_M: 1") button3.Enabled = true; else lines[2] = "Shif_M: 0"; break;
                        case 4: if (lines[3] == "Desh_M: 1") button3.Enabled = true; else lines[3] = "Desh_M: 0"; break;
                    }
                    File.WriteAllLines("config.conf", lines);
                }
                else
                {
                    StreamWriter streamWriter = new StreamWriter("config.conf");
                    streamWriter.WriteLine("Shif_P: 0");
                    streamWriter.WriteLine("Desh_A: 0");
                    streamWriter.WriteLine("Shif_M: 0");
                    streamWriter.WriteLine("Desh_M: 0");
                    streamWriter.Close();
                }
            }
            else
            {
                StreamWriter streamWriter = new StreamWriter("config.conf");
                streamWriter.WriteLine("Shif_P: 0");
                streamWriter.WriteLine("Desh_A: 0");
                streamWriter.WriteLine("Shif_M: 0");
                streamWriter.WriteLine("Desh_M: 0");
                streamWriter.Close();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            conFil();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
            //Environment.Exit(1); //Закрыть полностью программу
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (DialogResult.Yes == MessageBox.Show("Вы хотите закрыть программу?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //{
            //    Environment.Exit(1); //Закрыть полностью программу
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Theory ford = new Theory();
            Hide();
            ford.rej = rej;
            switch (rej)
            {
                case 1: ford.Text = "Шифрование методом подстановки"; break;
                case 2: ford.Text = "Дешифрование методом подстановки"; break;
                case 3: ford.Text = "Шифрование методом Цезаря"; break;
                case 4: ford.Text = "Дешифрование методом Цезаря"; break;
            }
            ford.ShowDialog();
            Show();
            conFil();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tren ford = new Tren();
            Hide();
            ford.rej = rej;
            switch (rej)
            {
                case 1: ford.Text = "Шифрование методом подстановки"; break;
                case 2: ford.Text = "Дешифрование методом подстановки"; break;
                case 3: 
                    ford.Text = "Шифрование методом Цезаря"; 
                    ford.Size = new Size(1900, 447);
                    break;
                case 4: 
                    ford.Text = "Дешифрование методом Цезаря";
                    ford.Size = new Size(1900, 447);
                    break;
            }
            ford.ShowDialog();
            Show();
            conFil();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TestControl testControl = new TestControl();
            Hide();
            testControl.rej = rej;
            switch (rej)
            {
                case 1: testControl.Text = "Шифрование методом подстановки"; break;
                case 2: testControl.Text = "Дешифрование методом подстановки"; break;
                case 3: 
                    testControl.Text = "Шифрование методом Цезаря"; 
                    testControl.Size = new Size(1900, 447);
                    testControl.MaximumSize = new Size(1900, 447);
                    testControl.MinimumSize = new Size(1900, 447);
                    break;
                case 4: 
                    testControl.Text = "Дешифрование методом Цезаря";
                    testControl.Size = new Size(1900, 447);
                    testControl.MaximumSize = new Size(1900, 447);
                    testControl.MinimumSize = new Size(1900, 447);
                    break;
            }
            testControl.ShowDialog();
            Show();
            conFil();
        }
    }
}
