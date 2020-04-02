using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;
using CoreModule.Shapes;
using CoreModule.Scenes;

using Point = CoreModule.Shapes.Point;

namespace CoreModule.Scenes {
    public class MainMenu : Scene {
        static Random RNG = new Random();
        [NonSerialized] static Sprite banner = Sprite.Load("Assets/Scenes/MainMenu/banner.png");

        public MainMenu() {
            Drawables.Add(new Banner(CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 - 30));

            Button playButton = new Button("Play", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2);
            playButton.Pressed += PlayButton_ButtonPressed;
            Button quitButton = new Button("Quit", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 + 20);
            quitButton.Pressed += QuitButton_ButtonPressed;
            Button settingsButton = new Button("Settings", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 + 10);
            settingsButton.Pressed += SettingsButton_Pressed;

            Drawables.Add(playButton);
            Drawables.Add(settingsButton);
            Drawables.Add(quitButton);
        }

        private void PlayButton_ButtonPressed(Button sender) {
            CoreGame.Instance.PushScene(new LevelSelect());
        }
        private void SettingsButton_Pressed(Button sender) {
            CoreGame.Instance.PushScene(new TileEditor());
        }
        private void QuitButton_ButtonPressed(Button sender) {
            CoreGame.Instance.Finish();
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
