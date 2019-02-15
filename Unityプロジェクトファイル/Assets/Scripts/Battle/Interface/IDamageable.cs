//攻撃を受けることができる//

namespace Battle
{
    public interface IDamageable
    {
        void OnAttackHit(UnityEngine.Collider col);

        void OnDead();
    }
}