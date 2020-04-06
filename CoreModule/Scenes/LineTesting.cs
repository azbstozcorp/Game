using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;
using static CoreModule.Collision;

namespace CoreModule.Scenes {
    public class LineTesting : Scene {
        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
        }

        public override void Draw() {
            base.Draw();

            Line a = new Line(new Point(0, 0), new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY));
            Line b = new Line(new Point(60, 0), new Point(0, 60));

            CoreGame.Instance.DrawLine(a.Start, a.End, PixelEngine.Pixel.Presets.White);
            CoreGame.Instance.DrawLine(b.Start, b.End, PixelEngine.Pixel.Presets.Blue);

            if(LinesIntersect(a,b))
            CoreGame.Instance.Draw(IntersectionOf(a, b), PixelEngine.Pixel.Presets.Red);
        }
    }
}
