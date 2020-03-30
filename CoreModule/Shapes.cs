// Shapes.cs
namespace CoreModule {
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

    /// <summary>
    /// Line in 2d space
    /// </summary>
    [System.Serializable]
    public class Line {
        public Point Start {
            get => a; set {
                a = value;
            }
        }
        public Point End {
            get => b; set {
                b = value;
            }
        }
        Point a, b;

        public Line() {
            a = new Point();
            b = new Point();
        }
        public Line(Point a, Point b) : this() { Start = a; End = b; }

        public static bool operator ==(Line a, Line b) => a.Start == b.Start && a.End == b.End;
        public static bool operator !=(Line a, Line b) => !(a == b);

        public override int GetHashCode() {
            int hash = 17;
            hash *= 23 + a.GetHashCode();
            hash *= 23 + b.GetHashCode();
            return hash;
        }
        public override bool Equals(object obj) {
            if (obj is Line line) return this == line;
            return false;
        }
    }

    /// <summary>
    /// Rectangle in 2d space
    /// </summary>
    [System.Serializable]
    public class Rect {
        public Point TopLeft => new Point(Left, Top);
        public Point TopRight => new Point(Right, Top);
        public Point BottomLeft => new Point(Left, Bottom);
        public Point BottomRight => new Point(Right, Bottom);

        public int Top { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Right { get; set; }

        public Rect() { }
        public Rect(Point topLeft, Point bottomRight) : this() {
            Top = topLeft.Y;
            Left = topLeft.X;
            Bottom = bottomRight.Y;
            Right = bottomRight.X;
        }
        public Rect(Point topLeft, int width, int height) : this(topLeft, (topLeft.X + width, topLeft.Y + height)) { }
        public Rect(int x1, int y1, int x2, int y2) : this((x1, y1), (x2, y2)) { }

        public static bool operator ==(Rect a, Rect b) =>
            a.Equals(b);
        public static bool operator !=(Rect a, Rect b) => !(a == b);

        public override string ToString() => $"[{TopLeft},{TopRight},{BottomLeft},{BottomRight}]";

        public override bool Equals(object obj) {
            if (obj is Rect rect) return this.GetHashCode() == rect.GetHashCode();
            return false;
        }
        public override int GetHashCode() {
            int hash = 17;
            hash *= 23 + Top.GetHashCode();
            hash *= 23 + Left.GetHashCode();
            hash *= 23 + Bottom.GetHashCode();
            hash *= 23 + Right.GetHashCode();
            return hash;
        }
    }
}
