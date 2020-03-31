﻿using System;
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

        public void ResetBounds() {
            oldBounds = Bounds.Copy;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            oldBounds = Bounds.Copy;
            Velocity.Y += Gravity;
            X += Velocity.X;
            Y += Velocity.Y + Gravity;

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
                if (Bounds.Bottom > collider.Top) {
                    Y = collider.Top - Bounds.Height;
                    Velocity.Y = 0;
                }
                else if (Bounds.Top < collider.Bottom) {
                    Y = collider.Bottom;
                    Velocity.Y = 0;
                }

                if (Bounds.Right < collider.Left) {
                    X = collider.Left - Bounds.Width;
                    Velocity.X = 0;
                }
                else if(Bounds.Left > collider.Right){
                    X = collider.Right;
                    Velocity.X = 0;
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
