using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellularAutomatonGoL
{
    public partial class Form1 : Form
    {
        Graphics g;
        Bitmap bm;
        Grid grid = new Grid();
        int gridCellWidth = 40;
        int gridCellHeight = 40;
        CheckNeighbouthood checkNeighbouthood;

        static bool paused = true;

        static bool[,] stillLife  = new bool[,] {   {false, true, true, false}, 
                                                    {true, false, false, true}, 
                                                    {false, true, true, false} };

        static bool[,] glider = new bool[,] {   {false, true, true},
                                                {true, true, false},
                                                {false, false, true} };

        static bool[,] oscillator = new bool[,] {{true},
                                                {true},
                                                {true} };

        //deep copy
        //static bool[,] toLoad = glider.Clone() as bool[,];

        //shallow copy
        static bool[,] toLoad = (bool[,])glider.Clone();

        public Form1()
        {
            InitializeComponent();
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pictureBox1.Image = bm;
            CreateGrid();

            checkNeighbouthood = new CheckNeighbouthood(grid.CheckNeighbouthoodBothContition);

        }

        private void CreateGrid()
        {
            grid.SetGridSize(gridCellWidth, gridCellHeight);
            grid.InitialCells(pictureBox1);
            grid.PrintGrid(pictureBox1, g, bm);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            paused = false;
            label1.Text = "Game is started!";
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            int dx = 0;
            int dy = 0;

            if (int.TryParse(textBox1.Text, out dx) && int.TryParse(textBox2.Text, out dy))
                label1.Text = "Success!";
            else
                label1.Text = "Fail!";

            grid.LoadPattern(toLoad, dx, dy);
            grid.PrintGrid(pictureBox1, g, bm);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            paused = true;
            label1.Text = "Game is paused!";
            List<List<Cell>> cells = grid.GetCells();
            for(int i = 0; i < cells.Count; i++)
            {
                for(int j = 0; j < cells[i].Count; j++)
                {
                    cells[i][j].SetState(false);
                    grid.SetPreviousStatus(-1, i, j);

                }
            }

            grid.PrintGrid(pictureBox1, g, bm);
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            List<List<Cell>> cells = grid.GetCells();

            for(int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if(coordinates.X > cells[i][j].GetPosition().X && coordinates.X < cells[i][j].GetPosition().X + cells[i][j].GetSize().Width &&
                        coordinates.Y > cells[i][j].GetPosition().Y && coordinates.Y < cells[i][j].GetPosition().Y + cells[i][j].GetSize().Height)
                    {
                        paused = true;
                        label1.Text = "Game is paused!";

                        cells[i][j].SetState(!cells[i][j].GetState());
                        grid.PrintGrid(pictureBox1, g, bm);
                        break;
                    }
                }
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedIndex == 0)
                checkNeighbouthood = new CheckNeighbouthood(grid.CheckNeighbouthoodBothContition);
            else if (comboBox1.SelectedIndex == 1)
                checkNeighbouthood = new CheckNeighbouthood(grid.CheckNeighbouthoodHorizontalContition);
            else if (comboBox1.SelectedIndex == 2)
                checkNeighbouthood = new CheckNeighbouthood(grid.CheckNeighbouthoodVerticalContition);
            else if (comboBox1.SelectedIndex == 3)
                checkNeighbouthood = new CheckNeighbouthood(grid.CheckNeighbouthoodNonContition);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!paused)
            {
                grid.CalculateNextGeneration(checkNeighbouthood);
                grid.PrintGrid(pictureBox1, g, bm);
            }

            Invalidate();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            paused = true;
            label1.Text = "Game is paused!";
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
                toLoad = oscillator;
            else if (comboBox2.SelectedIndex == 1)
                toLoad = glider;
            else if (comboBox2.SelectedIndex == 2)
                toLoad = stillLife;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            grid.GenerateRandomCellsState();
            grid.PrintGrid(pictureBox1, g, bm);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            int gridCHeight = 0;
            int gridCWidth = 0;
            pictureBox1.Size = new Size(500, 500);

            if (int.TryParse(textBox3.Text, out gridCWidth) && int.TryParse(textBox4.Text, out gridCHeight))
                label1.Text = "Success!";
            else
                label1.Text = "Fail!";

            gridCellHeight = gridCHeight;
            gridCellWidth = gridCWidth;

            int size = 0;
            double a = 0;
            if (gridCellHeight < gridCellWidth)
            {
                size = gridCellWidth;
                a = pictureBox1.Width / size;
                pictureBox1.Height = Convert.ToInt32(a * gridCellHeight);
            }
            else
            {
                size = gridCellHeight;
                a = pictureBox1.Height / size;
                pictureBox1.Width = Convert.ToInt32(a * gridCellWidth);
            }


            CreateGrid();
        }
    }
}
