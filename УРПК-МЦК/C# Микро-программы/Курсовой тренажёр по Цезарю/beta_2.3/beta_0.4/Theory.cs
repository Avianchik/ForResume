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
using Тренажёр;

namespace beta_0._4
{
    public partial class Theory : Form
    {
        public int rej;

        string[] trhSifrP = new string[] { "ShifrovanieTheory\\1.jpg", "ShifrovanieTheory\\2.jpg", "ShifrovanieTheory\\3.jpg" };
        string[] trhDeshA = new string[] { "ShifTheory\\1.jpg", "ShifTheory\\2.jpg", "ShifTheory\\3.jpg" };
        string trhSifrM = "ShifrCesaurus\\1.jpg";
        int iPhot = 1;

        public Theory()
        {
            InitializeComponent();  
        }

        private void Theory_Load(object sender, EventArgs e)
        {            
            switch (rej)
            {
                case 1:
                    if (Directory.Exists("ShifrovanieTheory"))
                    {
                        if (File.Exists(trhSifrP[0]) && File.Exists(trhSifrP[1]) && File.Exists(trhSifrP[2]))
                        {
                            pictureBox1.Image = Bitmap.FromFile(trhSifrP[0]);
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно или нет теоретического материала!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Папки \"ShifrovanieTheory\" с теорией не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    break;
                case 2: 
                    if(Directory.Exists("ShifTheory"))
                    {
                        if (File.Exists(trhDeshA[0]) && File.Exists(trhDeshA[1]) && File.Exists(trhDeshA[2]))
                        {
                            pictureBox1.Image = Bitmap.FromFile(trhDeshA[0]);
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно или нет теоретического материала!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Папки \"ShifTheory\" с теорией не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                     
                    break;
                case 3: case 4:
                    if (Directory.Exists("ShifrCesaurus"))
                    {
                        if (File.Exists(trhSifrM))
                        {
                            pictureBox1.Image = Bitmap.FromFile(trhSifrM);
                            button1.Visible = button2.Visible = label1.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("Недостаточно или нет теоретического материала!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Папки \"ShifrCesaurus\" с теорией не существует!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    break;                    
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (rej)
            {
                case 1:
                    switch (iPhot)
                    {
                        case 1:
                            iPhot++;
                            button1.Enabled = true;
                            //button2.Enabled = false;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhSifrP[iPhot - 1]);
                            break;
                        case 2:
                            iPhot++;
                            button2.Enabled = false;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhSifrP[iPhot - 1]);
                            break;
                    }
                    break;

                case 2:
                    switch (iPhot)
                    {
                        case 1:
                            iPhot++;
                            button1.Enabled = true;
                            //button2.Enabled = false;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhDeshA[iPhot - 1]);
                            break;
                        case 2:
                            iPhot++;
                            button2.Enabled = false;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhDeshA[iPhot - 1]);
                            break;
                    }
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (rej)
            {
                case 1:
                    switch (iPhot)
                    {
                        case 2:
                            iPhot--;
                            button1.Enabled = false;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhSifrP[iPhot - 1]);
                            break;
                        case 3:
                            iPhot--;
                            button2.Enabled = true;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhSifrP[iPhot - 1]);
                            break;
                    }
                    break;

                case 2:
                    switch (iPhot)
                    {
                        case 2:
                            iPhot--;
                            button1.Enabled = false;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhDeshA[iPhot - 1]);
                            break;
                        case 3:
                            iPhot--;
                            button2.Enabled = true;
                            label1.Text = (iPhot).ToString() + "/3";
                            pictureBox1.Image = Bitmap.FromFile(trhDeshA[iPhot - 1]);
                            break;
                    }
                    break;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
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
            this.Close();
        }
    }
}
