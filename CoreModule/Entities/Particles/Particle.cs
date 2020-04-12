namespace CoreModule.Entities.Particles {
    public class Particle : PhysicsEntity {
        bool wasOnGround = false;
        public float maxAge;
        public float age;

        public Particle(int x, int y, float velX, float velY) : base(x, y, 1, 1, null) {
            Velocity.X = velX;
            Velocity.Y = velY;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            wasOnGround = OnGround;
            if (age > maxAge) ParticleManager.Instance.RemoveParticle(this);
        }
        public override void Draw(bool drawDebug = false) {
            base.Draw();
        }
    }
}
