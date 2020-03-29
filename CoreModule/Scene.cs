using System;
using System.Collections;
using System.Collections.Generic;

using CoreModule.Drawables;

namespace CoreModule {
    public class Scene {
        public List<Drawable> Drawables { get; set; } = new List<Drawable>();

        public virtual void Update(float fElapsedTime) {
            foreach (Drawable d in Drawables) d.Update(fElapsedTime);
        }

        public virtual void Draw(CoreGame instance) {
            instance.Clear(PixelEngine.Pixel.Empty);
            foreach (Drawable d in Drawables) d.Draw(instance);
        }
    }
}