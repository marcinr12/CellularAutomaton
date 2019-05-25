using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellularAutomaton
{
    public partial class Form1 : Form
    {

        Graphics g;
        Bitmap bm;
        Grid grid = new Grid();
        int gridWidth = 40;
        int gridHeight = 20;
        byte whichRule = 30;
        int calculatedIteration = 0;


        public Form1()
        {
            InitializeComponent();
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pictureBox1.Image = bm;
            grid.SetGridSize(gridWidth, gridHeight);
            grid.InitialCells(pictureBox1);
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            grid.GetCells()[0][gridWidth / 2].SetState(true);
            grid.GetPreviousState()[0][gridWidth / 2] = true;
            calculatedIteration = 0;
            grid.PrintGrid(pictureBox1, g, bm);

        }

        private void Button2_Click(object sender, EventArgs e)
        {

            bool successY = Int32.TryParse(textBox1.Text, out gridHeight);
            bool successX = Int32.TryParse(textBox2.Text, out gridWidth);


            if (successX && successY)
            {
                grid.SetGridSize(gridWidth, gridHeight);
                grid.InitialCells(pictureBox1);
                calculatedIteration = 0;
                label1.Text = "New grid size: " + grid.GetCells().Count.ToString() + " x " + grid.GetCells()[0].Count.ToString();
            }
            else
            {
                label1.Text = "Fail to load new size.";
            }
        }


        private void Button3_Click(object sender, EventArgs e)
        {
            if (calculatedIteration < gridHeight - 1)
            {
                grid.CalculateRule(calculatedIteration, whichRule);
                calculatedIteration++;
                grid.PrintGrid(pictureBox1, g, bm);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                whichRule = 30;
            if (comboBox1.SelectedIndex == 1)
            {
                //label1.Text = "60";
                whichRule = 60;
            }
            if (comboBox1.SelectedIndex == 2)
                whichRule = 90;
            if (comboBox1.SelectedIndex == 3)
                whichRule = 120;
            if (comboBox1.SelectedIndex == 4)
                whichRule = 225;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            int iterationsToCalculate = 0;
            bool isConverted = Int32.TryParse(textBox3.Text, out iterationsToCalculate);
            
            if(isConverted)
            {
                label1.Text = "Number of iterations was loaded succesfully.";
                while (calculatedIteration < gridHeight - 1 && iterationsToCalculate > 0)
                {
                    grid.CalculateRule(calculatedIteration, whichRule);
                    calculatedIteration++;
                    iterationsToCalculate--;  
                }
                grid.PrintGrid(pictureBox1, g, bm);
            }
            else
            {
                label1.Text = "Fail to load number of iterations.";
            }
        }
    }
}

