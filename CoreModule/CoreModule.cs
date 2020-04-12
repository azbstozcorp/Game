using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Scenes;

namespace CoreModule {
    public static class Main {
        public static void Start() {
            Console.WriteLine("Starting core engine...");
            new CoreGame();
        }
    }

    public class CoreGame : Game {
        public static CoreGame Instance { get; private set; }

        /// <summary>
        /// Contains a stack of currently loaded scenes. The top of this stack will be updated in OnUpdate
        /// </summary>
        Stack<Scene> scenes;

        public CoreGame() {
            Console.WriteLine("Constructing core game");
            Instance = this;

            Construct(300, 180, 4, 4, 120);

            Sound.SoundPlayer.Init();

            scenes = new Stack<Scene>();
            scenes.Push(new MainMenu());

            Start();
        }

        float oldFPS = 0;
        public override void OnUpdate(float elapsed) {
            base.OnUpdate(elapsed);
            SceneUpdate(elapsed);
            elapsed = oldFPS * 0.99f + elapsed * 0.01f;
            AppName = $"Unnamed - {1f/elapsed}";
            oldFPS = elapsed;
        }

        public override void OnDestroy() {
            base.OnDestroy();
        }

        public void PushScene(Scene scene) => scenes.Push(scene);
        public void PopScene() => scenes.Pop();

        void SceneUpdate(float fElapsedTime) {
            if (scenes.Count <= 0) {
                Console.WriteLine("Scene stack empty.");
                return;
            }
            scenes.Peek().Update(fElapsedTime);
            scenes.Peek().Draw();
        }

        #region Events
        #region Mouse Events
        public delegate void MouseEventHandler(int X, int Y, Mouse mouse);
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MousePress;
        public event MouseEventHandler MouseRelease;

        public override void OnMouseDown(Mouse m) {
            base.OnMouseDown(m);
            MouseDown?.Invoke(MouseX, MouseY, m);
        }
        public override void OnMousePress(Mouse m) {
            base.OnMousePress(m);
            MousePress?.Invoke(MouseX, MouseY, m);
        }
        public override void OnMouseRelease(Mouse m) {
            base.OnMouseRelease(m);
            MouseRelease?.Invoke(MouseX, MouseY, m);
        }
        #endregion Mouse Events
        #region Key Events
        public delegate void KeyEventHandler(Key key);
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyPress;
        public event KeyEventHandler KeyRelease;

        public override void OnKeyDown(Key k) {
            base.OnKeyDown(k);
            KeyDown?.Invoke(k);
        }
        public override void OnKeyPress(Key k) {
            base.OnKeyPress(k);
            KeyPress?.Invoke(k);
        }
        public override void OnKeyRelease(Key k) {
            base.OnKeyRelease(k);
            KeyRelease?.Invoke(k);
        }
        #endregion Key Events
        #endregion
    }
}