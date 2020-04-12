namespace CoreModule.Entities.Particles {
    public class Hit : Particle {
        public PixelEngine.Pixel Colour { get; private set; }
        Shapes.Point last, lastlast;

        public Hit(int x, int y, float velX, float velY, PixelEngine.Pixel col) : base(x, y, velX, velY) {
            last = lastlast = new Shapes.Point(x, y);
            Colour = col;
            Bounciness = 0.4f;
            Bounciness = 0.9f;
            Friction = 1.08f;
            maxAge = CoreGame.Instance.Random(1000f, 2000f); 
        }

        public override void Update(float fElapsedTime) {
            lastlast = last; 
            last = Bounds.TopLeft;
            base.Update(fElapsedTime);
            //Colour = PixelEngine.Pixel.Presets.White;
            Colour = new PixelEngine.Pixel(255, 255, 255, (byte)CoreGame.Instance.Map(age, 0, maxAge, 255, 0));
        }

        public override void Draw(bool drawDebug = false) {
            base.Draw(); 
            CoreGame.Instance.DrawLine(lastlast.ToScreen(), last.ToScreen(), Colour);
            CoreGame.Instance.DrawLine(Bounds.TopLeft.ToScreen(), last.ToScreen(), Colour);
            CoreGame.Instance.Draw(Bounds.TopLeft.ToScreen(), Colour);
        }
    }
}
