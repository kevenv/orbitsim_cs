
namespace OrbitSim
{
    /// <summary>
    /// 2 integers container.
    /// </summary>
    public class Point2D
    {
        public int x;
        public int y;

        public Point2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "Point2D: " + x + "," + y;
        }
    }
}
