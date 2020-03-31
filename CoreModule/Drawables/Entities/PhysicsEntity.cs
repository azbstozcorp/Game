using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;
using CoreModule.Terrain;
using static CoreModule.Collision;

namespace CoreModule.Drawables.Entities {
    public class PhysicsEntity : Drawable {
        public float X {
            get => Bounds.Left; set {
                Bounds.Right = value + Bounds.Width;
                Bounds.Left = value;
            }
        }
        public float Y {
            get => Bounds.Top; set {
                Bounds.Bottom = value + Bounds.Height;
                Bounds.Top = value;
            }
        }

        public float Gravity { get; set; } = 0.1f;
        public PointF Velocity { get; set; } = new PointF();
        public PixelEngine.Sprite Sprite { get; set; }

        HashSet<Chunk> containingChunks = new HashSet<Chunk>();
        new RectF Bounds;
        RectF oldBounds;

        public PhysicsEntity(int x, int y, int width, int height, PixelEngine.Sprite sprite) {
            Bounds = new Rect(new Point(x, y), width, height);
            oldBounds = Bounds.Copy;
            Sprite = sprite;
        }

        void GetContainingChunks() {
            containingChunks.Add(World.Instance.GetChunkWithPoint(Bounds.TopLeft));
            containingChunks.Add(World.Instance.GetChunkWithPoint(Bounds.TopRight));
            containingChunks.Add(World.Instance.GetChunkWithPoint(Bounds.BottomLeft));
            containingChunks.Add(World.Instance.GetChunkWithPoint(Bounds.BottomRight));
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            oldBounds = Bounds.Copy;

            Velocity.Y += Gravity;
            X += Velocity.X;
            Y += Velocity.Y;

            List<Rect> collidingRects = new List<Rect>();
            GetContainingChunks();
            foreach (Chunk chunk in containingChunks) {
                if (chunk == null) continue;
                Rect locallyPositionedBounds = Bounds.Copy;
                locallyPositionedBounds.Left -= chunk.WorldPosition.X * Chunk.ChunkSize;
                locallyPositionedBounds.Top -= chunk.WorldPosition.Y * Chunk.ChunkSize;
                locallyPositionedBounds.Right -= chunk.WorldPosition.X * Chunk.ChunkSize;
                locallyPositionedBounds.Bottom -= chunk.WorldPosition.Y * Chunk.ChunkSize;

                foreach (Rect terrainCollider in chunk.Colliders) {
                    if (RectsOverlap(locallyPositionedBounds, terrainCollider)) {
                        Rect globalPositionedCollider = terrainCollider.Copy;
                        globalPositionedCollider.Left += chunk.WorldPosition.X * Chunk.ChunkSize;
                        globalPositionedCollider.Top += chunk.WorldPosition.Y * Chunk.ChunkSize;
                        globalPositionedCollider.Right += chunk.WorldPosition.X * Chunk.ChunkSize;
                        globalPositionedCollider.Bottom += chunk.WorldPosition.Y * Chunk.ChunkSize;
                        collidingRects.Add(globalPositionedCollider);
                    }
                }
            }

            foreach (Rect collider in collidingRects) {
                PointF change = Bounds.Center - oldBounds.Center;

                float margin = 4f;

                Rect left = new Rect(new PointF(Bounds.Left - 1, Bounds.Top + margin), new PointF(Bounds.Left, Bounds.Bottom - margin));
                Rect right = new Rect(new PointF(Bounds.Right, Bounds.Top + margin), new PointF(Bounds.Right + 1, Bounds.Bottom - margin));
                Rect up = new Rect(new PointF(Bounds.Left + margin, Y - 1), new PointF(Bounds.Right - margin, Y));
                Rect down = new Rect(new PointF(Bounds.Left + margin, Bounds.Center.Y), new PointF(Bounds.Right - margin, Bounds.Bottom + 1));

                if (RectsOverlap(left, collider)) {
                    X = collider.Right;
                    Velocity.X = 0;
                }
                if (RectsOverlap(right, collider)) {
                    X = collider.Left - Bounds.Width;
                    Velocity.X = 0;
                }
                if (RectsOverlap(up, collider)) {
                    Y = collider.Bottom;
                    Velocity.Y = 0;
                }
                if (RectsOverlap(down, collider)) {
                    Y = collider.Top - Bounds.Height;
                    Velocity.Y = 0;
                }
            }
        }

        public override void Draw() {
            base.Draw();

            if (Sprite != null)
                CoreGame.Instance.DrawSprite(Bounds.TopLeft, Sprite);

            CoreGame.Instance.DrawRect(Bounds.TopLeft, Bounds.BottomRight, PixelEngine.Pixel.Presets.Yellow);
        }
    }
}
