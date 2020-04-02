using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;
using CoreModule.Drawables;

namespace CoreModule.Scenes {
    public abstract class LevelState {
        public List<Drawable> Drawables { get; } = new List<Drawable>();

        public abstract LevelState TryMoveNext();

        public virtual void Update(float fElapsedTime) {
            foreach (Drawable d in Drawables) d.Update(fElapsedTime);
        }

        public virtual void Draw() {
            foreach (Drawable d in Drawables) d.Draw();
        }
    }
}
