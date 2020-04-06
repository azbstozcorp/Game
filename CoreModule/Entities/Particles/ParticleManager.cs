using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Entities.Particles {
    public class ParticleManager : Drawables.Drawable {
        HashSet<Particle> removalQueue = new HashSet<Particle>();

        public void AddParticle(Particle p) { Children.Add(p); }
        public void RemoveParticle(Particle p) { removalQueue.Add(p); }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            while (Children.Count > 1000) Children.Remove(Children.First());
            foreach (Particle p in removalQueue) Children.Remove(p);
        }

        public override void Draw() {
            base.Draw();
            foreach (Drawables.Drawable d in Children) d.Draw();
        }
    }

    public class Particle : PhysicsEntity {
        bool wasOnGround = false;

        public Particle(int x, int y, float velX, float velY) : base(x, y, 1, 1, null) {
            Velocity.X = velX;
            Velocity.Y = velY;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            //if(OnGround && wasOnGround)Velocity.X /= 1.05f;
            wasOnGround = OnGround;

            //if(Velocity.X == 0 && Velocity.Y == 0) { Scenes.Level.Instance.ParticleManager.RemoveParticle(this); }
        }
        public override void Draw() {
            base.Draw();
            //CoreGame.Instance.Draw(Scenes.Level.WorldToScreen(Bounds.TopLeft), PixelEngine.Pixel.Presets.White);
        }
    }

    public class Hit : Particle {
        public PixelEngine.Pixel Colour { get; private set; }
        PixelEngine.Point last, lastlast;
        public Hit(int x, int y, float velX, float velY, PixelEngine.Pixel col) : base(x, y, velX, velY) {
            last = lastlast = new PixelEngine.Point(x, y);
            Colour = col;
            Bounciness = 0.4f;
            Friction = 1.08f;
        }

        public override void Update(float fElapsedTime) {
            lastlast = last; 
            last = Bounds.TopLeft;
            base.Update(fElapsedTime);
        }

        public override void Draw() {
            base.Draw(); 
            CoreGame.Instance.DrawLine(Scenes.Level.WorldToScreen(lastlast), Scenes.Level.WorldToScreen(last), Colour);
            CoreGame.Instance.DrawLine(Scenes.Level.WorldToScreen(Bounds.TopLeft), Scenes.Level.WorldToScreen(last), Colour);
            CoreGame.Instance.Draw(Scenes.Level.WorldToScreen(Bounds.TopLeft), Colour);
        }
    }
}
