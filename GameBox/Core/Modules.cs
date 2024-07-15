namespace GameBox.Core
{
    public abstract class Module
    {
        public abstract void Initialize();
        public abstract void Update();
        public abstract void Render();
    }
}
