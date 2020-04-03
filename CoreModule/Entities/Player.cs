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
        public Animation Idle { get; } = Animation.Load("Player/idle");
        public Animation Walk { get; } = Animation.Load("Player/walk");
        public Animation Current { get; set; }

        int direction = 1;

        public Player(int x, int y) : base(x, y, 10, 32, null) {
            Current = Walk;
            Current.Playing = true;
        }

        public override void Update(float fElapsedTime) {
            int vel = 0;
            if (CoreGame.Instance.GetKey(Key.A).Down) vel--;
            if (CoreGame.Instance.GetKey(Key.D).Down) vel++;
            Velocity.X = vel;
            if (CoreGame.Instance.GetKey(Key.W).Pressed) Velocity.Y = -2.5f;

            Move(fElapsedTime);

            Current.X = (int)X;
            Current.Y = (int)Y;
            Current.Update(fElapsedTime);

            foreach (Drawable child in Children) child.Update(fElapsedTime);

            if (vel != 0) {
                direction = Math.Sign(vel);
                Current = Walk;
            }
            else {
                Current = Idle;
            }

            Current.Direction = direction;
        }

        public override void Draw() {
            base.Draw();

            Current.Draw();
        }
    }
}
