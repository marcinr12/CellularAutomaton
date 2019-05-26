using System.Drawing;

namespace CellularAutomaton
{
    internal class Cell
    {
        public bool state;
        Point position;
        Size size;

        public Cell(Point position, Size size, bool state = false)
        {
            this.state = state;
            this.position = position;
            this.size = size;
        }

        public bool GetState()
        {
            return this.state;
        }

        public Point GetPosition()
        {
            return this.position;
        }

        public Size GetSize()
        {
            return this.size;
        }

        public void SetState(bool state)
        {
            this.state = state;
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