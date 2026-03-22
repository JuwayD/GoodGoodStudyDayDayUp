using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 建筑预览虚影 (BuildingGhost)
    /// 负责在正式建造前显示一个对齐网格的透明/变色模型
    /// </summary>
    public class BuildingGhost : MonoBehaviour
    {
        private Material _ghostMaterial;
        private Color _validColor = new Color(0, 1, 0, 0.5f);   // 合法位置：绿色半透明
        private Color _invalidColor = new Color(1, 0, 0, 0.5f); // 非法位置：红色半透明

        private Renderer[] _renderers;
        private BuildingData _currentData;

        public void Initialize(BuildingData data)
        {
            _currentData = data;
            _renderers = GetComponentsInChildren<Renderer>();

            // 简单处理：将子物体的材质改为半透明
            // 注意：实际项目中通常会准备专门的 Ghost 材质
            foreach (var r in _renderers)
            {
                // 这里假设材质支持透明度，或者在运行时动态创建一个简单的半透明材质
                r.material.color = _validColor;
            }
        }

        public void SetPosition(Vector3 worldPos, bool isValid)
        {
            // 对齐网格
            Vector2Int gridPos = GridManager.Instance.WorldToGrid(worldPos);
            transform.position = GridManager.Instance.GridToWorld(gridPos);

            // 更新颜色反馈
            Color targetColor = isValid ? _validColor : _invalidColor;
            foreach (var r in _renderers)
            {
                r.material.color = targetColor;
            }
        }
    }
}
