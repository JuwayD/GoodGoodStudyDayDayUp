using UnityEngine;
using UnityEngine.AI;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 己方单位实体 (AlliedUnitEntity)
    /// 玩家可操控的小兵单位，具备寻路和受击能力
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class AlliedUnitEntity : DamageableEntity
    {
        [Header("单位属性")]
        public float MovementSpeed = 3.5f;

        private NavMeshAgent _agent;
        private bool _isSelected = false;
        private GameObject _selectionVisual;

        public bool IsSelected => _isSelected;

        protected override void Awake()
        {
            base.Awake();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = MovementSpeed;

            // 查找或创建选中的视觉反馈（比如脚下的圆圈）
            Transform selectionChild = transform.Find("SelectionCircle");
            if (selectionChild != null)
            {
                _selectionVisual = selectionChild.gameObject;
                _selectionVisual.SetActive(false);
            }
        }

        /// <summary>
        /// 设置单位选中状态
        /// </summary>
        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            if (_selectionVisual != null)
            {
                _selectionVisual.SetActive(isSelected);
            }
        }

        /// <summary>
        /// 指令：移动到目标点
        /// </summary>
        public void MoveTo(Vector3 destination)
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.SetDestination(destination);
                _agent.isStopped = false;
            }
        }

        protected override void HandleDeath()
        {
            BDLogger.LogInfo($"己方单位 {gameObject.name} 阵亡");
            // 通知控制器移除该单位的选中状态
            if (UnitController.Instance != null && _isSelected)
            {
                UnitController.Instance.DeselectUnit(this);
            }
            base.HandleDeath();
        }
    }
}
