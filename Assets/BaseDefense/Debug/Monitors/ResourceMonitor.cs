using UnityEngine;

namespace NanBeiStudy.BaseDefense.Debug.Monitors
{
    /// <summary>
    /// 资源数据监控器
    /// 监控游戏中的资源（金币、木材、能量等）的实时数据
    /// </summary>
    public class ResourceMonitor : MonoBehaviour
    {
        [Header("监控设置")]
        [Tooltip("是否启用资源监控")]
        public bool enableMonitor = true;

        private ResourceMonitorEntry[] _entries;

        private void Start()
        {
            if (!GameDataMonitor.IsDevelopmentMode || !enableMonitor)
            {
                enabled = false;
                return;
            }

            InitializeEntries();
            RegisterToMonitor();
            SubscribeToEvents();

            BDLogger.LogInfo("[ResourceMonitor] 资源监控器初始化完成");
        }

        private void OnDestroy()
        {
            if (!GameDataMonitor.IsDevelopmentMode) return;
            UnsubscribeFromEvents();
            UnregisterFromMonitor();
        }

        private void InitializeEntries()
        {
            _entries = new ResourceMonitorEntry[]
            {
                new ResourceMonitorEntry("资源", "金币", () => ResourceManager.Instance?.GetResourceAmount(ResourceType.Gold) ?? 0),
                new ResourceMonitorEntry("资源", "木材", () => ResourceManager.Instance?.GetResourceAmount(ResourceType.Wood) ?? 0),
                new ResourceMonitorEntry("资源", "能量", () => ResourceManager.Instance?.GetResourceAmount(ResourceType.Energy) ?? 0),
            };
        }

        private void RegisterToMonitor()
        {
            if (_entries == null) return;
            foreach (var entry in _entries)
            {
                GameDataMonitor.Instance.RegisterEntry(entry);
            }
        }

        private void UnregisterFromMonitor()
        {
            if (_entries == null) return;
            foreach (var entry in _entries)
            {
                GameDataMonitor.Instance.UnregisterEntry(entry);
            }
        }

        private void SubscribeToEvents()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
            }
        }

        private void OnResourceChanged(ResourceType type, int newAmount)
        {
            foreach (var entry in _entries)
            {
                entry.Refresh();
            }
        }

        /// <summary>
        /// 资源监控条目
        /// </summary>
        private class ResourceMonitorEntry : DataMonitorEntry<int>
        {
            private System.Func<int> _valueGetter;

            public override string Category { get; }
            public override string Name { get; }

            public ResourceMonitorEntry(string category, string name, System.Func<int> valueGetter)
            {
                Category = category;
                Name = name;
                _valueGetter = valueGetter;
                _value = valueGetter?.Invoke() ?? 0;
            }

            public override string GetValueString()
            {
                return _value.ToString();
            }

            public void Refresh()
            {
                int newValue = _valueGetter?.Invoke() ?? 0;
                UpdateValue(newValue);
            }
        }
    }
}