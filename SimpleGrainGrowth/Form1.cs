using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleGrainGrowth
{
    public partial class Form1 : Form
    {
        private readonly Graphics g;
        private readonly Bitmap bm;
        private Grid grid;
        private static Random random = new Random();
        private static List<Color> randomColours = new List<Color>();

        CheckNeighbourhood checkNeighbouthood;


        public Form1()
        {
            InitializeComponent();

            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pictureBox1.Image = bm;
            grid = new Grid(pictureBox1, 33, 33);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodAbsorbing);
            grid.PrintMesh(pictureBox1, g, bm);

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            double radius = 0;

            if (double.TryParse(textBox8.Text, out radius))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            if (grid.GetGrains() > 0)
            {
                grid.CalculateNextGeneration(checkNeighbouthood, Convert.ToInt32(radius * grid.GetCells()[0][0].GetSize().Height));

                grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
                if (checkBox1.Checked)
                    grid.PrintMesh(pictureBox1, g, bm);
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            List<List<Cell>> cells = grid.GetCells();

            


            for (int i = 0; i < cells.Count; i++)
            {               
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (coordinates.X > cells[i][j].GetPosition().X && coordinates.X < cells[i][j].GetPosition().X + cells[i][j].GetSize().Width &&
                        coordinates.Y > cells[i][j].GetPosition().Y && coordinates.Y < cells[i][j].GetPosition().Y + cells[i][j].GetSize().Height &&
                        cells[i][j].GetType() != Convert.ToInt32(numericUpDown1.Value))
                    {
                        if (Convert.ToInt32(numericUpDown1.Value) == 0)
                            grid.DecrementGrains();
                        else if (cells[i][j].GetType() == 0)
                            grid.IncrementGrain();
                        
                        cells[i][j].SetType(Convert.ToInt32(numericUpDown1.Value));
                        if (Convert.ToInt32(numericUpDown1.Value) > randomColours.Count)
                            AddTillRandomColorsList(Convert.ToInt32(numericUpDown1.Value));
                        grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
                        if (checkBox1.Checked)
                            grid.PrintMesh(pictureBox1, g, bm);

                        break;
                    }
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ClearGrid();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            int gridCellHeight = 0;
            int gridCellWidth = 0;

            if (int.TryParse(textBox1.Text, out gridCellWidth) && int.TryParse(textBox2.Text, out gridCellHeight))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            grid = new Grid(pictureBox1, gridCellHeight, gridCellWidth);
            checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodAbsorbing);
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ClearGrid();

            int grainsX = 0;
            int grainsY = 0;

            if (int.TryParse(textBox3.Text, out grainsX) && int.TryParse(textBox4.Text, out grainsY))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            grid.SetGrains(Convert.ToUInt32(grainsX * grainsY));
            CreateRandomColorsList(grainsX * grainsY);

            int dx = grid.GetCells()[0].Count / grainsX;
            int dy = grid.GetCells().Count / grainsY;

            int cellType = 1;
            for(int i = 0; i < grainsX; i++)
            {
                for(int j = 0; j < grainsY; j++)
                {
                    grid.SetCellType(dy / 2 + j * dy, dx / 2 + i * dx, cellType);
                    cellType++;
                }
            }


            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);

        }

        private void Button5_Click(object sender, EventArgs e)
        {
            ClearGrid();

            int cellsCounter = 0;

            if (int.TryParse(textBox5.Text, out cellsCounter))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            if (cellsCounter > grid.GetCells().Count * grid.GetCells()[0].Count)
            {
                cellsCounter = grid.GetCells().Count * grid.GetCells()[0].Count;
                Logs.Text = "Set: " + cellsCounter;
            }            

            grid.SetGrains(Convert.ToUInt32(cellsCounter));
            CreateRandomColorsList(cellsCounter);

            int setCells = 0;
            while(setCells < cellsCounter)
            {
                int x = random.Next(0, grid.GetCells().Count);
                int y = random.Next(0, grid.GetCells()[0].Count);
                if (grid.GetCells()[x][y].GetType() == 0)
                {
                    setCells++;
                    grid.SetCellType(x, y, setCells);
                }
            }

            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);

        }

        private void Button6_Click(object sender, EventArgs e)
        {
            int cellNumber = grid.GetCellHeight() * grid.GetCellWidth();
            uint grains = 0;

            double radius = 0;

            if (double.TryParse(textBox8.Text, out radius))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            while (grid.GetGrains() != grains && grid.GetGrains() > 0)
            {
                grains = grid.GetGrains();
                grid.CalculateNextGeneration(checkNeighbouthood, Convert.ToInt32(radius * grid.GetCells()[0][0].GetSize().Height));
            }

            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            ClearGrid();
            int radius = 0;
            int cellNumber;
            if (int.TryParse(textBox6.Text, out cellNumber) && int.TryParse(textBox7.Text, out radius))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            grid.SetGrains(Convert.ToUInt32(cellNumber));
            CreateRandomColorsList(cellNumber);
            List<int> randomX = new List<int>();
            List<int> randomY = new List<int>();

            int attemps = 100000;

            while (randomX.Count < cellNumber && attemps > 0)
            {
                int x = random.Next(0, grid.GetCells().Count);
                int y = random.Next(0, grid.GetCells()[0].Count);

                if(comboBox1.SelectedIndex == 0 && CheckRadiusAbsorbing(randomX, randomY, x, y, radius) && (!randomX.Contains(x) || !randomY.Contains(y)))
                {
                    randomX.Add(x);
                    randomY.Add(y);
                    attemps = 100000;
                }
                else if (comboBox1.SelectedIndex == 1 && CheckRadiusPeriodic(randomX, randomY, x, y, radius) && (!randomX.Contains(x) || !randomY.Contains(y)))
                {
                    randomX.Add(x);
                    randomY.Add(y);
                    attemps = 100000;
                }
                attemps--;
            }

            if(attemps <= 0)
            {
                Logs.Text = "Set: " + randomX.Count;
            }
            
            for (int i = 0; i < randomX.Count; i++)
                grid.SetCellType(randomX[i], randomY[i], i + 1);

            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);
        }

        private bool CheckRadiusAbsorbing(List<int> listX, List<int> listY, int x, int y, int r)
        {
            for(int i = 0; i < listX.Count; i++)
            {
                if ((Math.Pow(listX[i] - x, 2) + Math.Pow(listY[i] - y, 2)) <= Math.Pow(r, 2))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckRadiusPeriodic(List<int> listX, List<int> listY, int x, int y, int r)
        {
            for (int i = 0; i < listX.Count; i++)
            {
                int dx = grid.GetCells().Count;
                int dy = grid.GetCells()[0].Count;
                if ((Math.Pow(listX[i] - x, 2) + Math.Pow(listY[i] - y, 2)) <= Math.Pow(r, 2))
                    return false;
                if ((Math.Pow(listX[i] - (x + dx), 2) + Math.Pow(listY[i] - y, 2)) <= Math.Pow(r, 2))
                    return false;
                if ((Math.Pow(listX[i] - x, 2) + Math.Pow(listY[i] - (y + dy), 2)) <= Math.Pow(r, 2))
                    return false;
                if ((Math.Pow(listX[i] - (x + dx), 2) + Math.Pow(listY[i] - (y + dy), 2)) <= Math.Pow(r, 2))
                    return false;
                if ((Math.Pow(listX[i] - (x - dx), 2) + Math.Pow(listY[i] - y, 2)) <= Math.Pow(r, 2))
                    return false;
                if ((Math.Pow(listX[i] - x, 2) + Math.Pow(listY[i] - (y - dy), 2)) <= Math.Pow(r, 2))
                    return false;
                if ((Math.Pow(listX[i] - (x - dx), 2) + Math.Pow(listY[i] - (y - dy), 2)) <= Math.Pow(r, 2))
                    return false;
            }
            return true;
        }

        private void ClearGrid()
        {
            List<List<Cell>> cells = grid.GetCells();
            for (int i = 0; i < cells.Count; i++)
                for (int j = 0; j < cells[i].Count; j++)
                    cells[i][j].SetType(0);
            grid.SetGrains(Convert.ToUInt32(0));
            CreateRandomColorsList(0);

            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);


        }

        public Color RandomColor()
        {
            int red = random.Next(0, 255);
            int green = random.Next(0, 255);
            int blue = random.Next(0, 255);

            return Color.FromArgb(red, green, blue);
        }

        public void CreateRandomColorsList(int count)
        {
            randomColours = new List<Color>();
            for (int i = 0; i < count; i++)
                randomColours.Add(RandomColor());
        }

        public void AddTillRandomColorsList(int count)
        {
            while (randomColours.Count < count)
                randomColours.Add(RandomColor());
        }

        public static List<Color> GetRandomColorsList()
        {
            return randomColours;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 10 && comboBox1.SelectedIndex == 0)
                checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodRadiusAbsorbing);
            else if (comboBox2.SelectedIndex == 10 && comboBox1.SelectedIndex == 1)
                checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodRadiusPeriodic);
            else if (comboBox1.SelectedIndex == 0)
                checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodAbsorbing);
            else if (comboBox1.SelectedIndex == 1)
                checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodPeriodic);
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 10 && comboBox1.SelectedIndex == 0)
                checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodRadiusAbsorbing);
            else if (comboBox2.SelectedIndex == 10 && comboBox1.SelectedIndex == 1)
                checkNeighbouthood = new CheckNeighbourhood(grid.CheckNeighbourhoodRadiusPeriodic);
            else if (comboBox2.SelectedIndex == 0)
                grid.SetPatternIndex(0);
            else if (comboBox2.SelectedIndex == 1)
                grid.SetPatternIndex(1);
            else if (comboBox2.SelectedIndex == 2)
                grid.SetPatternIndex(2);
            else if (comboBox2.SelectedIndex == 3)
                grid.SetPatternIndex(3);
            else if (comboBox2.SelectedIndex == 4)
                grid.SetPatternIndex(4);
            else if (comboBox2.SelectedIndex == 5)
                grid.SetPatternIndex(5);
            else if (comboBox2.SelectedIndex == 6)
                grid.SetPatternIndex(6);
            else if (comboBox2.SelectedIndex == 7)
                grid.SetPatternIndex(7);
            else if (comboBox2.SelectedIndex == 8)
                grid.SetPatternIndex(8);
            else if (comboBox2.SelectedIndex == 9)
                grid.SetPatternIndex(9);

            ComboBox1_SelectedIndexChanged(sender, e);
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);
        }



        private void Button8_Click(object sender, EventArgs e)
        {
            int counter;

            if (int.TryParse(textBox9.Text, out counter))
                Logs.Text = "Success!";
            else
                Logs.Text = "Fail!";

            if (comboBox1.SelectedIndex == 0)
                for (int i = 0; i < counter; i++)
                    grid.MonteCarloAbsorbing();
            else if (comboBox1.SelectedIndex == 1)
                Logs.Text = "TO DO";

                grid.PrintGrid(pictureBox1, g, bm, checkBox2.Checked);
            if (checkBox1.Checked)
                grid.PrintMesh(pictureBox1, g, bm);

        }



    }
}
