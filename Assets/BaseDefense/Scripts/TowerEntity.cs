using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 防御塔实体 (TowerEntity)
    /// 自动搜敌、旋转炮塔并按频率发射子弹
    /// </summary>
    public class TowerEntity : BuildingEntity
    {
        [Header("战斗设置")]
        public Transform Rotator;      // 需要旋转的炮塔部件
        public Transform FirePoint;    // 子弹发射起始点
        public GameObject ProjectilePrefab;
        
        private EnemyEntity _currentTarget;
        private float _nextFireTime;

        private void Update()
        {
            if (Data == null) { BDLogger.LogDetail($"{gameObject.name} Data is null!"); return; }
            if (Data.Type != BuildingType.Defense) { BDLogger.LogDetail($"{gameObject.name} Type is not Defense!"); return; }

            FindNearestEnemy();

            if (_currentTarget != null)
            {
                LookAtTarget();
                if (Time.time >= _nextFireTime)
                {
                    BDLogger.LogInfo($"{gameObject.name} is firing at {_currentTarget.name}!");
                    Fire();
                    _nextFireTime = Time.time + Data.AttackInterval;
                }
            }
            else
            {
                // 可选：增加寻找目标的低频日志
            }
        }

        private void FindNearestEnemy()
        {
            // 简单实现：通过物体标签或全局搜索获取所有敌人
            // 之后可以优化为物理金圈检测 (OverlapSphere)
            EnemyEntity[] enemies = FindObjectsOfType<EnemyEntity>();
            float minDistance = Data.AttackRange;
            EnemyEntity nearest = null;

            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.IsDead) continue;
                
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = enemy;
                }
            }

            _currentTarget = nearest;
        }

        private void LookAtTarget()
        {
            if (Rotator == null || _currentTarget == null) return;

            Vector3 direction = _currentTarget.transform.position - Rotator.position;
            direction.y = 0; // 仅水平旋转
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                Rotator.rotation = Quaternion.Slerp(Rotator.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }

        private void Fire()
        {
            if (ProjectilePrefab == null || FirePoint == null || _currentTarget == null) return;

            GameObject go = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
            Projectile projectile = go.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Launch(_currentTarget, Data.AttackDamage);
            }
        }
    }
}
