using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Тренажёр;

namespace beta_0._4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            Form2 ford = new Form2();
            Hide();
            ford.rej = 1;
            ford.Text = "Шифрование методом подстановки";
            ford.ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 ford = new Form2();
            Hide();
            ford.rej = 2;
            ford.Text = "Дешифрование методом подстановки";
            ford.ShowDialog();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 ford = new Form2();
            Hide();
            ford.rej = 3;
            ford.Text = "Шифрование методом Цезаря";
            ford.ShowDialog();
            Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 ford = new Form2();
            Hide();
            ford.rej = 4;
            ford.Text = "Дешифрование методом Цезаря";
            ford.ShowDialog();
            Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Тренажёр.Help help = new Тренажёр.Help();            
            help.ShowDialog();
        }
    }
}
