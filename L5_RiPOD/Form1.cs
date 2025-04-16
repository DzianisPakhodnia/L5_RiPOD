using System;
using System.Drawing;
using System.Windows.Forms;

namespace L5_RiPOD
{
    public partial class Form1 : Form
    {
        private ConvolutionGraph convolution;
        public int CurrentStep { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            convolution = new ConvolutionGraph();
            convolution.File_Load("start_param.txt");
            convolution.Planning();
            WriteResults();
        }

        private void WriteResults()
        {
            ClearView();

            richTextBox1.Text = "Количество шагов: " + (convolution.StepList.Count - 1);

            for(int i = 1; i < convolution.StepList.Count; i++)
            {
                richTextBox1.Text += "\n";
                richTextBox1.Text += "Шаг " + i + ": " + convolution.SeqParFunc[i - 1];
            }

            ViewResults();
            textBox1.Text = "";
            textBox1.Text = "0";
            CurrentStep = 0;
            buttonPrev.Enabled = false;
        }

        private void ViewResults()
        {
            // типы операций

            dataGridView1.ColumnCount = convolution.TypesCount + 1;
            dataGridView1.RowCount = convolution.OperationsCount + 1;
            dataGridView1.Rows[0].Cells[0].Value = "Operation \\  type";
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.RowHeadersVisible = false;

            for (int i = -1; i < convolution.TypesCount; i++)
            {
                dataGridView1.Rows[0].Cells[i + 1].Style.BackColor = Color.FromArgb(110, 110, 110);
                dataGridView1.Rows[0].Cells[i + 1].Style.ForeColor = Color.FromArgb(255, 255, 255);
                if (i > -1)
                {
                    dataGridView1.Rows[0].Cells[i + 1].Value = i + 1;
                    dataGridView1.Columns[i + 1].Width = 50;
                }
            }

            for (int i = 0; i < convolution.OperationsCount; i++)
            {
                dataGridView1.Rows[i + 1].Cells[0].Style.BackColor = Color.FromArgb(110, 110, 110);
                dataGridView1.Rows[i + 1].Cells[0].Style.ForeColor = Color.FromArgb(255, 255, 255);
                dataGridView1.Rows[i + 1].Cells[0].Value = i + 1;

            }

            for (int i = 0; i < convolution.TypesCount; i++)
            {
                for (int k = 0; k < convolution.ArrayTypes[i].Length; k++)
                {
                    dataGridView1.Rows[convolution.ArrayTypes[i][k]].Cells[i + 1].Style.BackColor = Color.FromArgb(82, 97, 160);
                }
            }

            // матрица смежности

            dataGridView2.ColumnCount = convolution.OperationsCount + 1;
            dataGridView2.RowCount = convolution.OperationsCount + 1;
            dataGridView2.ColumnHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;

            for (int i = 0; i < convolution.OperationsCount + 1; i++)
            {
                // по вертикали
                dataGridView2.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(110, 110, 110);
                dataGridView2.Rows[i].Cells[0].Style.ForeColor = Color.FromArgb(255, 255, 255);
                dataGridView2.Rows[i].Cells[0].Value = i;
                // по горизонали
                dataGridView2.Rows[0].Cells[i].Style.ForeColor = Color.FromArgb(255, 255, 255);
                dataGridView2.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(110, 110, 110);
                dataGridView2.Rows[0].Cells[i].Value = i;

                dataGridView2.Columns[i].Width = 50;
            }

            dataGridView2.Rows[0].Cells[0].Value = "Operation \\  operation";

            for (int i = 0; i < convolution.OperationsCount; i++)
            {
                for (int k = 0; k < convolution.ArrayH[i].Length; k++)
                {
                    if (convolution.ArrayH[i][k] == 1)
                    {
                        dataGridView2.Rows[i + 1].Cells[k + 1].Style.BackColor = Color.FromArgb(82, 97, 160);
                    }
                }
            }
        }

        private void ClearView()
        {
            dataGridView1.ColumnCount = 1;
            dataGridView1.RowCount = 1;
            dataGridView2.ColumnCount = 1;
            dataGridView2.RowCount = 1;
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            int value = 0;
            if (!Int32.TryParse(textBox1.Text, out value))
            {
                value = 0;
            }
            else
            {
                value--;
            }

            if (value == 0)
            {
                buttonPrev.Enabled = false;
            }
            else
            {
                buttonPrev.Enabled = true;
            }

            textBox1.Text = value.ToString();
            label3.Text = "Граф на шаге " + value + ":";
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            int value = 0;
            if (!Int32.TryParse(textBox1.Text, out value))
            {
                value = 0;
            }
            else
            {
                value++;
            }

            if (value == convolution.StepList.Count)
            {
                buttonNext.Enabled = false;
            }
            else
            {
                buttonNext.Enabled = true;
            }

            textBox1.Text = value.ToString();
            label3.Text = "Граф на шаге " + value + ":";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            if (!Int32.TryParse(textBox1.Text, out value))
            {
                value = 0;
            }
            
            if (value == 0)
            {
                buttonPrev.Enabled = false;
            }
            else
            {
                buttonPrev.Enabled = true;
            }

            if (value == convolution.StepList.Count - 1)
            {
                buttonNext.Enabled = false;
            }
            else
            {
                buttonNext.Enabled = true;
            }

            pictureBox1.Refresh();

           
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int index;
            if(Int32.TryParse(textBox1.Text, out index))
            {
                DrawGraph dgGraph = new DrawGraph((PictureBox) sender, 
                    convolution.StepList[index], e.Graphics);
                dgGraph.Draw();
            }

        }
    }
}
