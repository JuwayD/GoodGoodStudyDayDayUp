using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 子弹逻辑 (Projectile)
    /// 负责飞行寻目标以及造成伤害
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public float Speed = 10f;
        public float Lifetime = 5f;
        
        private EnemyEntity _target;
        private int _damage;

        public void Launch(EnemyEntity target, int damage)
        {
            _target = target;
            _damage = damage;
            Destroy(gameObject, Lifetime);
        }

        private void Update()
        {
            if (_target == null || _target.IsDead)
            {
                // 如果目标丢失或死亡，子弹继续朝前飞一段距离后自毁
                transform.Translate(Vector3.forward * Speed * Time.deltaTime);
                return;
            }

            // 追踪目标
            Vector3 direction = (_target.transform.position + Vector3.up * 0.5f) - transform.position;
            float step = Speed * Time.deltaTime;

            if (direction.magnitude <= step)
            {
                HitTarget();
            }
            else
            {
                transform.LookAt(_target.transform.position + Vector3.up * 0.5f);
                transform.Translate(Vector3.forward * step);
            }
        }

        private void HitTarget()
        {
            if (_target != null)
            {
                _target.TakeDamage(_damage);
            }
            
            // 消失特效可以在这里播放
            Destroy(gameObject);
        }
    }
}
