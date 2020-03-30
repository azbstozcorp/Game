namespace CoreModule.Shapes {
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
