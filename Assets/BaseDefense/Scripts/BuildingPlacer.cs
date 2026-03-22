using UnityEngine;
using UnityEngine.EventSystems;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 建筑放置逻辑 (BuildingPlacer)
    /// 负责处理玩家输入、调用网格检测、执行资源扣除并生成实体
    /// </summary>
    public class BuildingPlacer : MonoBehaviour
    {
        private static BuildingPlacer _instance;
        public static BuildingPlacer Instance => _instance;

        [Header("建造配置")]
        public LayerMask GroundLayer; // 地面层用于射线检测
        public BuildingData SelectedBuilding; // 当前选中的建筑数据

        private GameObject _ghostObj;
        private BuildingGhost _ghostScript;

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            // 如果没有选中建筑，不执行逻辑
            if (SelectedBuilding == null) return;

            UpdateGhost();

            // 检测点击建造
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                TryPlace();
            }

            // 取消建造
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }

        /// <summary>
        /// 选择要建造的建筑（由 UI 调用）
        /// </summary>
        public void SelectBuilding(BuildingData data)
        {
            SelectedBuilding = data;
            PrepareGhost();
            BDLogger.LogInfo($"AI 切换建造目标: {data.BuildingName}");
        }

        private void PrepareGhost()
        {
            if (_ghostObj != null) Destroy(_ghostObj);

            // 实例化预览物（通常直接实例化建筑 Prefab，然后挂载 Ghost 脚本）
            _ghostObj = Instantiate(SelectedBuilding.Prefab);
            _ghostScript = _ghostObj.AddComponent<BuildingGhost>();
            _ghostScript.Initialize(SelectedBuilding);

            // 移除预览物上的功能性脚本以防报错
            var buildingEntity = _ghostObj.GetComponent<BuildingEntity>();
            if (buildingEntity) Destroy(buildingEntity);
        }

        private void UpdateGhost()
        {
            if (_ghostScript == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GroundLayer))
            {
                Vector2Int gridPos = GridManager.Instance.WorldToGrid(hit.point);
                bool isValid = !GridManager.Instance.IsOccupied(gridPos) &&
                               ResourceManager.Instance.HasEnoughResource(SelectedBuilding.CostType, SelectedBuilding.ConstructionCost);

                _ghostScript.SetPosition(hit.point, isValid);
                _ghostObj.SetActive(true);
            }
            else
            {
                _ghostObj.SetActive(false);
            }
        }

        private void TryPlace()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GroundLayer))
            {
                Vector2Int gridPos = GridManager.Instance.WorldToGrid(hit.point);

                // 检查资源
                if (!ResourceManager.Instance.HasEnoughResource(SelectedBuilding.CostType, SelectedBuilding.ConstructionCost))
                {
                    BDLogger.LogWarning("建造失败：资源不足！");
                    return;
                }

                // 检查占用
                if (GridManager.Instance.IsOccupied(gridPos))
                {
                    BDLogger.LogWarning("建造失败：该位置已被占用！");
                    return;
                }

                // 执行建造
                ExecutePlacement(gridPos);
            }
        }

        private void ExecutePlacement(Vector2Int gridPos)
        {
            // 扣除资源
            ResourceManager.Instance.ConsumeResource(SelectedBuilding.CostType, SelectedBuilding.ConstructionCost);

            // 生成实体并设置位置
            Vector3 worldPos = GridManager.Instance.GridToWorld(gridPos);
            GameObject buildingObj = Instantiate(SelectedBuilding.Prefab, worldPos, Quaternion.identity);
            BuildingEntity entity = buildingObj.GetComponent<BuildingEntity>();

            // 如果预制体没挂脚本，自动补一个（容错）
            if (entity == null) entity = buildingObj.AddComponent<BuildingEntity>();

            entity.Initialize(SelectedBuilding, gridPos);

            // 注册到网格
            GridManager.Instance.TryPlaceBuilding(gridPos, entity);

            BDLogger.LogInfo($"[BUILD] 成功建造 {SelectedBuilding.BuildingName} 于 {gridPos} (世界坐标: {worldPos})");
        }

        public void CancelPlacement()
        {
            SelectedBuilding = null;
            if (_ghostObj != null) Destroy(_ghostObj);
            BDLogger.LogInfo("已取消建造模式。");
        }

        /// <summary>
        /// AI 专项：强行放置接口（用于无人值守验证）
        /// </summary>
        public void AI_ForcePlace(BuildingData data, Vector2Int gridPos)
        {
            SelectedBuilding = data;
            ExecutePlacement(gridPos);
        }
    }
}
