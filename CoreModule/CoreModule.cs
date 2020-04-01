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

            Construct(400, 240, 4, 4, 120);

            //Enable(Subsystem.Audio);
            PixelMode = Pixel.Mode.Alpha;

            scenes = new Stack<Scene>();
            scenes.Push(new MainMenu());

            Start();
        }

        float oldFPS = 0;
        public override void OnUpdate(float elapsed) {
            base.OnUpdate(elapsed);
            SceneUpdate(elapsed);
            elapsed = oldFPS * 0.99f + elapsed * 0.01f;
            AppName = $"Unnamed - {elapsed}";
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
    }
}