﻿using System;
using System.Collections;
using System.Collections.Generic;

using CoreModule.Drawables;

namespace CoreModule.Scenes {
    [Serializable]
    public class Scene {
        public List<Drawable> Drawables { get; set; } = new List<Drawable>();

        public virtual void Update(float fElapsedTime) {
            foreach (Drawable d in Drawables) d.Update(fElapsedTime);
        }

        public virtual void Draw() {
            CoreGame.Instance.Clear(PixelEngine.Pixel.Empty);
            foreach (Drawable d in Drawables) d.Draw();
        }
    }
}