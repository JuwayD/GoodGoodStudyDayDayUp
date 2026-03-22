using UnityEngine;
using TMPro; // 默认使用 TMP 提升视觉质量
using NanBeiStudy.BaseDefense;

namespace NanBeiStudy.UI
{
    /// <summary>
    /// 基础资源显示 UI
    /// 监听 ResourceManager 事件并实时更新界面
    /// </summary>
    public class ResourceHUD : MonoBehaviour
    {
        [Header("UI 引用")]
        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI WoodText;
        public TextMeshProUGUI EnergyText;

        private void Start()
        {
            if (ResourceManager.Instance != null)
            {
                // 注册事件
                ResourceManager.Instance.OnResourceChanged += UpdateUI;
                
                // 初始化显示
                RefreshAll();
                
                BDLogger.LogInfo("ResourceHUD 已就绪并完成事件订阅。");
            }
            else
            {
                BDLogger.LogError("ResourceHUD 找不到 ResourceManager 实例！");
            }
        }

        private void OnDestroy()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnResourceChanged -= UpdateUI;
            }
        }

        private void UpdateUI(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Gold:
                    if (GoldText) GoldText.text = amount.ToString();
                    break;
                case ResourceType.Wood:
                    if (WoodText) WoodText.text = amount.ToString();
                    break;
                case ResourceType.Energy:
                    if (EnergyText) EnergyText.text = amount.ToString();
                    break;
            }
            
            BDLogger.LogDetail($"UI 刷新: {type} -> {amount}");
        }

        private void RefreshAll()
        {
            UpdateUI(ResourceType.Gold, ResourceManager.Instance.GetResourceAmount(ResourceType.Gold));
            UpdateUI(ResourceType.Wood, ResourceManager.Instance.GetResourceAmount(ResourceType.Wood));
            UpdateUI(ResourceType.Energy, ResourceManager.Instance.GetResourceAmount(ResourceType.Energy));
        }
    }
}
