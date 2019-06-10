using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleGrainGrowth
{
    public delegate List<List<int>> CheckNeighbourhood(int radius = 0);



    internal class Grid
    {
        private List<List<Cell>> cells = new List<List<Cell>>();
        private int gridCellWidth = 10;
        private int gridCellHeight = 10;
        private double cellSize = 0;
        private Random random;// = new Random();
        private uint grains = 0;
        private Color[] energyColors = { Color.FromArgb(255, 255, 255),
                    Color.FromArgb(102, 255, 255),
                    Color.FromArgb(0, 204, 204),
                    Color.FromArgb(178, 255, 102),
                    Color.FromArgb(102, 204, 0),
                    Color.FromArgb(255, 102, 102),
                    Color.FromArgb(204, 0, 0),
                    Color.FromArgb(0, 0, 0) };
        private Boolean[,] lastRecrystallized;


        private Dictionary<int, int[]> patterns = new Dictionary<int, int[]>();
        int selectedPatternIndex = 0;

        public Grid(PictureBox pb, Random random, int gridCellHeight, int gridCellWidth)
        {
            this.random = random;

            this.gridCellHeight = gridCellHeight;
            this.gridCellWidth = gridCellWidth;

            double cellSizeWidth = Convert.ToDouble(pb.Width / (gridCellWidth * 1.0));
            double cellSizeHeight = Convert.ToDouble(pb.Height / (gridCellHeight * 1.0));
            this.cellSize = (cellSizeHeight < cellSizeWidth) ? cellSizeHeight : cellSizeWidth;
            this.lastRecrystallized = new Boolean[this.gridCellHeight, this.gridCellWidth];


            InitialPatterns();
            InitialCells(pb);
            SetPatternIndex(0);

        }

        public void InitialPatterns()
        {
            //starting from left up corner, going clockwise
            this.patterns.Add(0, new int[] { 0, 1, 0, 1, 0, 1, 0, 1 });         //von Neumann
            this.patterns.Add(1, new int[] { 1, 1, 1, 1, 1, 1, 1, 1 });         //Moor
            this.patterns.Add(2, new int[] { 0, 1, 1, 1, 0, 1, 1, 1 });         // Hexagonal left
            this.patterns.Add(3, new int[] { 1, 1, 0, 1, 1, 1, 0, 1 });         // Hexagonal right
            this.patterns.Add(4, new int[] { 1, 1, 0, 0, 0, 1, 1, 1 });         // Pentagonal left
            this.patterns.Add(5, new int[] { 0, 1, 1, 1, 1, 1, 0, 0 });         // Pentagonal right
            this.patterns.Add(6, new int[] { 1, 1, 1, 1, 0, 0, 0, 1 });         // Pentagonal top
            this.patterns.Add(7, new int[] { 0, 0, 0, 1, 1, 1, 1, 1 });         // Pentagonal left
        }

        public void SetPatternIndex(int index)
        {
            this.selectedPatternIndex = index;
        }

        public void SetCellType(int i, int j, int type)
        {
            cells[i][j].SetType(type);
        }

        public List<List<Cell>> GetCells()
        {
            return cells;
        }

        public Cell GetCell(int x, int y)
        {
            return this.cells[x][y];
        }

        public uint GetGrains()
        {
            return grains;
        }

        public int GetCellHeight()
        {
            return this.gridCellHeight;
        }

        public int GetCellWidth()
        {
            return this.gridCellWidth;
        }

        public void SetGrains(uint grains)
        {
            this.grains = grains;
        }

        public void IncrementGrain()
        {
            grains++;
        }

        public void DecrementGrains()
        {
            this.grains--;
        }

        internal void InitialCells(PictureBox pb)
        {

            Size size = new Size(Convert.ToInt32(cellSize), Convert.ToInt32(cellSize));

            for (int i = 0; i < gridCellHeight; i++)
            {
                cells.Add(new List<Cell>());
                for (int j = 0; j < gridCellWidth; j++)
                {
                    cells[i].Add(new Cell(new Point(Convert.ToInt32(j * cellSize), Convert.ToInt32(i * cellSize)), size, random));
                }
            }
        }

        public void PrintMesh(PictureBox pb, Graphics g, Bitmap bm)
        {
            Pen blackP = new Pen(Color.Black);

            //vertical
            for (int i = 0; i <= gridCellWidth; i++)
            {
                g.DrawLine(blackP, Convert.ToSingle(i * cellSize) - 1, 0, Convert.ToSingle(i * cellSize) - 1, Convert.ToSingle(gridCellHeight * cellSize));
            }
            //horizontal
            for (int i = 0; i <= gridCellHeight; i++)
            {
                g.DrawLine(blackP, 0, Convert.ToSingle(i * cellSize) - 1, Convert.ToSingle(gridCellWidth * cellSize), Convert.ToSingle(i * cellSize) - 1);
            }

            pb.Image = bm;
        }

        public void PrintGrid(PictureBox pb, Graphics g, Bitmap bm, Boolean displayCentersOfMass)
        {
            g.Clear(Color.White);
            Brush redB = Brushes.Red;

            for (int i = 0; i < gridCellHeight; i++)
            {
                for (int j = 0; j < gridCellWidth; j++)
                {
                    Cell cell = cells[i][j];

                    Color color;
                    if (cell.GetType() == 0)
                        color = Color.White;
                    else
                        color = Form1.GetRandomColorsList()[cell.GetType() - 1];

                    ///////////////////////////////////
                    if (cell.GetRecrystallized())
                        color = Color.Red;
                    ///////////////////////////////////
                    ///
                    SolidBrush brush = new SolidBrush(color);

                    g.FillRectangle(brush, cell.GetPosition().X, cell.GetPosition().Y, cell.GetSize().Width + 1, cell.GetSize().Height + 1);
                    if (displayCentersOfMass)
                    {
                        int r = 4;
                        g.FillEllipse(redB, cell.GetCenterOfMass().X - r / 2, cell.GetCenterOfMass().Y - r / 2, r, r);
                    }
                }
            }
            pb.Image = bm;

        }

        public void PrintEnergy(PictureBox pb, Graphics g, Bitmap bm)
        {
            g.Clear(Color.White);

            for (int i = 0; i < gridCellHeight; i++)
            {
                for (int j = 0; j < gridCellWidth; j++)
                {
                    Cell cell = cells[i][j];

                    Color color;
                    if (cell.GetEnergy() > 0)
                        color = this.energyColors[cell.GetEnergy()];
                    else
                        color = Color.White;

                    SolidBrush brush = new SolidBrush(color);

                    g.FillRectangle(brush, cell.GetPosition().X, cell.GetPosition().Y, cell.GetSize().Width + 1, cell.GetSize().Height + 1);
                }
            }
            pb.Image = bm;
        }

        public List<List<int>> CheckNeighbourhoodAbsorbing(int radius = 0)
        {
            int sizeY = cells.Count;
            int sizeX = cells[0].Count;
            int[] type = new int[] { 0, 0, 0, 0 };

            List<List<int>> neighbours = new List<List<int>>();

            for (int i = 0; i < sizeY; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeX; j++)
                    neighbours[i].Add(0);
            }
            //cells.Count       -> rows counter
            //cells[0].Count    -> columns counter



            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    type = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    int index = selectedPatternIndex;

                    if (selectedPatternIndex == 8)
                        index = random.Next(2, 4);
                    else if (selectedPatternIndex == 9)
                        index = random.Next(4, 8);


                    if (patterns[index][0] == 1 && i + 1 < sizeY && j + 1 < sizeX)
                        type[0] = cells[i + 1][j + 1].GetType();
                    if (patterns[index][1] == 1 && i + 1 < sizeY)
                        type[1] = cells[i + 1][j].GetType();
                    if (patterns[index][2] == 1 && i + 1 < sizeY && j - 1 >= 0)
                        type[2] = cells[i + 1][j - 1].GetType();
                    if (patterns[index][3] == 1 && j - 1 >= 0)
                        type[3] = cells[i][j - 1].GetType();
                    if (patterns[index][4] == 1 && i - 1 >= 0 && j - 1 >= 0)
                        type[4] = cells[i - 1][j - 1].GetType();
                    if (patterns[index][5] == 1 && i - 1 >= 0)
                        type[5] = cells[i - 1][j].GetType();
                    if (patterns[index][6] == 1 && i - 1 >= 0 && j + 1 < sizeX)
                        type[6] = cells[i - 1][j + 1].GetType();
                    if (patterns[index][7] == 1 && j + 1 < sizeX)
                        type[7] = cells[i][j + 1].GetType();

                    neighbours[i][j] = GetMostRepeatedElenent(type);
                }
            }
            return neighbours;
        }

        public List<List<int>> CheckNeighbourhoodPeriodic(int radius = 0)
        {
            int sizeY = cells.Count;
            int sizeX = cells[0].Count;
            int[] type = new int[] { 0, 0, 0, 0 };

            List<List<int>> neighbours = new List<List<int>>();

            for (int i = 0; i < sizeY; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeX; j++)
                    neighbours[i].Add(0);
            }


            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    int index = selectedPatternIndex;
                    type = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    if (selectedPatternIndex == 8)
                    {
                        index = random.Next(2, 4);
                    }
                    else if (selectedPatternIndex == 9)
                    {
                        index = random.Next(4, 8);
                    }
                    if (patterns[index][0] == 1)
                    {
                        if (i + 1 < sizeY && j + 1 < sizeX)
                            type[0] = cells[i + 1][j + 1].GetType();
                        else if (i + 1 < sizeY && j + 1 >= sizeX)
                            type[0] = cells[i + 1][0].GetType();
                        else if (i + 1 >= sizeY && j + 1 < sizeX)
                            type[0] = cells[0][j + 1].GetType();
                        else
                            type[0] = cells[0][0].GetType();
                    }
                    if (patterns[index][1] == 1)
                    {
                        if (i + 1 < sizeY)
                            type[1] = cells[i + 1][j].GetType();
                        else
                            type[1] = cells[0][j].GetType();
                    }
                    if (patterns[index][2] == 1)
                    {
                        if (i + 1 < sizeY && j - 1 >= 0)
                            type[2] = cells[i + 1][j - 1].GetType();
                        else if (i + 1 < sizeY && j - 1 < 0)
                            type[2] = cells[i + 1][sizeX - 1].GetType();
                        else if (i + 1 >= sizeY && j - 1 >= 0)
                            type[2] = cells[0][j - 1].GetType();
                        else
                            type[2] = cells[0][sizeX - 1].GetType();
                    }
                    if (patterns[index][3] == 1)
                    {
                        if (j - 1 >= 0)
                            type[3] = cells[i][j - 1].GetType();
                        else
                            type[3] = cells[i][sizeX - 1].GetType();
                    }
                    if (patterns[index][4] == 1)
                    {
                        if (i - 1 >= 0 && j - 1 >= 0)
                            type[4] = cells[i - 1][j - 1].GetType();
                        else if (i - 1 >= 0 && j - 1 < 0)
                            type[4] = cells[i - 1][sizeX - 1].GetType();
                        else if (i - 1 < 0 && j - 1 >= 0)
                            type[4] = cells[sizeY - 1][j - 1].GetType();
                        else
                            type[4] = cells[sizeY - 1][sizeX - 1].GetType();
                    }
                    if (patterns[index][5] == 1)
                    {
                        if (i - 1 >= 0)
                            type[5] = cells[i - 1][j].GetType();
                        else
                            type[5] = cells[sizeY - 1][j].GetType();
                    }
                    if (patterns[index][6] == 1)
                    {
                        if (i - 1 >= 0 && j + 1 < sizeX)
                            type[6] = cells[i - 1][j + 1].GetType();
                        else if (i - 1 >= 0 && j + 1 >= sizeX)
                            type[6] = cells[i - 1][0].GetType();
                        else if (i - 1 < 0 && j + 1 < sizeX)
                            type[6] = cells[sizeY - 1][j + 1].GetType();
                        else
                            type[6] = cells[sizeY - 1][0].GetType();
                    }
                    if (patterns[index][7] == 1)
                    {
                        if (j + 1 < sizeX)
                            type[7] = cells[i][j + 1].GetType();
                        else
                            type[7] = cells[i][0].GetType();
                    }
                    neighbours[i][j] = GetMostRepeatedElenent(type);
                }
            }
            return neighbours;
        }

        public List<List<int>> CheckNeighbourhoodRadiusAbsorbing(int radius)
        {
            int sizeY = cells.Count;
            int sizeX = cells[0].Count;


            List<List<int>> neighbours = new List<List<int>>();
            for (int i = 0; i < sizeY; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeX; j++)
                    neighbours[i].Add(0);
            }

            for (int i = 0; i < sizeY; i++)
            {
                for (int j = 0; j < sizeX; j++)
                {
                    //Cell cell = cells[j][i];
                    if (cells[j][i].GetType() != 0)
                        continue;
                    List<int> cellNeighbours = new List<int>();

                    for (int k = 0; k < sizeY; k++)
                    {
                        for (int l = 0; l < sizeX; l++)
                        {
                            if (Math.Pow(cells[j][i].GetCenterOfMass().X - cells[l][k].GetCenterOfMass().X, 2) +
                                Math.Pow(cells[j][i].GetCenterOfMass().Y - cells[l][k].GetCenterOfMass().Y, 2) <= radius * radius)
                                cellNeighbours.Add(cells[l][k].GetType());
                        }
                    }
                    neighbours[j][i] = GetMostRepeatedElenent(cellNeighbours);
                }
            }

            return neighbours;
        }

        public List<List<int>> CheckNeighbourhoodRadiusPeriodic(int radius)
        {
            int sizeY = cells.Count;
            int sizeX = cells[0].Count;

            List<List<int>> neighbours = new List<List<int>>();
            for (int i = 0; i < sizeY; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeX; j++)
                    neighbours[i].Add(0);
            }

            for (int i = 0; i < sizeY; i++)
            {
                for (int j = 0; j < sizeX; j++)
                {
                    Cell cell = cells[j][i];
                    List<int> cellNeighbours = new List<int>();
                    for (int k = 0; k < sizeY; k++)
                    {
                        for (int l = 0; l < sizeX; l++)
                        {
                            if (Math.Pow(cell.GetCenterOfMass().X - cells[l][k].GetCenterOfMass().X, 2) +
                                Math.Pow(cell.GetCenterOfMass().Y - cells[l][k].GetCenterOfMass().Y, 2) <= radius * radius)
                                cellNeighbours.Add(cells[l][k].GetType());
                            else if (Math.Pow(cell.GetCenterOfMass().X + (gridCellWidth * cellSize) - cells[l][k].GetCenterOfMass().X, 2) +
                                Math.Pow(cell.GetCenterOfMass().Y - cells[l][k].GetCenterOfMass().Y, 2) <= radius * radius)
                                cellNeighbours.Add(cells[l][k].GetType());
                            else if (Math.Pow(cell.GetCenterOfMass().X - cells[l][k].GetCenterOfMass().X, 2) +
                                Math.Pow(cell.GetCenterOfMass().Y + (gridCellHeight * cellSize) - cells[l][k].GetCenterOfMass().Y, 2) <= radius * radius)
                                cellNeighbours.Add(cells[l][k].GetType());
                            else if (Math.Pow(cell.GetCenterOfMass().X - (gridCellWidth * cellSize) - cells[l][k].GetCenterOfMass().X, 2) +
                                Math.Pow(cell.GetCenterOfMass().Y - cells[l][k].GetCenterOfMass().Y, 2) <= radius * radius)
                                cellNeighbours.Add(cells[l][k].GetType());
                            else if (Math.Pow(cell.GetCenterOfMass().X - cells[l][k].GetCenterOfMass().X, 2) +
                                Math.Pow(cell.GetCenterOfMass().Y - (gridCellHeight * cellSize) - cells[l][k].GetCenterOfMass().Y, 2) <= radius * radius)
                                cellNeighbours.Add(cells[l][k].GetType());

                        }
                    }
                    neighbours[j][i] = GetMostRepeatedElenent(cellNeighbours);
                }
            }

            return neighbours;
        }

        public void CalculateNextGeneration(CheckNeighbourhood checkNeighbourhood, int radius = 0)
        {
            List<List<int>> neighbours;

            neighbours = checkNeighbourhood(radius);

            for (int i = 0; i < gridCellHeight; i++)
            {
                for (int j = 0; j < gridCellWidth; j++)
                {

                    if (neighbours[i][j] > 0 && this.cells[i][j].GetType() == 0)
                    {
                        this.cells[i][j].SetType(neighbours[i][j]);
                        grains++;
                    }
                }
            }
        }

        public int GetMostRepeatedElenent(List<int> list)
        {
            int maxRepeats = 0;
            int mostCommonValue = 0;
            for (int k = 0; k < list.Count; k++)
            {
                int value = list[k];
                int repeats = 0;
                for (int l = 0; l < list.Count; l++)
                    if (list[l] == value && list[l] != 0)
                        repeats++;
                if (repeats > maxRepeats)
                {

                    maxRepeats = repeats;
                    mostCommonValue = value;
                }
                else if (repeats == maxRepeats && random.Next(1) == 1)
                {
                    maxRepeats = repeats;
                    mostCommonValue = value;
                }
            }

            return mostCommonValue;
        }

        public int GetMostRepeatedElenent(int[] array)
        {
            int maxRepeats = 0;
            int mostCommonValue = 0;
            for (int k = 0; k < array.Length; k++)
            {
                int value = array[k];
                int repeats = 0;
                for (int l = 0; l < array.Length; l++)
                    if (array[l] == value && array[l] != 0)
                        repeats++;
                if (repeats > maxRepeats)
                {

                    maxRepeats = repeats;
                    mostCommonValue = value;
                }
                else if (repeats == maxRepeats && random.Next(1) == 1)
                {
                    maxRepeats = repeats;
                    mostCommonValue = value;
                }
            }

            return mostCommonValue;
        }

        public void MonteCarloAbsorbing(double kt)
        {
            int cellsCounter = this.gridCellHeight * this.gridCellWidth;
            int[] indexArray = Enumerable.Range(0, (cellsCounter)).ToArray();

            Shuffle(indexArray, random);

            int index = selectedPatternIndex;
            List<int> neighbours = new List<int>();

            for (int i = 0; i < indexArray.Count(); i++)
            {
                int x = indexArray[i] % this.gridCellWidth;
                int y = indexArray[i] / this.gridCellWidth;
                int j = i / this.gridCellWidth;

                if (selectedPatternIndex == 8)
                    index = random.Next(2, 4);
                else if (selectedPatternIndex == 9)
                    index = random.Next(4, 8);

                if (patterns[index][0] == 1 && y - 1 >= 0 && x - 1 >= 0)
                    neighbours.Add(cells[y - 1][x - 1].GetType());
                if (patterns[index][1] == 1 && y - 1 >= 0)
                    neighbours.Add(cells[y - 1][x].GetType());
                if (patterns[index][2] == 1 && y - 1 >= 0 && x + 1 < cells[j].Count)
                    neighbours.Add(cells[y - 1][x + 1].GetType());
                if (patterns[index][3] == 1 && x + 1 < cells[j].Count)
                    neighbours.Add(cells[y][x + 1].GetType());
                if (patterns[index][4] == 1 && y + 1 < cells.Count && x + 1 < cells[j].Count)
                    neighbours.Add(cells[y + 1][x + 1].GetType());
                if (patterns[index][5] == 1 && y + 1 < cells.Count)
                    neighbours.Add(cells[y + 1][x].GetType());
                if (patterns[index][6] == 1 && y + 1 < cells.Count && x - 1 >= 0)
                    neighbours.Add(cells[y + 1][x - 1].GetType());
                if (patterns[index][7] == 1 && x - 1 >= 0)
                    neighbours.Add(cells[y][x - 1].GetType());


                int energy = CalculateEnergy(this.cells[y][x].GetType(), neighbours);
                int randomType = neighbours[random.Next(0, neighbours.Count)];
                int randomEnergy = CalculateEnergy(randomType, neighbours);


                while (energy < randomEnergy)
                {
                    randomType = neighbours[random.Next(0, neighbours.Count)];
                    randomEnergy = CalculateEnergy(randomType, neighbours);
                    int dE = randomEnergy - energy;

                    //Console.WriteLine(Math.Pow(Math.E, Convert.ToDouble(-dE / kt)));

                    if (random.NextDouble() < Math.Pow(Math.E, Convert.ToDouble(-dE / kt)))
                        break;

                }

                

                //Console.WriteLine("energy: " + energy);
                //Console.WriteLine("randomType: " + randomType);
                //Console.WriteLine("randomEnergy: " + randomEnergy);
                //PrintList(neighbours);

                this.cells[y][x].SetType(randomType);
                this.cells[y][x].SetEnergy(randomEnergy);
                neighbours = new List<int>();
            }
        }

        public void MonteCarloPeriodic(double kt)
        {
            int cellsCounter = this.gridCellHeight * this.gridCellWidth;
            int[] indexArray = Enumerable.Range(0, (cellsCounter)).ToArray();
            int h = this.gridCellHeight - 1;
            int w = this.gridCellWidth - 1;

            Shuffle(indexArray, random);

            int index = selectedPatternIndex;
            List<int> neighbours = new List<int>();

            for (int i = 0; i < indexArray.Count(); i++)
            {
                int x = indexArray[i] % this.gridCellWidth;
                int y = indexArray[i] / this.gridCellWidth;
                int j = i / this.gridCellWidth;

                if (selectedPatternIndex == 8)
                    index = random.Next(2, 4);
                else if (selectedPatternIndex == 9)
                    index = random.Next(4, 8);

                if (patterns[index][0] == 1)
                {
                    if (y - 1 >= 0 && x - 1 >= 0)
                        neighbours.Add(cells[y - 1][x - 1].GetType());
                    else if (y - 1 < 0 && x - 1 >= 0)
                        neighbours.Add(cells[h][x - 1].GetType());
                    else if (y - 1 >= 0 && x - 1 < 0)
                        neighbours.Add(cells[y - 1][w].GetType());
                    else
                        neighbours.Add(cells[h][w].GetType());
                }
                if (patterns[index][1] == 1)
                {
                    if (y - 1 >= 0)
                        neighbours.Add(cells[y - 1][x].GetType());
                    else
                        neighbours.Add(cells[h][x].GetType());
                }
                if (patterns[index][2] == 1)
                {
                    if (y - 1 >= 0 && x + 1 < cells[j].Count)
                        neighbours.Add(cells[y - 1][x + 1].GetType());
                    else if (y - 1 >= 0 && x + 1 >= cells[j].Count)
                        neighbours.Add(cells[y - 1][0].GetType());
                    else if (y - 1 < 0 && x + 1 < cells[j].Count)
                        neighbours.Add(cells[h][x + 1].GetType());
                    else
                        neighbours.Add(cells[h][w].GetType());
                }
                if (patterns[index][3] == 1)
                {
                    if (x + 1 < cells[j].Count)
                        neighbours.Add(cells[y][x + 1].GetType());
                    else
                        neighbours.Add(cells[y][0].GetType());
                }
                if (patterns[index][4] == 1)
                {
                    if (y + 1 < cells.Count && x + 1 < cells[j].Count)
                        neighbours.Add(cells[y + 1][x + 1].GetType());
                    else if (y + 1 < cells.Count && x + 1 >= cells[j].Count)
                        neighbours.Add(cells[y + 1][0].GetType());
                    else if (y + 1 >= cells.Count && x + 1 < cells[j].Count)
                        neighbours.Add(cells[0][x + 1].GetType());
                    else
                        neighbours.Add(cells[0][0].GetType());
                }
                if (patterns[index][5] == 1)
                {
                    if (y + 1 < cells.Count)
                        neighbours.Add(cells[y + 1][x].GetType());
                    else
                        neighbours.Add(cells[0][x].GetType());
                }
                if (patterns[index][6] == 1)
                {
                    if (y + 1 < cells.Count && x - 1 >= 0)
                        neighbours.Add(cells[y + 1][x - 1].GetType());
                    else if (y + 1 < cells.Count && x - 1 < 0)
                        neighbours.Add(cells[y + 1][w].GetType());
                    else if (y + 1 >= cells.Count && x - 1 >= 0)
                        neighbours.Add(cells[0][x - 1].GetType());
                    else
                        neighbours.Add(cells[0][w].GetType());
                }
                if (patterns[index][7] == 1)
                {
                    if (x - 1 >= 0)
                        neighbours.Add(cells[y][x - 1].GetType());
                    else
                        neighbours.Add(cells[y][w].GetType());
                }



                int energy = CalculateEnergy(this.cells[y][x].GetType(), neighbours);
                int randomType = neighbours[random.Next(0, neighbours.Count)];
                int randomEnergy = CalculateEnergy(randomType, neighbours);


                while (energy < randomEnergy)
                {
                    randomType = neighbours[random.Next(0, neighbours.Count)];
                    randomEnergy = CalculateEnergy(randomType, neighbours);
                    int dE = randomEnergy - energy;

                    //if (random.Next(0, 101) > 100 * Math.Pow(Math.E, Convert.ToDouble(-dE / kt)))
                    //    break;
                    if (random.NextDouble() < Math.Pow(Math.E, Convert.ToDouble(-dE / kt)))
                        break;
                }

                //Console.WriteLine("energy: " + energy);
                //Console.WriteLine("randomType: " + randomType);
                //Console.WriteLine("randomEnergy: " + randomEnergy);
                //PrintList(neighbours);

                this.cells[y][x].SetType(randomType);
                this.cells[y][x].SetEnergy(randomEnergy);
                neighbours = new List<int>();

            }
        }

        public void CalculateEnergyForGrid()
        {
            int index = selectedPatternIndex;
            List<int> neighbours = new List<int>();

            for (int i = 0; i < this.gridCellHeight; i++)
            {
                for(int j = 0; j < this.gridCellWidth; j++)
                {
                    if (selectedPatternIndex == 8)
                        index = random.Next(2, 4);
                    else if (selectedPatternIndex == 9)
                        index = random.Next(4, 8);

                    if (patterns[index][0] == 1 && j - 1 >= 0 && i - 1 >= 0)
                        neighbours.Add(cells[j - 1][i - 1].GetType());
                    if (patterns[index][1] == 1 && j - 1 >= 0)
                        neighbours.Add(cells[j - 1][i].GetType());
                    if (patterns[index][2] == 1 && j - 1 >= 0 && i + 1 < cells[j].Count)
                        neighbours.Add(cells[j - 1][i + 1].GetType());
                    if (patterns[index][3] == 1 && i + 1 < cells[j].Count)
                        neighbours.Add(cells[j][i + 1].GetType());
                    if (patterns[index][4] == 1 && j + 1 < cells.Count && i + 1 < cells[j].Count)
                        neighbours.Add(cells[j + 1][i + 1].GetType());
                    if (patterns[index][5] == 1 && j + 1 < cells.Count)
                        neighbours.Add(cells[j + 1][i].GetType());
                    if (patterns[index][6] == 1 && j + 1 < cells.Count && i - 1 >= 0)
                        neighbours.Add(cells[j + 1][i - 1].GetType());
                    if (patterns[index][7] == 1 && i - 1 >= 0)
                        neighbours.Add(cells[j][i - 1].GetType());

                    int energy = CalculateEnergy(this.cells[j][i].GetType(), neighbours);

                    this.cells[j][i].SetEnergy(energy);
                    neighbours = new List<int>();

                }
            }
        }

        public static int CalculateEnergy(int type, List<int> neighbours)
        {
            int energy = 0;
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i] != type)
                    energy++;
            }

            return energy;
        }

        public static void PrintArray(int[] arr)
        {
            Console.Write("Array: ");
            for (int i = 0; i < arr.Length; i++)
                Console.Write(arr[i] + " ");
            Console.WriteLine();
        }

        public static void PrintList(List<int> list)
        {
            Console.Write("List: ");
            for (int i = 0; i < list.Count; i++)
                Console.Write(list[i] + " ");
            Console.WriteLine();
        }

        /// Knuth shuffle
        public static void Shuffle(int[] array, Random random)
        {
            //Random random = new Random();
            int n = array.Count();
            while (n > 1)
            {
                n--;
                int i = random.Next(n + 1);
                int temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }
        }

        public void Recrystallization(double A, double B, double timeStep, int step, double percentageForAll, Random random)
        {
            double ro = A / B + (1 - A / B) * Math.Pow(Math.E, -B * timeStep * step);
            double prevRo = A / B + (1 - A / B) * Math.Pow(Math.E, -B * timeStep * (step - 1));
            double dRo = Math.Abs(ro - prevRo);

            double roPerCell = dRo / (this.gridCellHeight * this.gridCellWidth);
            double roForAllPerCell = roPerCell / percentageForAll;

            AddRecrystallizationDencityPerEveryCell(roForAllPerCell);

            double roRandom = dRo * (100 - percentageForAll);
            int packAmmount = random.Next(1, 101);
            double packSize = roRandom / Convert.ToDouble(packAmmount);

            AddRecrystallizationDencityRandom(packAmmount, packSize, random);
            //Console.WriteLine(ro);

        }

        public void AddRecrystallizationDencityPerEveryCell(double roPerCell)
        {
            for (int i = 0; i < cells.Count; i++)
                for (int j = 0; j < cells[0].Count; j++)
                    cells[i][j].AddDislocationDensity(roPerCell);
        }

        public void AddRecrystallizationDencityRandom(int packAmmount, double packSize, Random random)
        {
            for (int i = 0; i < packAmmount; i++)
            {
                int index = random.Next(0, this.gridCellHeight * this.gridCellWidth);
                int x = index % this.gridCellWidth;
                int y = index / this.gridCellWidth;

                int probability = random.Next(0, 101);

                if (cells[y][x].GetEnergy() > 0)
                {
                    if(probability <= 80)
                        cells[y][x].AddDislocationDensity(packSize);
                }
                else
                {
                    Debug.WriteLine("WORK!!!");
                    if (probability >= 80)
                        cells[y][x].AddDislocationDensity(packSize);
                }
                    
            }

        }

        public void Recovery()
        {
            Boolean[,] recrystallized = new Boolean[cells.Count, cells[0].Count];

            double roC = 4215840142323.42 / (this.gridCellHeight * this.gridCellWidth);

            for (int i = 0; i< cells.Count; i++)
            {
                for(int j = 0; j < cells[0].Count; j++)
                {
                    if(cells[i][j].GetDislocationDensity() > roC && cells[i][j].GetEnergy() > 0)
                    {
                        cells[i][j].SetRecrystallized(true);
                        recrystallized[i, j] = true;
                        cells[i][j].SetDislocationDensity(0);
                    }
                    //else      // deleting seeds from last interation
                    //    cells[i][j].SetRecrystallized(false);
                }
            }

            for (int i = 0; i < cells.Count; i++)
                for (int j = 0; j < cells[0].Count; j++)
                    lastRecrystallized[i, j] = recrystallized[i, j];
        }

        public void IterationOfRecrystallizationAbsorbing()
        {
            int sizeY = cells.Count;
            int sizeX = cells[0].Count;

            Boolean[,] thisIteration = new Boolean[cells.Count, cells[0].Count];
            for (int i = 0; i < cells.Count; i++)
                for (int j = 0; j < cells[0].Count; j++)
                    thisIteration[i, j] = lastRecrystallized[i, j];


            //cells.Count       -> rows counter
            //cells[0].Count    -> columns counter
            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    int index = selectedPatternIndex;

                    if (selectedPatternIndex == 8)
                        index = random.Next(2, 4);
                    else if (selectedPatternIndex == 9)
                        index = random.Next(4, 8);

                    Boolean recrystallizedNeighbour = false;
                    Boolean lowerDencity = true;

                    if (patterns[index][0] == 1 && i + 1 < sizeY && j + 1 < sizeX)
                    {
                        if (this.lastRecrystallized[i + 1, j + 1] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i + 1][j + 1].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][1] == 1 && i + 1 < sizeY)
                                            {
                        if (this.lastRecrystallized[i + 1, j] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i + 1][j].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][2] == 1 && i + 1 < sizeY && j - 1 >= 0)
                    {
                        if (this.lastRecrystallized[i + 1, j - 1] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i + 1][j - 1].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][3] == 1 && j - 1 >= 0)
                    {
                        if (this.lastRecrystallized[i, j - 1] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i][j - 1].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][4] == 1 && i - 1 >= 0 && j - 1 >= 0)
                    {
                        if (this.lastRecrystallized[i - 1, j - 1] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i - 1][j - 1].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][5] == 1 && i - 1 >= 0)
                    {
                        if (this.lastRecrystallized[i - 1, j] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i - 1][j].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][6] == 1 && i - 1 >= 0 && j + 1 < sizeX)
                    {
                        if (this.lastRecrystallized[i - 1, j + 1] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i - 1][j + 1].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }
                    if (patterns[index][7] == 1 && j + 1 < sizeX)
                    {
                        if (this.lastRecrystallized[i, j + 1] == true)
                            recrystallizedNeighbour = true;
                        if (cells[i][j + 1].GetDislocationDensity() > cells[i][j].GetDislocationDensity())
                            lowerDencity = false;
                    }

                    if (recrystallizedNeighbour && lowerDencity)
                    {
                        cells[i][j].SetRecrystallized(true);
                        cells[i][j].SetDislocationDensity(0);
                        thisIteration[i, j] = true;
                    }
                    //else
                        //thisIteration[i, j] = false;
                }
            }
            for (int i = 0; i < cells.Count; i++)
                for (int j = 0; j < cells[0].Count; j++)
                    if(thisIteration[i,j] == false)
                        lastRecrystallized[i, j] = thisIteration[i, j];
        }

        public void PrintRecrystallisation()
        {
            for(int i = 0; i < lastRecrystallized.GetLength(0); i++)
            {
                for (int j = 0; j< lastRecrystallized.GetLength(1); j++)
                {
                    Console.Write(Convert.ToInt32(lastRecrystallized[i, j]) + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

    }
}