using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

namespace CoreModule {
    public class MainMenu : Scene {
        Random RNG = new Random();

        Sprite banner = Sprite.Load("Assets/Scenes/MainMenu/banner.png");
        Sprite playButton = Sprite.Load("Assets/Scenes/MainMenu/playbutton.png");

        public override void Update(float fElapsedTime, CoreGame instance) {
            base.Update(fElapsedTime, instance);
            instance.Clear(Pixel.Empty);

            int bannerX = (instance.ScreenWidth) / 2 - banner.Width / 2 + RNG.Next(-1, 1);
            int bannerY = (instance.ScreenHeight) / 2 - banner.Height - 20 + RNG.Next(-1, 1);
            instance.DrawSprite(new Point(bannerX, bannerY), banner);

            int playButtonX = (instance.ScreenWidth) / 2 - playButton.Width / 2;
            int playButtonY = (instance.ScreenHeight) / 2 - playButton.Height / 2;
            instance.DrawSprite(new Point(playButtonX, playButtonY), playButton);

            if (instance.MouseX > playButtonX && instance.MouseX < playButtonX + playButton.Width && instance.MouseY > playButtonY && instance.MouseY < playButtonY + playButton.Height)
                instance.DrawRect(new Point(playButtonX - 1, playButtonY - 1),
                    new Point(playButtonX + playButton.Width + 1, playButtonY + playButton.Height + 1),
                    instance.Random(Pixel.PresetPixels));

            instance.Draw(instance.MouseX, instance.MouseY, instance.Random(Pixel.PresetPixels));
        }
    }
}
