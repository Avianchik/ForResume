using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Тренажёр
{
    public partial class StatTren : Form
    {
        public string st;
        public int rej;
        public StatTren()
        {
            InitializeComponent();
        }

        private void StatTren_Load(object sender, EventArgs e)
        {
            label1.Text = "Вы прошли тест.\nСреднее количество ошибок в слове: " + st + ".";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            TestControl testControl = new TestControl();
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
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
