// Shapes.cs
namespace CoreModule.Shapes {
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
}
