namespace Autovrse
{

    public interface IDamagable
    {
        public float Heath { get; }

        public void DoDamage(float value);
    }
}
