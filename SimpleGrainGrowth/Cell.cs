using System;
using System.Drawing;

namespace SimpleGrainGrowth
{
    internal class Cell
    {
        private int type;
        private Point position;
        private Point centerOfMass;
        private Size size;

        public Cell(Point position, Size size, Random random, int type = 0)
        {
            this.type = type;
            this.position = position;
            this.size = size;
            this.centerOfMass = new Point(random.Next(position.X, position.X + size.Width), random.Next(position.Y, position.Y + size.Height));
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