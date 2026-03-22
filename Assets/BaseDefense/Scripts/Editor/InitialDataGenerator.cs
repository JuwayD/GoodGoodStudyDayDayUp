using UnityEditor;
using UnityEngine;
using NanBeiStudy.BaseDefense;

namespace NanBeiStudy.Editor
{
    public static class InitialDataGenerator
    {
        [MenuItem("BaseDefense/Generate Tower Data")]
        public static void GenerateTower()
        {
            string path = "Assets/BaseDefense/Data/Buildings/TowerData.asset";
            BuildingData data = ScriptableObject.CreateInstance<BuildingData>();
            data.BuildingName = "防御塔";
            data.Type = BuildingType.Defense;
            data.MaxHealth = 200;
            data.AttackDamage = 10;
            data.AttackRange = 5f;
            data.AttackInterval = 1f;

            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            BDLogger.LogInfo("TowerData Created!");
        }
    }
}
