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

        public override string ToString() => $"({X},{Y})";

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
    }

    /// <summary>
    /// Rectangle in 2d space
    /// </summary>
    [System.Serializable]
    public class Rect {
        public Point TopLeft {
            get => topLeft; set {
                topLeft = value;
                topRight.Y = value.Y;
                bottomRight.X = value.X;
                MaintainStructure();
            }
        }
        public Point TopRight {
            get => topRight; set {
                topRight = value;
                topLeft.Y = value.Y;
                bottomRight.X = value.X;
                MaintainStructure();
            }
        }
        public Point BottomLeft {
            get => bottomLeft; set {
                bottomLeft = value;
                bottomRight.Y = value.Y;
                topLeft.X = value.X;
                MaintainStructure();
            }
        }
        public Point BottomRight {
            get => bottomRight; set {
                bottomRight = value;
                bottomLeft.Y = value.Y;
                topRight.X = value.X;
                MaintainStructure();
            }
        }

        public int Top => TopLeft.Y;
        public int Left => TopLeft.X;
        public int Bottom => BottomRight.Y;
        public int Right => BottomRight.X;

        public Point topLeft, topRight, bottomLeft, bottomRight;

        public Rect() {
            topLeft = new Point();
            topRight = new Point();
            bottomLeft = new Point();
            bottomRight = new Point();
        }
        public Rect(Point topLeft, Point bottomRight) : this() {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
        public Rect(Point topLeft, int width, int height) : this(topLeft, (topLeft.X + width, topLeft.Y + height)) { }
        public Rect(int x1, int y1, int x2, int y2) : this((x1, y1), (x2, y2)) { }

        void MaintainStructure() {
            if (TopLeft.X > TopRight.X) (TopLeft, TopRight) = (TopRight, TopLeft);
            if (BottomRight.Y < TopRight.Y) (BottomRight, TopRight) = (TopRight, BottomRight);
        }

        public static bool operator ==(Rect a, Rect b) =>
            a.TopLeft == b.TopLeft &&
            a.TopRight == b.TopRight &&
            a.BottomLeft == b.BottomLeft &&
            a.BottomRight == b.BottomRight;
        public static bool operator !=(Rect a, Rect b) => !(a == b);

        public override string ToString() => $"[{TopLeft},{TopRight},{BottomLeft},{BottomRight}]";
    }
}
