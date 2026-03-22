using System.Collections.Generic;
using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 网格管理器 (GridManager)
    /// 负责世界坐标与网格坐标转换，以及记录建筑占用情况
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        private static GridManager _instance;
        public static GridManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<GridManager>();
                return _instance;
            }
        }

        public float CellSize = 1.0f;
        public Vector2 GridOrigin = Vector2.zero;

        private Dictionary<Vector2Int, BuildingEntity> _occupiedCells = new Dictionary<Vector2Int, BuildingEntity>();

        private void Awake()
        {
            if (_instance == null) _instance = this;
        }

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt((worldPos.x - GridOrigin.x) / CellSize);
            int z = Mathf.FloorToInt((worldPos.z - GridOrigin.y) / CellSize);
            return new Vector2Int(x, z);
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            float x = gridPos.x * CellSize + GridOrigin.x + (CellSize * 0.5f);
            float z = gridPos.y * CellSize + GridOrigin.y + (CellSize * 0.5f);
            return new Vector3(x, 0, z);
        }

        public bool IsOccupied(Vector2Int gridPos)
        {
            return _occupiedCells.ContainsKey(gridPos);
        }

        public bool TryPlaceBuilding(Vector2Int gridPos, BuildingEntity building)
        {
            if (IsOccupied(gridPos)) return false;
            _occupiedCells[gridPos] = building;
            return true;
        }

        public void RemoveBuilding(Vector2Int gridPos)
        {
            if (_occupiedCells.ContainsKey(gridPos))
            {
                _occupiedCells.Remove(gridPos);
            }
        }

        public BuildingEntity GetBuildingAt(Vector2Int gridPos)
        {
            return _occupiedCells.ContainsKey(gridPos) ? _occupiedCells[gridPos] : null;
        }
    }
}
