using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using PixelEngine.Extensions.Transforms;

namespace CoreModule.Drawables {
    public class Animator : Drawable {
        public Dictionary<string, Animation> Animations { get; } = new Dictionary<string, Animation>();
        public Animation CurrentAnimation => Animations[CurrentAnimationName];
        public string CurrentAnimationName => currentAnimation;
        public Sprite[] Frames => Animations[CurrentAnimationName].Frames;
        public Sprite Frame => Frames[frame];

        public int Direction { get; set; } = 1;
        public bool Playing { get; set; }

        string currentAnimation;
        int frame = 0;
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Create an animation by name file
        /// </summary>
        /// <param name="namefileName">path to namefile</param>
        public static Animator Load(string namefileName) {
            string[] namefile = File.ReadAllLines($"Assets/Animations/{namefileName}.namefile");

            Animator result = new Animator();

            for (int i = 0; i < namefile.Length; i++) {
                string[] data = namefile[i].Split(' ');

                string name = data[0];
                string path = data[1];
                int frameWidth = int.Parse(data[2]);
                float delay = float.Parse(data[3]);

                result.LoadSpritesheetToAnimation(name, delay, Sprite.Load(path), frameWidth);
            }

            result.currentAnimation = result.Animations.Keys.First();
            result.Bounds = new Shapes.Rect(new Shapes.Point(0, 0), result.Frame.Width-1, result.Frame.Height);
            return result;
        }

        public void Play(string name) {
            Playing = true;
            timer.Start();

            if (Animations.ContainsKey(name)) currentAnimation = name;
            else currentAnimation = Animations.Keys.First();
            frame = 0;
        }

        void LoadSpritesheetToAnimation(string animationName, float delay, Sprite sheet, int frameWidth) {
            Sprite[] animation = new Sprite[sheet.Width / frameWidth];

            for (int frame = 0; frame < animation.Length; frame++) {
                Sprite current = new Sprite(frameWidth, sheet.Height);

                for (int x = 0; x < frameWidth; x++) {
                    for (int y = 0; y < sheet.Height; y++) {
                        current[x, y] = sheet[x + frame * frameWidth, y];
                    }
                }

                animation[frame] = current;
            }

            Animations[animationName] = new Animation(animation, delay);
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (Playing)
                if (timer.ElapsedMilliseconds >= CurrentAnimation.Delay) { timer.Restart(); frame++; }
            if (frame >= Frames.Length) frame = 0;
        }

        public override void Draw() {
            base.Draw();

            Transform transform = new Transform();

            if (Direction < 0) {
                transform.Scale(-1, 1);
                transform.Translate(Bounds.Width, 0);
            }
            else {
                transform.Scale(1, 1);
            }

            transform.Translate(X + Scenes.Level.Instance.CameraLocation.X, Y + Scenes.Level.Instance.CameraLocation.Y);

            Transform.DrawSprite(Frame, transform);
        }

        public class Animation {
            public Sprite[] Frames { get; private set; }
            public Sprite this[int index] {
                get => Frames[index];
                set => Frames[index] = value;
            }
            public float Delay { get; set; }

            public Animation(Sprite[] frames, float delay) {
                Frames = frames;
                Delay = delay;
            }
            public Animation(int numFrames, float delay) {
                Frames = new Sprite[numFrames];
                Delay = delay;
            }
            public Animation(int numFrames) : this(numFrames, 1) { }
        }
    }
}
