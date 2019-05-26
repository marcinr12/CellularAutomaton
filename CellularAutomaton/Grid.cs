using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CellularAutomaton
{
    internal class Grid
    {
        private List<List<Cell>> cells;
        private List<List<bool>> previousState;
        private Size gridSize;

        public Grid() { }

        public void SetGridSize(int width, int height)
        {
            this.gridSize.Width = width;
            this.gridSize.Height = height;

            // creating empty lists
            this.cells = new List<List<Cell>>();
            this.previousState = new List<List<bool>>();
        }

        public List<List<Cell>> GetCells()
        {
            return cells;
        }

        public List<List<bool>> GetPreviousState()
        {
            return previousState;
        }

        public void InitialCells(PictureBox pb)
        {
            double sizeCellWidth = Convert.ToDouble(pb.Width / (gridSize.Width * 1.0));
            double sizeCellHeight = Convert.ToDouble(pb.Height / (gridSize.Height * 1.0));
            Size size = new Size(Convert.ToInt32(sizeCellWidth), Convert.ToInt32(sizeCellHeight));

            for (int i = 0; i < gridSize.Height; i++)
            {
                cells.Add(new List<Cell>());
                previousState.Add(new List<bool>());
                for (int j = 0; j < gridSize.Width; j++)
                {
                    cells[i].Add(new Cell(new Point(Convert.ToInt32(j * sizeCellWidth), Convert.ToInt32(i * sizeCellHeight)), size));
                    previousState[i].Add(false);
                }
            }
        }

        public void printMesh(PictureBox pb, Graphics g, Bitmap bm)
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

            printMesh(pb, g, bm);
        }


        public bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber - 1)) != 0;
        }

        public void CalculateRule(int row, byte rule)
        {
            for (int i = 0; i < cells[row].Count; i++)
            {
                int prev = i - 1;
                int next = i + 1;
                if (prev < 0)
                    prev = cells[row].Count - 1;
                if (next == cells[row].Count)
                    next = 0;

                if (previousState[row][prev] == true && previousState[row][i] == true && previousState[row][next] == true)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 8));
                    previousState[row + 1][i] = GetBit(rule, 8);
                }
                if (previousState[row][prev] == true && previousState[row][i] == true && previousState[row][next] == false)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 7));
                    previousState[row + 1][i] = GetBit(rule, 7);
                }
                if (previousState[row][prev] == true && previousState[row][i] == false && previousState[row][next] == true)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 6));
                    previousState[row + 1][i] = GetBit(rule, 6);
                }
                if (previousState[row][prev] == true && previousState[row][i] == false && previousState[row][next] == false)
                {
                    cells[row + 1][i].state = GetBit(rule, 5);
                    previousState[row + 1][i] = GetBit(rule, 5);
                }
                if (previousState[row][prev] == false && previousState[row][i] == true && previousState[row][next] == true)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 4));
                    previousState[row + 1][i] = GetBit(rule, 4);
                }
                if (previousState[row][prev] == false && previousState[row][i] == true && previousState[row][next] == false)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 3));
                    previousState[row + 1][i] = GetBit(rule, 3);
                }
                if (previousState[row][prev] == false && previousState[row][i] == false && previousState[row][next] == true)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 2));
                    previousState[row + 1][i] = GetBit(rule, 2);
                }
                if (previousState[row][prev] == false && previousState[row][i] == false && previousState[row][next] == false)
                {
                    cells[row + 1][i].SetState(GetBit(rule, 1));
                    previousState[row + 1][i] = GetBit(rule, 1);
                }
            }
            for (int i = 0; i < cells[row].Count; i++)
                previousState[row][i] = cells[row][i].GetState();
        }

    }
}