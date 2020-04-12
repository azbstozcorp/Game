using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using PixelEngine.Extensions.Transforms;
using CoreModule.Terrain;
using CoreModule.Shapes;
using CoreModule.Drawables;

namespace CoreModule.Entities {
    public class Player : PhysicsEntity {
        public Animator Animator = Animator.Load("Player/player");

        int direction = 1;

        public Player(int x, int y) : base(x, y, 10, 32, null) {
            Animator.Playing = true;
        }

        public override void Update(float fElapsedTime) {
            int vel = 0;
            if (CoreGame.Instance.GetKey(Key.A).Down) vel--;
            if (CoreGame.Instance.GetKey(Key.D).Down) vel++;
            Velocity.X = vel;
            if (CoreGame.Instance.GetKey(Key.W).Pressed) Velocity.Y = -2.5f;

            Move(fElapsedTime);

            Animator.X = (int)X;
            Animator.Y = (int)Y;
            Animator.Update(fElapsedTime);

            foreach (Drawable child in Children) child.Update(fElapsedTime);

            if (vel != 0) {
                direction = Math.Sign(vel);
                if (Animator.CurrentAnimationName != "walk")
                    Animator.Play("walk");
            }
            else {
                if (Animator.CurrentAnimationName != "idle")
                    Animator.Play("idle");
            }

            Animator.Direction = direction;
        }

        public override void Draw(bool drawDebug = false) {
            base.Draw();

            Animator.Draw();
        }
    }
}
