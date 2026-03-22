using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    public enum BuildingType
    {
        Core,       // 主基地/核心
        Producer,   // 生产建筑（金矿、伐木场）
        Defense,    // 防御塔
        Wall        // 城墙/工事
    }

    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "BaseDefense/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Header("基础信息")]
        public string BuildingName;
        [TextArea]
        public string Description;
        public BuildingType Type;

        [Header("视觉与预制体")]
        public GameObject Prefab;
        public Sprite Icon;

        [Header("数值属性")]
        public int MaxHealth = 100;
        public int ConstructionCost = 50;
        public ResourceType CostType = ResourceType.Gold;

        [Header("生产属性 (仅限 Producer 类型)")]
        public int ProductionAmount = 10;
        public float ProductionInterval = 5f;

        [Header("战斗属性 (仅限 Defense 类型)")]
        public int AttackDamage = 10;
        public float AttackRange = 5f;
        public float AttackInterval = 1f;
    }
}
