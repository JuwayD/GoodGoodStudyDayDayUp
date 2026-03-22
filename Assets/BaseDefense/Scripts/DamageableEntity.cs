using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 可受击实体基类 (DamageableEntity)
    /// 统一管理带有生命值的游戏对象（如建筑、敌人）
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public abstract class DamageableEntity : MonoBehaviour
    {
        protected HealthComponent healthComponent;

        protected virtual void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            healthComponent.OnDeath += HandleDeath;
        }

        /// <summary>
        /// 处理死亡逻辑（默认直接销毁，子类可重写以触发特效或掉落）
        /// </summary>
        protected virtual void HandleDeath()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 外部调用的受伤接口
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            healthComponent.TakeDamage(damage);
        }
        
        public bool IsDead => healthComponent != null && healthComponent.CurrentHealth <= 0;
    }
}
