using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;
using CoreModule.Shapes;

using Point = CoreModule.Shapes.Point;

namespace CoreModule {
    public class MainMenu : Scene {
        static Random RNG = new Random();
        static Sprite banner = Sprite.Load("Assets/Scenes/MainMenu/banner.png");

        public MainMenu() {
            Drawables.Add(new Banner(CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 - 30));

            Button playButton = new Button("Play", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2);
            playButton.ButtonPressed += PlayButton_ButtonPressed;
            Drawables.Add(playButton);
            Drawables.Add(new Button("Settings", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 + 10));
            Drawables.Add(new Button("Quit", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 + 20));
        }

        private void PlayButton_ButtonPressed() {
            CoreGame.Instance.PushScene(new Level());
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
        }
        public override void Draw() {
            base.Draw();
            CoreGame.Instance.Draw(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY, CoreGame.Instance.Random(Pixel.PresetPixels));
        }

        class Banner : Drawable {
            public Banner(int centerX, int centerY) {
                Bounds = new Rect(new Point(centerX - banner.Width / 2, centerY - banner.Height / 2),
                                  banner.Width, banner.Height);
            }

            public override void Draw() {
                base.Draw();
                Point drawPosition = Collision.WithinRect(Bounds, (CoreGame.Instance.MouseX, CoreGame.Instance.MouseY))
                    ? new Point(Position.X + RNG.Next(-1, 2), Position.Y + RNG.Next(-1, 2)) : Position;
                CoreGame.Instance.DrawSprite(drawPosition, banner);
            }
        }
    }
}
