
namespace Assets.Scripts.Interfaces
{
    public interface IDamageable
    {
        float TakeDamage(float physicalDamage, float magicDamage);
        bool IsAlive();

    }
}