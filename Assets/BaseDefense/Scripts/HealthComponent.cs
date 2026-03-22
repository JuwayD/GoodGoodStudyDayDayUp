using System;
using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 通用生命值组件 (HealthComponent)
    /// 负责生命值的数值维护、受击计算与死亡触发
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [Header("生命值设置")]
        [SerializeField] private int _maxHealth = 100;
        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public float HealthPercent => (float)_currentHealth / _maxHealth;

        // 事件：参数为 (当前血量, 最大血量)
        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (damage <= 0 || _currentHealth <= 0) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);

            BDLogger.LogDetail($"{gameObject.name} 受到 {damage} 点伤害，剩余血量: {_currentHealth}/{_maxHealth}");
            
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            BDLogger.LogInfo($"{gameObject.name} 已死亡/被销毁");
            OnDeath?.Invoke();
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void Heal(int amount)
        {
            if (amount <= 0 || _currentHealth <= 0) return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }
}
