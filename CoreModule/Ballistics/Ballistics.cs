using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;
using CoreModule.Terrain;
using CoreModule.Entities.Particles;
using Pixel = PixelEngine.Pixel;

namespace CoreModule.Ballistics {
    public static class Ballistics {
        public static void Fire(Point from, Point through) {
            Sound.SoundPlayer.PlayOnce("Assets/Audio/GS_handgun_bass.wav");

            float angle = (float)(Math.Atan2(through.Y - from.Y, through.X - from.X));
            Line result = new Line(from, angle, 1000);

            int dirX = -Math.Sign(through.X - from.X);
            int dirY = -Math.Sign(through.Y - from.Y);
            Scenes.Level.Instance.CameraLocation.X -= dirX * 10;
            Scenes.Level.Instance.CameraLocation.Y -= dirY * 3;

            Rect screen = new Rect(new Point(0, 0).ToWorld(), new Point(CoreGame.Instance.ScreenWidth, CoreGame.Instance.ScreenHeight).ToWorld());
            HashSet<Chunk> chunks = new HashSet<Chunk> {
                        Scenes.Level.Instance.GetChunkWithPoint(screen.TopLeft),
                        Scenes.Level.Instance.GetChunkWithPoint(screen.TopRight),
                        Scenes.Level.Instance.GetChunkWithPoint(screen.BottomLeft),
                        Scenes.Level.Instance.GetChunkWithPoint(screen.BottomRight)
                    };

            List<Point> points = new List<Point>();
            foreach (Chunk c in chunks) {
                foreach (Rect r in c.Colliders) {
                    Rect check = r.Copy;
                    check.Move(c.Bounds.Left, c.Bounds.Top);
                    if (Collision.LineOverlapsRect(result, check)) {
                        if (Collision.LinesIntersect(result, check.TLBL)) points.Add(Collision.IntersectionOf(result, check.TLBL));
                        if (Collision.LinesIntersect(result, check.BRTR)) points.Add(Collision.IntersectionOf(result, check.BRTR));
                        if (Collision.LinesIntersect(result, check.TLTR)) points.Add(Collision.IntersectionOf(result, check.TLTR));
                        if (Collision.LinesIntersect(result, check.BRBL)) points.Add(Collision.IntersectionOf(result, check.BRBL));
                    }
                }
            }  

            if (points.Count > 0) {
                Point closest = Collision.Closest(from, points.ToArray()).ToScreen();

                for (int i = 0; i < 5; i++) {
                    ParticleManager.Instance.AddParticle(
                        new Hit((closest + (dirX, 0)).ToWorld().X, closest.ToWorld().Y, CoreGame.Instance.Random(dirX * 1f, dirX * 4f),
                            CoreGame.Instance.Random(-4f, 4f),
                            CoreGame.Instance.GetScreenPixel(closest.X - dirX * 2, closest.Y - dirY)) { Bounciness = CoreGame.Instance.Random(0.3f, 0.5f) }
                        );
                }

                CoreGame.Instance.DrawLine(from.ToScreen(), closest, Pixel.Presets.White);
            }
            else CoreGame.Instance.DrawLine(from.ToScreen(), result.End.ToScreen(), Pixel.Presets.White);
        }
    }
}
