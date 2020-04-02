using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CoreModule.Drawables;

namespace CoreModule.Scenes {
    public class LevelSelect : Scene {

        List<Drawable> controls;
        Button back;
        Button load;
        TextBox path;

        static string levelExists = "Level exists.";
        static string levelDoesntExist = "No level by that name exists.";

        static string keytipEnter = "Enter : Load / Create level";
        static string keytipEscape = "Escape : Back to menu";

        public LevelSelect() {
            back = new Button("Back", 25, CoreGame.Instance.ScreenHeight / 2 - 4);
            load = new Button("Load", CoreGame.Instance.ScreenWidth - 25, CoreGame.Instance.ScreenHeight / 2 - 4);
            path = new TextBox("TestingRoom", CoreGame.Instance.ScreenWidth / 2, CoreGame.Instance.ScreenHeight / 2 - 4);
            controls = new List<Drawable>() { back, load, path };

            path.TextChanged += UpdateText;
            UpdateText(path.Text);
            path.Press();
            load.Pressed += Load_Pressed;
            back.Pressed += Back_Pressed;
        }

        private void UpdateText(string newText) {
            if (File.Exists($"Assets/Levels/{newText}.bin")) load.Text = "Load";
            else load.Text = "New ";
        }
        private void Load_Pressed(Button pressed) {
            Level level = Level.LoadLevel(path.Text);
            CoreGame.Instance.PushScene(level);
        }
        private void Back_Pressed(Button pressed) {
            CoreGame.Instance.PopScene();
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            UpdateText(path.Text);
            foreach (Drawable control in controls) control.Update(fElapsedTime);

            if (CoreGame.Instance.GetKey(PixelEngine.Key.Control).Pressed) path.Press();
            if (CoreGame.Instance.GetKey(PixelEngine.Key.Enter).Pressed) load.Press();
            if (CoreGame.Instance.GetKey(PixelEngine.Key.Escape).Pressed) back.Press();
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.DrawText(new PixelEngine.Point(1, 18), keytipEnter, PixelEngine.Pixel.Presets.White);
            CoreGame.Instance.DrawText(new PixelEngine.Point(1, 25), keytipEscape, PixelEngine.Pixel.Presets.White);

            if (File.Exists($"Assets/Levels/{path.Text}.bin")) CoreGame.Instance.DrawText(new PixelEngine.Point(CoreGame.Instance.ScreenWidth / 2 - levelExists.Length * 4, 10), levelExists, PixelEngine.Pixel.Presets.Green);
            else CoreGame.Instance.DrawText(new PixelEngine.Point(CoreGame.Instance.ScreenWidth / 2 - levelDoesntExist.Length * 4, 10), levelDoesntExist, PixelEngine.Pixel.Presets.Red);

            foreach (Drawable control in controls) control.Draw();
        }
    }
}
