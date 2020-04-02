using System.Collections.Generic;
using CoreModule.Drawables;

namespace CoreModule.Scenes {
    /// <summary>
    /// Base class for level states, when you want different editing / drawing functionality but not an entire different scene
    /// </summary>
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
