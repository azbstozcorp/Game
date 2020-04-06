// Collision.cs
using CoreModule.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreModule {
    public static class Collision {
        /// <summary>
        /// Check a given value between two bounds
        /// </summary>
        public static bool Between(int lowerBound, int upperBound, int value, bool inclusive = false) =>
             (lowerBound < value && upperBound > value) ||                  /* Check the value between upper and lower bounds    */
             (inclusive && (value == lowerBound || value == upperBound));   /* If the check is inclusive, return true for bounds */

        /// <summary>
        /// Check if a given point is within a given rectangle
        /// </summary>
        public static bool WithinRect(Point topLeft, Point bottomRight, Point check, bool inclusive = false) =>
            Between(topLeft.X, bottomRight.X, check.X, inclusive) &&   /* Check X coordinate */
            Between(topLeft.Y, bottomRight.Y, check.Y, inclusive);     /* Check Y coordinate */
        public static bool WithinRect(Rect bounds, Point check, bool inclusive = false) =>
            WithinRect(bounds.TopLeft, bounds.BottomRight, check, inclusive);

        /// <summary>
        /// Check if two rectangles overlap
        /// </summary>
        public static bool RectsOverlap(Rect a, Rect b) =>
            a.Left < b.Right && a.Right > b.Left &&
            a.Top < b.Bottom && a.Bottom > b.Top;

        public static bool LineOverlapsRect(Line line, Rect rect) {
            bool left = LinesIntersect(line, rect.TLBL);
            bool right = LinesIntersect(line, rect.BRTR);
            bool top = LinesIntersect(line, rect.TLTR);
            bool bottom = LinesIntersect(line, rect.BRBL);

            return left || right || top || bottom;
        }

        static bool CCW(Point a, Point b, Point c) => (c.Y - a.Y) * (b.X - a.X) > (b.Y - a.Y) * (c.X - a.X);
        /// <summary>
        /// Check if two lines intersect
        /// </summary>
        public static bool LinesIntersect(Line a, Line b) =>
            CCW(a.Start, b.Start, b.End) != CCW(a.End, b.Start, b.End) && CCW(a.Start, a.End, b.Start) != CCW(a.Start, a.End, b.End);

        public static Point Closest(Point from, Point[] testing) {
            int pow2(int n) => n * n;
            int dist2(int x1, int y1, int x2, int y2) => pow2(x2 - x1) + pow2(y2 - y1);

            Point closest = testing[0];
            for (int i = 0; i < testing.Length; i++)
                if (dist2(from.X, from.Y, testing[i].X, testing[i].Y) < dist2(from.X, from.Y, closest.X, closest.Y))
                    closest = testing[i];
            return closest;
        }

        /// <summary>
        /// Find the intersection point of two lines
        /// </summary>
        public static Point IntersectionOf(Line a, Line b) {
            float p0_x = a.Start.X, p0_y = a.Start.Y, p1_x = a.End.X, p1_y = a.End.Y, p2_x = b.Start.X, p2_y = b.Start.Y, p3_x = b.End.X, p3_y = b.End.Y, s1_x, s1_y, s2_x, s2_y, s, t;
            s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
            s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;
            s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);
            return s >= 0 && s <= 1 && t >= 0 && t <= 1 ? new Point((int)(p0_x + (t * s1_x)), (int)(p0_y + (t * s1_y))) : null;
        }
        /// <summary>
        /// Find the intersection rect of a rect and a rect
        /// </summary>
        public static Rect IntersectionRect(Rect a, Rect b) {
            return new Rect(new Point(Max(a.Left, b.Left), Max(a.Top, b.Top)), new Point(Min(a.Right, b.Right), Min(a.Bottom, b.Bottom)));
        }

        static int Min(int a, int b) => a > b ? b : a;
        static int Max(int a, int b) => a > b ? a : b;
    }
}
