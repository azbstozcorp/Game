// Shapes.cs
namespace CoreModule.Shapes {
    /// <summary>
    /// Point in 2d space
    /// </summary>
    [System.Serializable]
    public class Point {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() { }
        public Point(int x, int y) { X = x; Y = y; }
        public Point(PixelEngine.Point from) { X = from.X; Y = from.Y; }
        public Point((int x, int y) from) { X = from.x; Y = from.y; }

        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !(a == b);

        public static implicit operator Point(PixelEngine.Point p) => new Point(p);
        public static implicit operator Point((int x, int y) p) => new Point(p);
        public static implicit operator PixelEngine.Point(Point p) => new PixelEngine.Point(p.X, p.Y);
        public static implicit operator (int x, int y) (Point p) => (p.X, p.Y);

        public static Point operator +(Point a, int b) => new Point(a.X + b, a.Y + b);
        public static Point operator -(Point a, int b) => a + -b;
        public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);

        public override string ToString() => $"({X},{Y})";

        public override int GetHashCode() {
            int hash = 17;
            hash *= 23 + X.GetHashCode();
            hash *= 23 + Y.GetHashCode();
            return hash;
        }
        public override bool Equals(object obj) {
            Point other = obj as Point;
            if (other != null)   /* If the object is a point, check for equality ---------- */
                return other.X == X && other.Y == Y;
            return false;        /* If the object isn't a point, there's no way it is equal */
        }
    }
}
