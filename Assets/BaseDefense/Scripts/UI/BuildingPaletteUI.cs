using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 建筑调色盘 UI (BuildingPaletteUI)
    /// 提供建筑选择、刷怪控制等功能按钮
    /// </summary>
    public class BuildingPaletteUI : MonoBehaviour
    {
        [Header("建造配置")]
        public List<BuildingData> AvailableBuildings;

        [Header("UI 容器")]
        public Transform ButtonContainer;
        public GameObject ButtonPrefab;
        public TextMeshProUGUI EnemyCountText;

        [Header("UI 引用")]
        public Button SpawnEnemyBtn;
        public Button ToggleAutoSpawnBtn;
        public Button CancelBtn;

        private void Start()
        {
            // 自动寻找并绑定通用按钮
            if (SpawnEnemyBtn != null) SpawnEnemyBtn.onClick.AddListener(SpawnEnemy);
            if (ToggleAutoSpawnBtn != null) ToggleAutoSpawnBtn.onClick.AddListener(ToggleAutoSpawn);
            if (CancelBtn != null) CancelBtn.onClick.AddListener(CancelPlacement);

#if UNITY_EDITOR
            // 自动搜寻 BuildingData 并初始化建造列表 (只在编辑器环境下辅助初始化)
            if (AvailableBuildings == null || AvailableBuildings.Count == 0)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BuildingData");
                List<BuildingData> dataList = new List<BuildingData>();
                foreach (var guid in guids)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var data = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildingData>(path);
                    if (data != null && data.Type != BuildingType.Core)
                    {
                        dataList.Add(data);
                    }
                }
                // Assuming ButtonContainer has a HorizontalLayoutGroup component
                var layout = ButtonContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                if (layout != null)
                {
                    layout.childControlWidth = false;
                    layout.childControlHeight = false;
                    layout.childForceExpandWidth = false;
                    layout.spacing = 10;
                    layout.childAlignment = TextAnchor.MiddleCenter;
                }
                InitializePalette(dataList);
            }
#endif
        }

        private void Update()
        {
            if (EnemyCountText != null && EnemySpawner.Instance != null)
            {
                EnemyCountText.text = $"Enemies: {EnemySpawner.Instance.GetActiveEnemyCount()}";
            }
        }

        public void SpawnEnemy()
        {
            EnemySpawner.AI_Spawn();
            BDLogger.LogInfo("UI 触发: 生成敌人");
        }

        public void ToggleAutoSpawn()
        {
            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.AutoSpawn = !EnemySpawner.Instance.AutoSpawn;
                BDLogger.LogInfo($"UI 触发: 自动刷怪 = {EnemySpawner.Instance.AutoSpawn}");
            }
        }

        public void SelectBuilding(int index)
        {
            if (index >= 0 && index < AvailableBuildings.Count)
            {
                BuildingPlacer.Instance.SelectBuilding(AvailableBuildings[index]);
            }
        }

        public void CancelPlacement()
        {
            BuildingPlacer.Instance.CancelPlacement();
        }

        /// <summary>
        /// AI 专项：动态生成建筑按钮
        /// </summary>
        public void InitializePalette(List<BuildingData> buildings)
        {
            UnityEngine.Debug.Log($"<color=green>[BD-UI]</color> Initializing palette with {buildings.Count} buildings.");
            if (ButtonPrefab == null || ButtonContainer == null)
            {
                UnityEngine.Debug.LogWarning("<color=orange>[BD-UI]</color> ButtonPrefab or ButtonContainer is null!");
                return;
            }

            AvailableBuildings = buildings;
            // 清除旧按钮
            foreach (Transform child in ButtonContainer) Destroy(child.gameObject);

            for (int i = 0; i < AvailableBuildings.Count; i++)
            {
                int index = i;
                GameObject btnObj = Instantiate(ButtonPrefab, ButtonContainer);
                var btn = btnObj.GetComponent<Button>();
                var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();

                if (text != null) text.text = AvailableBuildings[i].BuildingName;
                if (btn != null) btn.onClick.AddListener(() => SelectBuilding(index));
                btnObj.SetActive(true);
            }
        }
    }
}
