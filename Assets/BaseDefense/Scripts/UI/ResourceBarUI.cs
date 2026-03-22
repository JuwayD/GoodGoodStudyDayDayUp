using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 资源栏 UI (ResourceBarUI)
    /// 显示金币、木材、能量等实时数值
    /// </summary>
    public class ResourceBarUI : MonoBehaviour
    {
        [Header("UI 引用")]
        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI WoodText;
        public TextMeshProUGUI EnergyText;

        private void OnEnable()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
                RefreshAll();
            }
        }

        private void Start()
        {
            RefreshAll();
        }

        private void OnDisable()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
            }
        }

        private void RefreshAll()
        {
            UpdateText(ResourceType.Gold, ResourceManager.Instance.GetResourceAmount(ResourceType.Gold));
            UpdateText(ResourceType.Wood, ResourceManager.Instance.GetResourceAmount(ResourceType.Wood));
            UpdateText(ResourceType.Energy, ResourceManager.Instance.GetResourceAmount(ResourceType.Energy));
        }

        private void HandleResourceChanged(ResourceType type, int amount)
        {
            UpdateText(type, amount);
        }

        private void UpdateText(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Gold:
                    if (GoldText) GoldText.text = $"Gold: {amount}";
                    break;
                case ResourceType.Wood:
                    if (WoodText) WoodText.text = $"Wood: {amount}";
                    break;
                case ResourceType.Energy:
                    if (EnergyText) EnergyText.text = $"Energy: {amount}";
                    break;
            }
        }

        /// <summary>
        /// AI 专项：自动尝试寻找子物体中的 Text 组件进行绑定（容错型）
        /// </summary>
        public void AutoBind()
        {
            if (GoldText == null) GoldText = transform.Find("GoldText")?.GetComponent<TextMeshProUGUI>();
            if (WoodText == null) WoodText = transform.Find("WoodText")?.GetComponent<TextMeshProUGUI>();
            if (EnergyText == null) EnergyText = transform.Find("EnergyText")?.GetComponent<TextMeshProUGUI>();
        }
    }
}
