using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NanBeiStudy.BaseDefense;

namespace NanBeiStudy.Editor
{
    /// <summary>
    /// AI 验证中心 (Editor 版)
    /// 仅在编辑器环境下运行，不进入构建包。
    /// 为 AI 提供精简的、参数化的数据查询接口。
    /// </summary>
    public static class AIVerificationHubEditor
    {
        public static string QueryState(string key, string param = "")
        {
            switch (key.ToLower())
            {
                case "res": return GetResourceState(param);
                case "count": return GetEntityCount(param).ToString();
                case "grid": return GetGridState(param);
                case "spawn":
                    EnemySpawner.AI_Spawn();
                    return "Spawn Requested";
                case "core_hp":
                    return GetCoreHealth();
                case "entity_hp":
                    return GetEntityHealth(param);
                case "allied_selected":
                    return GetSelectedAlliedCount().ToString();
                default: return "Unknown Query Key";
            }
        }

        private static string GetResourceState(string resourceName)
        {
            var instance = Object.FindObjectOfType<ResourceManager>();
            if (instance == null) return "ResourceManager Not Found";

            if (System.Enum.TryParse(resourceName, true, out ResourceType type))
            {
                int amount = instance.GetResourceAmount(type);
                return amount.ToString();
            }
            return "Invalid Resource Type";
        }

        private static int GetEntityCount(string typeName)
        {
            if (typeName.ToLower() == "enemy")
            {
                var spawner = Object.FindObjectOfType<EnemySpawner>();
                return spawner != null ? spawner.GetActiveEnemyCount() : 0;
            }

            if (typeName.ToLower() == "building")
            {
                return Object.FindObjectsOfType<BuildingEntity>().Length;
            }

            if (typeName.ToLower() == "allied")
            {
                return Object.FindObjectsOfType<AlliedUnitEntity>().Length;
            }
            return 0;
        }

        private static string GetGridState(string param)
        {
            var grid = Object.FindObjectOfType<GridManager>();
            if (grid == null) return "GridManager Not Found";

            string[] split = param.Split(',');
            if (split.Length != 2) return "Invalid Format";

            if (int.TryParse(split[0], out int x) && int.TryParse(split[1], out int z))
            {
                Vector2Int pos = new Vector2Int(x, z);
                bool occupied = grid.IsOccupied(pos);
                if (occupied)
                {
                    var building = grid.GetBuildingAt(pos);
                    return $"Occupied:True, Building:{(building != null ? building.Data.BuildingName : "Unknown")}";
                }
                return "Occupied:False";
            }
            return "Parse Error";
        }

        private static string GetEntityHealth(string typeName)
        {
            if (typeName.ToLower() == "enemy")
            {
                var enemy = Object.FindObjectOfType<EnemyEntity>();
                if (enemy != null)
                {
                    var health = enemy.GetComponent<HealthComponent>();
                    return health != null ? $"{health.CurrentHealth}/{health.MaxHealth}" : "No HealthComp";
                }
                return "No Enemy";
            }
            return "Type Not Supported";
        }

        private static string GetCoreHealth()
        {
            BuildingEntity[] buildings = Object.FindObjectsOfType<BuildingEntity>();
            foreach (var b in buildings)
            {
                if (b.Data != null && b.Data.Type == BuildingType.Core)
                {
                    var health = b.GetComponent<HealthComponent>();
                    return health != null ? $"{health.CurrentHealth}/{health.MaxHealth}" : "No HealthComp";
                }
            }
            return "Core Not Found";
        }

        private static int GetSelectedAlliedCount()
        {
            var alliedUnits = Object.FindObjectsOfType<AlliedUnitEntity>();
            int count = 0;
            foreach (var unit in alliedUnits)
            {
                // 注意：这里需要反射或直接访问 _isSelected。
                // 为了简单，我们给 AlliedUnitEntity 增加一个公共属性 IsSelected
                var prop = unit.GetType().GetProperty("IsSelected");
                if (prop != null)
                {
                    if ((bool)prop.GetValue(unit)) count++;
                }
            }
            return count;
        }
    }
}
