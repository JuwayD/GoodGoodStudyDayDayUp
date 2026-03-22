using System.Collections.Generic;
using UnityEngine;
using System;

namespace NanBeiStudy.BaseDefense
{
    public enum ResourceType { Gold, Wood, Energy }

    /// <summary>
    /// 资源管理器 (ResourceManager)
    /// 负责游戏中各种数值资源（金币、木材、能量）的状态维护
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        private static ResourceManager _instance;
        public static ResourceManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<ResourceManager>();
                return _instance;
            }
        }

        [Serializable]
        public struct ResourceEntry
        {
            public ResourceType Type;
            public int Amount;
        }

        [Header("初始资源设置")]
        public List<ResourceEntry> InitialResources;

        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        // 事件：资源变化时触发
        public event Action<ResourceType, int> OnResourceChanged;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            InitializeResources();
        }

        private void InitializeResources()
        {
            foreach (var entry in InitialResources)
            {
                _resources[entry.Type] = entry.Amount;
                OnResourceChanged?.Invoke(entry.Type, entry.Amount);
            }
        }

        public int GetResourceAmount(ResourceType type)
        {
            return _resources.ContainsKey(type) ? _resources[type] : 0;
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (!_resources.ContainsKey(type)) _resources[type] = 0;
            _resources[type] += amount;
            OnResourceChanged?.Invoke(type, _resources[type]);
            BDLogger.LogDetail($"资源采集: {type} +{amount}, 当前: {_resources[type]}");
        }

        public bool HasEnoughResource(ResourceType type, int amount)
        {
            return GetResourceAmount(type) >= amount;
        }

        public bool ConsumeResource(ResourceType type, int amount)
        {
            if (HasEnoughResource(type, amount))
            {
                _resources[type] -= amount;
                OnResourceChanged?.Invoke(type, _resources[type]);
                BDLogger.LogDetail($"资源消耗: {type} -{amount}, 剩余: {_resources[type]}");
                return true;
            }
            return false;
        }
    }
}
