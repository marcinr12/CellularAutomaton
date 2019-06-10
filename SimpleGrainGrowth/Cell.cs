using System;
using System.Drawing;

namespace SimpleGrainGrowth
{
    internal class Cell
    {
        private int type;
        private int energy;
        private Point position;
        private Point centerOfMass;
        private Size size;
        private Boolean recrystallized;
        private double dislocationDensity;

        public Cell(Point position, Size size, Random random, double dislocationDensity = 0, int type = 0, int energy = 0, Boolean recrystallized = false)
        {
            this.position = position;
            this.size = size;
            this.centerOfMass = new Point(random.Next(position.X, position.X + size.Width), random.Next(position.Y, position.Y + size.Height));
            this.dislocationDensity = dislocationDensity;
            this.type = type;
            this.energy = energy;
            this.recrystallized = recrystallized;
        }
        public Cell(Point position, Size size, Random random, int type, Point centerOfMass)
        {
            this.type = type;
            this.position = position;
            this.size = size;
            this.centerOfMass = centerOfMass;
        }

        public Boolean GetRecrystallized()
        {
            return this.recrystallized;
        }

        public void SetRecrystallized(Boolean recrystallized)
        {
            this.recrystallized = recrystallized;
        }

        public void SetDislocationDensity(double dislocationDensity)
        {
            this.dislocationDensity = dislocationDensity;
        }

        public void AddDislocationDensity(double dislocationDensity)
        {
            this.dislocationDensity += dislocationDensity;
        }

        public double GetDislocationDensity()
        {
            return this.dislocationDensity;
        }

        public void SetEnergy(int energy)
        {
            this.energy = energy;
        }

        public int GetEnergy()
        {
            return this.energy;
        }

        public Cell(ref Cell cell)
        {
            this.type = cell.type;
            this.position = cell.position;
            this.size = cell.size;
            this.centerOfMass = cell.centerOfMass;
        }

        public new int GetType()
        {
            return this.type;
        }

        public Point GetCenterOfMass()
        {
            return this.centerOfMass;
        }

        public Point GetPosition()
        {
            return this.position;
        }

        public Size GetSize()
        {
            return this.size;
        }

        public void SetType(int type)
        {
            this.type = type;
        }

        public void SetPosition(Point position)
        {
            this.position = position;
        }

        public void SetSize(Size size)
        {
            this.size = size;
        }
    }
}