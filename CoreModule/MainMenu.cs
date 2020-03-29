using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;

namespace CoreModule {
    public class MainMenu : Scene {
        static Random RNG = new Random();
        static Sprite banner = Sprite.Load("Assets/Scenes/MainMenu/banner.png");

        public MainMenu() {
            Drawables.Add(new Banner(CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 - 40));
            Drawables.Add(new Button("Play", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2));
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
        }
        public override void Draw(CoreGame instance) {
            base.Draw(instance);
            instance.Draw(instance.MouseX, instance.MouseY, instance.Random(Pixel.PresetPixels));
        }

        class Banner : Drawable {
            public Banner(int centerX, int centerY) {
                Bounds = new Rect(new Point(centerX - banner.Width / 2, centerY - banner.Height / 2),
                                  banner.Width, banner.Height);
            }

            public override void Draw(CoreGame instance) {
                base.Draw(instance);
                Point drawPosition = Collision.WithinRect(Bounds, (instance.MouseX, instance.MouseY))
                    ? new Point(Position.X + RNG.Next(-1, 2), Position.Y + RNG.Next(-1, 2)) : Position;
                instance.DrawSprite(drawPosition, banner);
            }
        }
    }
}
