namespace Playa.App
{
    public abstract class OptionClass{
        public string type = "Base";
        public int _Priority;
        public abstract bool Compare(OptionClass optionClass);
        public abstract void OnUpdate();
        public abstract void OnRemove();
    }
}