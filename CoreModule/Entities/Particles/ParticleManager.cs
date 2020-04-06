using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Entities.Particles {
    public class ParticleManager : Drawables.Drawable {
        HashSet<Particle> removalQueue = new HashSet<Particle>();

        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        public ParticleManager() {
            timer.Start();
        }

        public void AddParticle(Particle p) { Children.Add(p); }
        public void RemoveParticle(Particle p) { removalQueue.Add(p); }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            foreach (Particle p in Children) ((Particle)p).age += timer.ElapsedMilliseconds;
            timer.Restart();
            while (Children.Count > 1000) Children.Remove(Children.First());
            foreach (Particle p in removalQueue) Children.Remove(p);
        }

        public override void Draw() {
            base.Draw();
            foreach (Drawables.Drawable d in Children) d.Draw();
        }
    }
}
