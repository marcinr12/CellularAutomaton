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
        private Random random = new Random();
        private uint grains = 0;



        private Dictionary<int, int[]> patterns = new Dictionary<int, int[]>();
        int selestedPatternIndex = 0;

        public Grid(PictureBox pb, int gridCellHeight, int gridCellWidth)
        {
            this.gridCellHeight = gridCellHeight;
            this.gridCellWidth = gridCellWidth;

            double cellSizeWidth = Convert.ToDouble(pb.Width / (gridCellWidth * 1.0));
            double cellSizeHeight = Convert.ToDouble(pb.Height / (gridCellHeight * 1.0));
            this.cellSize = (cellSizeHeight < cellSizeWidth) ? cellSizeHeight : cellSizeWidth;


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
            this.selestedPatternIndex = index;
        }

        public void SetCellType(int i, int j, int type)
        {
            cells[i][j].SetType(type);
        }

        public List<List<Cell>> GetCells()
        {
            return cells;
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
                //g.DrawLine(blackP, Convert.ToSingle(i * sizeCellWidth), 0, Convert.ToSingle(i * sizeCellWidth), pb.Height);
                g.DrawLine(blackP, Convert.ToSingle(i * cellSize) - 1, 0, Convert.ToSingle(i * cellSize) - 1, Convert.ToSingle(gridCellHeight * cellSize));

            }
            //horizontal
            for (int i = 0; i <= gridCellHeight; i++)
            {
                //g.DrawLine(blackP, 0, Convert.ToSingle(i * sizeCellHeight), pb.Width, Convert.ToSingle(i * sizeCellHeight));
                g.DrawLine(blackP, 0, Convert.ToSingle(i * cellSize) - 1, Convert.ToSingle(gridCellWidth * cellSize), Convert.ToSingle(i * cellSize) - 1);
            }

            pb.Image = bm;
        }

        public void PrintGrid(PictureBox pb, Graphics g, Bitmap bm, Boolean displayCentersOfMass)
        {
            g.Clear(Color.White);
            Brush redB = Brushes.Red;
            Brush greenB = Brushes.Green;

            for (int i = 0; i < gridCellHeight; i++)
            {
                for (int j = 0; j < gridCellWidth; j++)
                {
                    Cell cell = cells[i][j];
                    int type = cell.GetType();

                    Color color;
                    if (cell.GetType() == 0)
                        color = Color.White;
                    else
                        color = Form1.GetRandomColorsList()[cell.GetType() - 1];

                    SolidBrush brush = new SolidBrush(color);

                    g.FillRectangle(brush, cell.GetPosition().X, cell.GetPosition().Y, cell.GetSize().Width + 1, cell.GetSize().Height + 1);
                    if(displayCentersOfMass)
                    {
                        int r = 4;
                        g.FillEllipse(redB, cell.GetCenterOfMass().X - r / 2, cell.GetCenterOfMass().Y - r / 2, r, r);
                    }
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
                    //type = new int[] { 0, 0, 0, 0 };
                    //if (j - 1 >= 0)
                    //{
                    //    type[0] = cells[i][j - 1].GetType();
                    //}
                    //if (i + 1 < sizeY)
                    //{
                    //    type[1] = cells[i + 1][j].GetType();
                    //}
                    //if (j + 1 < sizeX)
                    //{
                    //    type[2] = cells[i][j + 1].GetType();
                    //}
                    //if (i - 1 >= 0)
                    //{
                    //    type[3] = cells[i - 1][j].GetType();
                    //}
                    
                    type = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    int index = selestedPatternIndex;
                    if (selestedPatternIndex == 8)
                    {
                        index = random.Next(2, 4);
                    }
                    else if (selestedPatternIndex == 9)
                    {
                        index = random.Next(4, 8);
                    }

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


                    int maxRepeats = 0;
                    int mostCommonValue = 0;
                    for (int k = 0; k < type.Length; k++)
                    {
                        int value = type[k];
                        int repeats = 0;
                        for (int l = 0; l < type.Length; l++)
                            if (type[l] == value && type[l] != 0)
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
                    neighbours[i][j] = mostCommonValue;
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
                    //type = new int[] { 0, 0, 0, 0 };
                    //if (j - 1 >= 0)
                    //    type[0] = cells[i][j - 1].GetType();
                    //else
                    //    type[0] = cells[i][cells[i].Count - 1].GetType();


                    //if (i + 1 < sizeY)

                    //    type[1] = cells[i + 1][j].GetType();
                    //else
                    //    type[1] = cells[0][j].GetType();



                    //if (j + 1 < sizeX)
                    //    type[2] = cells[i][j + 1].GetType();
                    //else
                    //    type[2] = cells[i][0].GetType();



                    //if (i - 1 >= 0)
                    //    type[3] = cells[i - 1][j].GetType();
                    //else
                    //    type[3] = cells[cells.Count - 1][j].GetType();


                    int index = selestedPatternIndex;
                    type = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    if (selestedPatternIndex == 8)
                    {
                        index = random.Next(2, 4);
                    }
                    else if (selestedPatternIndex == 9)
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


                    int maxRepeats = 0;
                    int mostCommonValue = 0;
                    for (int k = 0; k < type.Length; k++)
                    {
                        int value = type[k];
                        int repeats = 0;
                        for (int l = 0; l < type.Length; l++)
                            if (type[l] == value && type[l] != 0)
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
                    neighbours[i][j] = mostCommonValue;
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
                for(int j = 0; j < sizeX; j++)
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
                        }
                    }

                    //check most repeated element
                    int maxRepeats = 0;
                    int mostCommonValue = 0;
                    for (int k = 0; k < cellNeighbours.Count; k++)
                    {
                        int value = cellNeighbours[k];
                        int repeats = 0;
                        for (int l = 0; l < cellNeighbours.Count; l++)
                            if (cellNeighbours[l] == value && cellNeighbours[l] != 0)
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
                    neighbours[j][i] = mostCommonValue;
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

    }
}