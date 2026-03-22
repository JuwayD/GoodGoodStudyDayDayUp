using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 单位操控控制器 (UnitController)
    /// 处理鼠标交互、单位选择及下达指令
    /// </summary>
    public class UnitController : MonoBehaviour
    {
        private static UnitController _instance;
        public static UnitController Instance => _instance;

        [Header("配置")]
        public LayerMask UnitLayer;    // 单位层 (AlliedUnit)
        public LayerMask GroundLayer;  // 地面层
        public GameObject UnitPrefab;  // 用于测试生成的预制体

        private List<AlliedUnitEntity> _selectedUnits = new List<AlliedUnitEntity>();

        private void Awake()
        {
            if (_instance == null) _instance = this;
        }

        private void Update()
        {
            // 如果点击了 UI，不处理游戏内的指令
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            // 左键点击：选择单位
            if (Input.GetMouseButtonDown(0))
            {
                HandleSelection();
            }

            // 右键点击：下达指令
            if (Input.GetMouseButtonDown(1))
            {
                HandleCommand();
            }
        }

        private void HandleSelection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, UnitLayer))
            {
                AlliedUnitEntity unit = hit.collider.GetComponent<AlliedUnitEntity>();
                if (unit != null)
                {
                    // 简单逻辑：点击即替换选中（之后可做 Shift 多选）
                    ClearSelection();
                    SelectUnit(unit);
                }
            }
            else if (Physics.Raycast(ray, out RaycastHit hitGround, 1000f, GroundLayer))
            {
                // 点击地面试图取消选中
                ClearSelection();
            }
        }

        private void HandleCommand()
        {
            if (_selectedUnits.Count == 0) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GroundLayer))
            {
                BDLogger.LogInfo($"向 {_selectedUnits.Count} 个单位下达移动指令 -> {hit.point}");
                foreach (var unit in _selectedUnits)
                {
                    unit.MoveTo(hit.point);
                }
            }
        }

        public void SelectUnit(AlliedUnitEntity unit)
        {
            if (!_selectedUnits.Contains(unit))
            {
                _selectedUnits.Add(unit);
                unit.SetSelected(true);
                BDLogger.LogDetail($"选中单位: {unit.gameObject.name}");
            }
        }

        public void DeselectUnit(AlliedUnitEntity unit)
        {
            if (_selectedUnits.Contains(unit))
            {
                unit.SetSelected(false);
                _selectedUnits.Remove(unit);
            }
        }

        public void ClearSelection()
        {
            foreach (var unit in _selectedUnits)
            {
                if (unit != null) unit.SetSelected(false);
            }
            _selectedUnits.Clear();
        }

        /// <summary>
        /// 测试接口：在鼠标位置生成一个单位
        /// </summary>
        [ContextMenu("Spawn Test Unit")]
        public void SpawnTestUnit()
        {
            if (UnitPrefab == null) return;

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            // 这里简单用前向或固定位置，如果在编辑器运行，可以传参
            Vector3 spawnPos = Vector3.zero;
            Instantiate(UnitPrefab, spawnPos, Quaternion.identity);
            BDLogger.LogInfo("测试单位已生成于 Origin");
        }

        /// <summary>
        /// AI 专项：强制选择单位
        /// </summary>
        public static bool AI_SelectUnit(int instanceID)
        {
            if (Instance == null)
            {
                BDLogger.LogWarning("AI_SelectUnit 失败: UnitController.Instance 为空");
                return false;
            }
            var entities = Object.FindObjectsOfType<AlliedUnitEntity>();
            var target = entities.FirstOrDefault(e => e.gameObject.GetInstanceID() == instanceID);
            if (target != null)
            {
                Instance.ClearSelection();
                Instance.SelectUnit(target);
                return true;
            }
            BDLogger.LogWarning($"AI_SelectUnit 失败: 找不到 InstanceID 为 {instanceID} 的单位 (当前场景共有 {entities.Length} 个 AlliedUnitEntity)");
            return false;
        }

        /// <summary>
        /// AI 专项：强制移动选中单位
        /// </summary>
        public static void AI_MoveSelectedUnits(float x, float y, float z)
        {
            if (Instance == null) return;
            Vector3 target = new Vector3(x, y, z);
            Instance.HandleMoveCommand(target);
        }

        private void HandleMoveCommand(Vector3 target)
        {
            BDLogger.LogInfo($"AI 指令：向 {target} 移动");
            foreach (var unit in _selectedUnits)
            {
                unit.MoveTo(target);
            }
        }
    }
}
