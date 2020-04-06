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
        public Point Center => new Point(Left + Width / 2, Top + Height / 2);

        public Line TLTR => new Line(TopLeft, TopRight);
        public Line TLBL => new Line(TopLeft, BottomLeft);
        public Line BRTR => new Line(BottomRight, TopRight);
        public Line BRBL => new Line(BottomRight, BottomLeft);

        public int Width => Right - Left;
        public int Height => Bottom - Top;

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

        public void Move(int amountX, int amountY) { Left += amountX; Right += amountX; Top += amountY; Bottom += amountY; }

        public Rect Copy => new Rect(TopLeft, BottomRight);

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

    [System.Serializable]
    public class RectF {
        public PointF TopLeft => new PointF(Left, Top);
        public PointF TopRight => new PointF(Right, Top);
        public PointF BottomLeft => new PointF(Left, Bottom);
        public PointF BottomRight => new PointF(Right, Bottom);
        public PointF Center => new PointF(Left + Width / 2f, Top + Height / 2f);

        public float Width => Right - Left;
        public float Height => Bottom - Top;

        public float Top { get; set; }
        public float Left { get; set; }
        public float Bottom { get; set; }
        public float Right { get; set; }

        public RectF() { }
        public RectF(PointF topLeft, PointF bottomRight) : this() {
            Top = topLeft.Y;
            Left = topLeft.X;
            Bottom = bottomRight.Y;
            Right = bottomRight.X;
        }
        public RectF(PointF topLeft, float width, float height) : this(topLeft, (topLeft.X + width, topLeft.Y + height)) { }
        public RectF(float x1, float y1, float x2, float y2) : this((x1, y1), (x2, y2)) { }

        public void Move(float amountX, float amountY) { Left += amountX; Right += amountX; Top += amountY; Bottom += amountY; }

        public RectF Copy => new RectF(TopLeft, BottomRight);

        public static bool operator ==(RectF a, RectF b) =>
            a.Equals(b);
        public static bool operator !=(RectF a, RectF b) => !(a == b);

        public static implicit operator RectF(Rect r) => new RectF(r.TopLeft, r.BottomRight);
        public static implicit operator Rect(RectF r) => new Rect(r.TopLeft, r.BottomRight);

        public override string ToString() => $"[{TopLeft},{TopRight},{BottomLeft},{BottomRight}]";

        public override bool Equals(object obj) {
            if (obj is RectF RectF) return this.GetHashCode() == RectF.GetHashCode();
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
