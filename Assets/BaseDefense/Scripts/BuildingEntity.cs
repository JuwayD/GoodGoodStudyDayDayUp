using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 建筑实体 (BuildingEntity)
    /// 所有建筑实例的基类，集成生命值与网格逻辑
    /// </summary>
    public class BuildingEntity : DamageableEntity
    {
        public BuildingData Data;
        private Vector2Int _gridPos;

        /// <summary>
        /// 初始化建筑
        public virtual void Initialize(BuildingData data, Vector2Int pos)
        {
            Data = data;
            _gridPos = pos;

            // 初始化生命值组件
            if (healthComponent != null)
            {
                healthComponent.Initialize(data.MaxHealth);
            }

            BDLogger.LogDetail($"建筑 {data.BuildingName} 已初始化在 {pos}，生命值: {data.MaxHealth}");
        }

        protected override void HandleDeath()
        {
            BDLogger.LogInfo($"建筑 {Data.BuildingName} 被摧毁！");

            // 从网格管理器注销
            if (GridManager.Instance != null)
            {
                GridManager.Instance.RemoveBuilding(_gridPos);
            }

            // 之后可以在这里生成残骸或播放特效
            base.HandleDeath();
        }
    }
}
