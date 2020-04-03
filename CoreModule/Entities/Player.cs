using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Terrain;
using CoreModule.Shapes;
using CoreModule.Drawables;

namespace CoreModule.Entities {
    public class Player : PhysicsEntity {
        public Slot Head { get; } = new Slot("Player/head");
        public Slot Torso { get; } = new Slot("Player/torso");
        public Slot Legs { get; } = new Slot("Player/legs");
        public Slot Feet { get; } = new Slot("Player/feet");

        public Player(int x, int y) : base(x, y, 10, 32, null) {
            Head.Under.Playing = true;
            Torso.Under.Playing = true;
            Legs.Under.Playing = true;
            Feet.Under.Playing = true;

            Children.Add(Head);
            Children.Add(Torso);
            Children.Add(Legs);
            Children.Add(Feet);

            //DrawDebug = true;
        }

        public override void Update(float fElapsedTime) {
            int vel = 0;
            if (CoreGame.Instance.GetKey(Key.A).Down) vel--;
            if (CoreGame.Instance.GetKey(Key.D).Down) vel++;
            Velocity.X = vel;
            if (CoreGame.Instance.GetKey(Key.W).Pressed) Velocity.Y = -2.5f;

            Move(fElapsedTime);

            Head.X = (int)X;
            Head.Y = (int)Y;
            Torso.X = (int)X;
            Torso.Y = (int)Y + 8;
            Legs.X = (int)X;
            Legs.Y = (int)Y + 16; 
            Feet.X = (int)X;
            Feet.Y = (int)Y + 24;

            foreach (Drawable child in Children) child.Update(fElapsedTime);
        }
    }

    public class Slot : Drawable {
        public Equipable Contents { get; set; }
        public Animation Under { get; set; }

        public Slot() {
            Children.Add(Under);
        }
        public Slot(string name) {
            Under = Animation.Load(name);
            Children.Add(Under);
        }

        public override void Update(float fElapsedTime) {
            Under.X = X;
            Under.Y = Y;
            base.Update(fElapsedTime);
        }

        public override void Draw() {
            base.Draw();
            Contents?.Draw();
        }
    }

    public class Equipable : Drawable {

    }
}
