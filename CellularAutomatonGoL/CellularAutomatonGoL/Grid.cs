using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CellularAutomatonGoL
{
    public delegate List<List<int>> CheckNeighbouthood();

    internal class Grid
    {
        private List<List<Cell>> cells;
        private List<List<int>> previousState;
        private Size gridSize;
        Random random = new Random();


        public Grid() { }

        public void SetGridSize(int width, int height)
        {
            this.gridSize.Width = width;
            this.gridSize.Height = height;

            // creating empty lists
            this.cells = new List<List<Cell>>();
            this.previousState = new List<List<int>>();
        }

        public List<List<Cell>> GetCells()
        {
            return cells;
        }

        public List<List<int>> GetPreviousState()
        {
            return previousState;
        }

        public void SetPreviousStatus(int previousState, int x, int y)
        {
            this.previousState[x][y] = previousState;
        }

        internal void InitialCells(PictureBox pb)
        {
            double sizeCellWidth = Convert.ToDouble(pb.Width / (gridSize.Width * 1.0));
            double sizeCellHeight = Convert.ToDouble(pb.Height / (gridSize.Height * 1.0));
            Size size = new Size(Convert.ToInt32(sizeCellWidth), Convert.ToInt32(sizeCellHeight));

            for (int i = 0; i < gridSize.Height; i++)
            {
                cells.Add(new List<Cell>());
                previousState.Add(new List<int>());
                for (int j = 0; j < gridSize.Width; j++)
                {
                    cells[i].Add(new Cell(new Point(Convert.ToInt32(j * sizeCellWidth), Convert.ToInt32(i * sizeCellHeight)), size));
                    previousState[i].Add(-1);
                }
            }
        }

        public void PrintMesh(PictureBox pb, Graphics g, Bitmap bm)
        {
            double sizeCellWidth = Convert.ToDouble(pb.Width / (gridSize.Width * 1.0));
            double sizeCellHeight = Convert.ToDouble(pb.Height / (gridSize.Height * 1.0));
            Pen blackP = new Pen(Color.Black);

            for (int i = 0; i < gridSize.Width; i++)
            {
                g.DrawLine(blackP, Convert.ToSingle(i * sizeCellWidth), 0, Convert.ToSingle(i * sizeCellWidth), pb.Height);
                g.DrawLine(blackP, Convert.ToSingle(i * sizeCellWidth) - 1, 0, Convert.ToSingle(i * sizeCellWidth) - 1, pb.Height);

            }
            for (int i = 0; i < gridSize.Height; i++)
            {
                g.DrawLine(blackP, 0, Convert.ToSingle(i * sizeCellHeight), pb.Width, Convert.ToSingle(i * sizeCellHeight));
                g.DrawLine(blackP, 0, Convert.ToSingle(i * sizeCellHeight) - 1, pb.Width, Convert.ToSingle(i * sizeCellHeight) - 1);
            }

            pb.Image = bm;
        }

        public void PrintGrid(PictureBox pb, Graphics g, Bitmap bm)
        {
            g.Clear(Color.White);
            Brush redB = Brushes.Red;
            Brush greenB = Brushes.Green;
            Pen blackP = new Pen(Color.Black);

            for (int i = 0; i < gridSize.Height; i++)
            {
                for (int j = 0; j < gridSize.Width; j++)
                {
                    Cell cell = cells[i][j];
                    if (cell.GetState() == true)
                    {
                        g.FillRectangle(greenB, cell.GetPosition().X, cell.GetPosition().Y, cell.GetSize().Width, cell.GetSize().Height);
                    }
                    else
                    {
                        g.FillRectangle(redB, cell.GetPosition().X, cell.GetPosition().Y, cell.GetSize().Width, cell.GetSize().Height);
                    }
                }
            }

            PrintMesh(pb, g, bm);
        }

        public void LoadPattern(bool [,] pattern, int dx, int dy)
        {
            dx = dx % (cells.Count + pattern.GetLength(0));
            dy = dy % (cells[0].Count + pattern.GetLength(1));

            for(int i = 0; i < pattern.GetLength(0); i++)
            {
                for(int j = 0; j < pattern.GetLength(1); j++)
                {
                    cells[i + dx][j + dy].SetState(pattern[i, j]);
                    previousState[i + dx][j + dy] = Convert.ToInt32(!pattern[i, j]);
                }
            }
        }

        public List<List<int>> CheckNeighbouthoodBothContition()
        {
            int sizeX = cells.Count;
            int sizeY = cells[0].Count;

            List<List<int>> neighbours = new List<List<int>>();
            
            for (int i = 0; i < sizeX; i++)
            {
                neighbours.Add(new List<int>());
                for(int j = 0; j < sizeY; j++)
                    neighbours[i].Add(0); 
            }


            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (cells[i][j].GetState())
                    {
                        neighbours[(i - 1 + sizeX) % sizeX][(j - 1 + sizeY) % sizeY]++;
                        neighbours[i][(j - 1 + sizeY) % sizeY]++;
                        neighbours[(i + 1 + sizeX) % sizeX][(j - 1 + sizeY) % sizeY]++;

                        neighbours[(i - 1 + sizeX) % sizeX][j]++;
                        neighbours[(i + 1 + sizeX) % sizeX][j]++;

                        neighbours[(i - 1 + sizeX) % sizeX][(j + 1 + sizeY) % sizeY]++;
                        neighbours[i][(j + 1 + sizeY) % sizeY]++;
                        neighbours[(i + 1 + sizeX) % sizeX][(j + 1 + sizeY) % sizeY]++;
                    }
                }
            }

            return neighbours;
        }

        public List<List<int>> CheckNeighbouthoodHorizontalContition()
        {
            int sizeX = cells.Count;
            int sizeY = cells[0].Count;

            List<List<int>> neighbours = new List<List<int>>();

            for (int i = 0; i < sizeX; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeY; j++)
                    neighbours[i].Add(0);
            }


            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (cells[i][j].GetState())
                    {
                        neighbours[(i - 1 + sizeX) % sizeX][j]++;
                        neighbours[(i + 1 + sizeX) % sizeX][j]++;

                        if (j - 1 >= 0)
                        {
                            neighbours[(i - 1 + sizeX) % sizeX][j - 1]++;
                            neighbours[i][j - 1]++;
                            neighbours[(i + 1 + sizeX) % sizeX][j - 1]++;
                        }
                        if (j + 1 < sizeY)
                        {
                            neighbours[(i - 1 + sizeX) % sizeX][j + 1]++;
                            neighbours[i][j + 1]++;
                            neighbours[(i + 1 + sizeX) % sizeX][j + 1]++;
                        }
                    }
                }
            }

            return neighbours;
        }

        public List<List<int>> CheckNeighbouthoodVerticalContition()
        {
            int sizeX = cells.Count;
            int sizeY = cells[0].Count;

            List<List<int>> neighbours = new List<List<int>>();

            for (int i = 0; i < sizeX; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeY; j++)
                    neighbours[i].Add(0);
            }


            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (cells[i][j].GetState())
                    {
                        neighbours[i][(j - 1 + sizeY) % sizeY]++;
                        neighbours[i][(j + 1 + sizeY) % sizeY]++;

                        if (i - 1 >= 0)
                        {
                            neighbours[i - 1][(j - 1 + sizeY) % sizeY]++;
                            neighbours[i - 1][j]++;
                            neighbours[i - 1][(j + 1 + sizeY) % sizeY]++;
                        }
                        if (i + 1 < sizeX)
                        {
                            neighbours[i + 1][(j - 1 + sizeY) % sizeY]++;
                            neighbours[i + 1][j]++;
                            neighbours[i + 1][(j + 1 + sizeY) % sizeY]++;
                        }
                    }
                }
            }

            return neighbours;
        }

        public List<List<int>> CheckNeighbouthoodNonContition()
        {
            int sizeX = cells.Count;
            int sizeY = cells[0].Count;

            List<List<int>> neighbours = new List<List<int>>();

            for (int i = 0; i < sizeX; i++)
            {
                neighbours.Add(new List<int>());
                for (int j = 0; j < sizeY; j++)
                    neighbours[i].Add(0);
            }


            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = 0; j < cells[i].Count; j++)
                {
                    if (cells[i][j].GetState())
                    {
                        if (i - 1 >= 0)
                        {
                            if (j - 1 >= 0)
                            {
                                neighbours[i - 1][j - 1]++;
                            }
                            if (j + 1 < sizeY)
                            {
                                neighbours[i - 1][j + 1]++;
                            }
                            neighbours[i - 1][j]++;
                        }
                        if (i + 1 < sizeX)
                        {
                            if (j - 1 >= 0)
                            {
                                neighbours[i + 1][j - 1]++;
                            }
                            if (j + 1 < sizeY)
                            {
                                neighbours[i + 1][j + 1]++;
                            }
                            neighbours[i + 1][j]++;
                        }

                        if (j - 1 >= 0)
                        {
                            neighbours[i][j - 1]++;
                        }
                        if (j + 1 < sizeY)
                        {
                            neighbours[i][j + 1]++;
                        }
                    }
                }
            }

            return neighbours;
        }

        public void CalculateNextGeneration(CheckNeighbouthood checkNeighbouthood)
        {
            int sizeX = cells.Count;
            int sizeY = cells[0].Count;
            List<List<int>> neighbours;


            neighbours = checkNeighbouthood();

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (neighbours[i][j] == 3)
                        this.cells[i][j].SetState(true);
                    else if (cells[i][j].GetState() == true && neighbours[i][j] == 2)
                        this.cells[i][j].SetState(true);
                    else
                        this.cells[i][j].SetState(false);


                    if (neighbours[i][j] == 3)
                    {
                        cells[i][j].SetState(true);
                    }
                    else if (cells[i][j].GetState() == true && neighbours[i][j] == 2)
                    {
                        cells[i][j].SetState(true);
                    }
                    else
                    {
                        cells[i][j].SetState(false);
                    }
                }
            }

        }

        public void GenerateRandomCellsState()
        {
            for (int i = 0; i < cells.Count; i++)
                for (int j = 0; j < cells[i].Count; j++)
                    cells[i][j].SetState(Convert.ToBoolean(random.Next(2)));
        }
    }
}