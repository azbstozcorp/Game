// Collision.cs
namespace CoreModule {
    public static class Collision {
        /// <summary>
        /// Check a given value between two bounds
        /// </summary>
        public static bool Between(int lowerBound, int upperBound, int value, bool inclusive = false) =>
             (lowerBound < value && upperBound < value) ||                  /* Check the value between upper and lower bounds    */
             (inclusive && (value == lowerBound || value == upperBound));   /* If the check is inclusive, return true for bounds */

        /// <summary>
        /// Check if a given point is within a given rectangle
        /// </summary>
        public static bool WithinRect(Point topLeft, Point bottomRight, Point check, bool inclusive = false) =>
            Between(topLeft.X, bottomRight.X, check.X, inclusive) &&   /* Check X coordinate */
            Between(topLeft.Y, bottomRight.Y, check.Y, inclusive);     /* Check Y coordinate */

        /// <summary>
        /// Check if two rectangles overlap
        /// </summary>
        public static bool RectsOverlap(Rect a, Rect b) =>
            a.Left < b.Right && a.Right > b.Left &&
            a.Top < b.Bottom && a.Bottom > b.Top;

        static bool CCW(Point a, Point b, Point c) => (c.Y - a.Y) * (b.X - a.X) > (b.Y - a.Y) * (c.X - a.X);
        /// <summary>
        /// Check if two lines intersect
        /// </summary>
        public static bool LinesIntersect(Line a, Line b) =>
            CCW(a.Start, b.Start, b.End) != CCW(a.End, b.Start, b.End) && CCW(a.Start, a.End, b.Start) != CCW(a.Start, a.End, b.End);

        /// <summary>
        /// Find the intersection point of two lines
        /// </summary>
        public static Point IntersectionOf(Line a, Line b) {
            int a1 = a.End.Y - a.Start.Y;
            int b1 = a.Start.X - a.End.X;
            int c1 = a1 * a.Start.X + b1 * a.Start.Y;

            int a2 = b.End.Y - b.Start.Y;
            int b2 = b.Start.X - b.End.X;
            int c2 = a1 * b.Start.X + b1 * b.Start.Y;

            int delta = a1 * b2 - a2 * b1;
            return delta == 0 ? null : new Point((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }

        static int Min(int a, int b) => a > b ? b : a;
        static int Max(int a, int b) => a > b ? a : b;
    }
}
