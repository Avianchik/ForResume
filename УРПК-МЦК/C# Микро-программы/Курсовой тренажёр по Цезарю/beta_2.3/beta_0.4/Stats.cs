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
    public partial class Stats : Form
    {
        public bool KR;
        public int rej;
        
        public string st;


        public string[,] MasOTV = new string[10,2];
        public Stats()
        {
            InitializeComponent();
        }

        private void Stats_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("Правильный ответ", "Правильный ответ");
            dataGridView1.Columns.Add("Ваш ответ", "Ваш ответ");
            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
            }

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersWidth = 70;


            int ColPrOtv = 0;

            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = MasOTV[i, 0];
                dataGridView1.Rows[i].Cells[1].Value = MasOTV[i, 1];

                if (MasOTV[i, 0] == MasOTV[i, 1])
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.YellowGreen;
                    ColPrOtv++;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.Crimson;
                }
            }

            label5.Text = "Количество правильных ответов: " + ColPrOtv + "/10\nОценка: " + ColPrOtv * 0.5;
            
        }
    }
}
