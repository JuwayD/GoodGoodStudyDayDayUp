using UnityEngine;
using UnityEngine.AI;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 敌人实体 (EnemyEntity)
    /// 负责敌人的寻路、攻击逻辑
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyEntity : DamageableEntity
    {
        [Header("属性")]
        public int AttackDamage = 10;
        public float AttackInterval = 1.0f;
        public float StopDistance = 1.5f;

        private NavMeshAgent _agent;
        private Transform _target;
        private float _nextAttackTime;

        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponent<NavMeshAgent>();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            if (_agent != null && _target != null && _agent.isOnNavMesh)
            {
                _agent.SetDestination(_target.position);
            }
            else if (_agent != null && !_agent.isOnNavMesh)
            {
                BDLogger.LogWarning($"{gameObject.name} is NOT on NavMesh. Check if NavMesh is baked.");
            }
        }

        private void Update()
        {
            if (_target == null || IsDead) return;

            // 持续更新目标位置（如果目标在移动）
            // 注意：为了性能考虑，实际项目中通常会限制更新频率
            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance <= StopDistance)
            {
                _agent.isStopped = true;
                TryAttack();
            }
            else
            {
                _agent.isStopped = false;
                _agent.SetDestination(_target.position);
            }
        }

        private void TryAttack()
        {
            if (Time.time >= _nextAttackTime)
            {
                DamageableEntity targetEntity = _target.GetComponent<DamageableEntity>();
                if (targetEntity != null)
                {
                    targetEntity.TakeDamage(AttackDamage);
                    _nextAttackTime = Time.time + AttackInterval;
                    BDLogger.LogDetail($"{gameObject.name} 攻击了目标，造成 {AttackDamage} 伤害");
                }
            }
        }

        protected override void HandleDeath()
        {
            BDLogger.LogInfo($"敌人 {gameObject.name} 被消灭");
            // 可以在此处增加掉落资源逻辑
            base.HandleDeath();
        }
    }
}
