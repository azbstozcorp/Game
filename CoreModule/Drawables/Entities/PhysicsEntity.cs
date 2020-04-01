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
        public PixelEngine.Sprite Sprite;

        HashSet<Chunk> containingChunks = new HashSet<Chunk>();
        new RectF Bounds;
        RectF oldBounds;

        public PhysicsEntity(int x, int y, int width, int height, PixelEngine.Sprite sprite) {
            Bounds = new Rect(new Point(x, y), width, height);
            oldBounds = Bounds.Copy;
            Sprite = sprite;
        }

        void GetContainingChunks() {
            containingChunks.Add(Level.Instance.GetChunkWithPoint(Bounds.TopLeft));
            containingChunks.Add(Level.Instance.GetChunkWithPoint(Bounds.TopRight));
            containingChunks.Add(Level.Instance.GetChunkWithPoint(Bounds.BottomLeft));
            containingChunks.Add(Level.Instance.GetChunkWithPoint(Bounds.BottomRight));
        }

        void Move() {
            List<Rect> collidingRects = new List<Rect>();

            Velocity.Y += Gravity;
            RectF newBounds = Bounds.Copy;
            newBounds.Move(Velocity.X, Velocity.Y);

            void GetCollisions(RectF with) {
                collidingRects.Clear();
                GetContainingChunks();
                foreach (Chunk chunk in containingChunks) {
                    if (chunk == null) continue;
                    Rect locallyPositionedBounds = with.Copy;
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
            }

            bool up = false, down = false, left = false, right = false;

            if (Velocity.Y != 0) {
                RectF vertical = new RectF();
                if (Velocity.Y < 0) {
                    vertical = new RectF(Bounds.Left, Bounds.Top + Velocity.Y, Bounds.Right, Bounds.Top);
                    up = true;
                }
                else {
                    vertical = new RectF(Bounds.Left, Bounds.Bottom, Bounds.Right, Bounds.Bottom + Velocity.Y);
                    down = true;
                }
                GetCollisions(vertical);
                foreach (Rect collision in collidingRects) {
                    Rect overlap = IntersectionRect(vertical, collision);
                    if (down) {
                        newBounds.Move(0, -overlap.Height);
                    }
                    if (up) {
                        newBounds.Move(0, overlap.Height);
                    }
                    Velocity.Y = 0;
                    break;
                }
            }

            if (Velocity.X != 0) {
                RectF horizontal = new RectF();
                if (Velocity.X < 0) {
                    horizontal = new RectF(Bounds.Left + Velocity.X, Bounds.Top, Bounds.Left, Bounds.Bottom);
                    left = true;
                }
                else {
                    horizontal = new RectF(Bounds.Right, Bounds.Top, Bounds.Right + Velocity.X, Bounds.Bottom);
                    right = true;
                }
                GetCollisions(horizontal);
                foreach (Rect collision in collidingRects) {
                    Rect overlap = IntersectionRect(horizontal, collision);
                    if (left) {
                        newBounds.Move(overlap.Width, 0);
                    }
                    if (right) {
                        newBounds.Move(-overlap.Width, 0);
                    }
                    Velocity.X = 0;
                }
            }
            Bounds = newBounds;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            Move();
        }

        public override void Draw() {
            base.Draw();

            if (Sprite != null)
                CoreGame.Instance.DrawSprite(Bounds.TopLeft, Sprite);

            CoreGame.Instance.DrawRect(Bounds.TopLeft, Bounds.BottomRight, PixelEngine.Pixel.Presets.Yellow);
        }
    }
}
