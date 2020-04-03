using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

namespace CoreModule.Drawables {
    public class Animation : Drawable {
        public Sprite[] Frames { get; private set; }
        public bool Playing { get; set; }

        float frameTime = 0f;
        int frame = 0;
        public float millisecondsPerFrame = 1f;
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Create an animation by name file
        /// </summary>
        /// <param name="name">path to namefile</param>
        public static Animation Load(string name) {
            string namefile = File.ReadAllText($"Assets/Animations/{name}.namefile");
            string[] data = namefile.Split(' ');

            string path = data[0];
            int numFrames = int.Parse(data[1]);
            int frameWidth = int.Parse(data[2]);
            float delay = float.Parse(data[3]);

            Animation result = new Animation(path, numFrames, frameWidth);
            result.millisecondsPerFrame = delay;
            return result;
        }
        /// <summary>
        /// Create an animation from a png sheet
        /// </summary>
        /// <param name="path">Path to sheet</param>
        /// <param name="numFrames">Number of frames</param>
        /// <param name="frameWidth">Width of frames</param>
        public Animation(string path, int numFrames, int frameWidth) {
            Sprite sheet = Sprite.Load(path);
            if (sheet.Width / numFrames != frameWidth) throw new ArgumentException("Sheet not the right size.");

            Frames = new Sprite[numFrames];
            for (int frame = 0; frame < numFrames; frame++) { // Copy the frames in the sheet into the frame array
                Sprite current = new Sprite(frameWidth, sheet.Height);
                for (int x = 0; x < frameWidth; x++) {
                    for (int y = 0; y < sheet.Height; y++) {
                        current[x, y] = sheet[x + frame * frameWidth, y];
                    }
                }
                Frames[frame] = current;
            }
            timer.Start();
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (Playing) frameTime += fElapsedTime;
            if (timer.ElapsedMilliseconds >= millisecondsPerFrame) { timer.Restart(); frame++; }
            if (frame >= Frames.Length) frame = 0;
        }

        public override void Draw() {
            base.Draw();
            CoreGame.Instance.DrawSprite(Bounds.TopLeft + Scenes.Level.Instance.CameraLocation, Frames[frame]);
        }
    }
}
