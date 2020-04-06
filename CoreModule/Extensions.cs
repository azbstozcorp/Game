using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Ballistics;
using CoreModule.Drawables;
using CoreModule.Entities;
using CoreModule.Entities.Particles;
using CoreModule.Guns;
using CoreModule.Saving;
using CoreModule.Scenes;
using CoreModule.Shapes;
using CoreModule.Sound;
using CoreModule.Terrain;

namespace CoreModule {
    public static class Extensions {
        public static Point ToWorld(this Point me) => Level.ScreenToWorld(me);
        public static Point ToWorld(this PointF me) => ((Point)me).ToWorld();
        public static Point ToScreen(this Point me) => Level.WorldToScreen(me);
        public static Point ToScreen(this PointF me) => ((Point)me).ToScreen();

        public static bool Collides(Point p, Rect r, bool inclusive = false) => Collision.WithinRect(r, p, inclusive);
        public static bool Collides(Rect a, Rect b) => Collision.RectsOverlap(a, b);
        public static bool Collides(Line a, Line b) => Collision.LinesIntersect(a, b);
        public static bool Collides(Line l, Rect r) => Collision.LineOverlapsRect(l, r);
    }
}
