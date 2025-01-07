using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Тренажёр
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources._1;
            //pictureBox1.Image = Bitmap.FromFile("ds.jpg");
            //pictureBox1.Size = new Size(pictureBox1.Width + 38, pictureBox1.Height + 38);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources._2;
            //pictureBox1.Image = Bitmap.FromFile("2.jpg");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources._3;
            //pictureBox1.Image = Bitmap.FromFile("3.jpg");
        }
    }
}
