using UnityEngine;
using System.Collections.Generic;

namespace NanBeiStudy.BaseDefense.Debug.Monitors
{
    /// <summary>
    /// 单位实体监控器
    /// 监控游戏中各类单位（友军、敌人、建筑）的数量和状态
    /// </summary>
    public class EntityMonitor : MonoBehaviour
    {
        [Header("监控设置")]
        [Tooltip("是否启用单位监控")]
        public bool enableMonitor = true;

        [Header("更新间隔（秒）")]
        public float updateInterval = 1f;

        private List<EntityCountEntry> _entries = new List<EntityCountEntry>();
        private float _updateTimer = 0f;

        private void Start()
        {
            if (!GameDataMonitor.IsDevelopmentMode || !enableMonitor)
            {
                enabled = false;
                return;
            }

            InitializeEntries();
            RegisterToMonitor();

            BDLogger.LogInfo("[EntityMonitor] 单位监控器初始化完成");
        }

        private void OnDestroy()
        {
            if (!GameDataMonitor.IsDevelopmentMode) return;
            UnregisterFromMonitor();
        }

        private void Update()
        {
            if (!GameDataMonitor.IsDevelopmentMode) return;

            _updateTimer += Time.deltaTime;
            if (_updateTimer >= updateInterval)
            {
                _updateTimer = 0f;
                RefreshAllEntries();
            }
        }

        private void InitializeEntries()
        {
            _entries.Add(new EntityCountEntry("单位", "友军单位", () => FindObjectsOfType<AlliedUnitEntity>().Length));
            _entries.Add(new EntityCountEntry("单位", "敌人单位", () => FindObjectsOfType<EnemyEntity>().Length));
            _entries.Add(new EntityCountEntry("单位", "防御塔", () => FindObjectsOfType<TowerEntity>().Length));
            _entries.Add(new EntityCountEntry("单位", "建筑", () => FindObjectsOfType<BuildingEntity>().Length));
            _entries.Add(new EntityCountEntry("单位", "可破坏物", () => FindObjectsOfType<DamageableEntity>().Length));
        }

        private void RefreshAllEntries()
        {
            foreach (var entry in _entries)
            {
                entry.Refresh();
            }
        }

        private void RegisterToMonitor()
        {
            foreach (var entry in _entries)
            {
                GameDataMonitor.Instance.RegisterEntry(entry);
            }
        }

        private void UnregisterFromMonitor()
        {
            foreach (var entry in _entries)
            {
                GameDataMonitor.Instance.UnregisterEntry(entry);
            }
        }

        /// <summary>
        /// 单位数量监控条目
        /// </summary>
        private class EntityCountEntry : DataMonitorEntry<int>
        {
            private System.Func<int> _countGetter;

            public override string Category { get; }
            public override string Name { get; }

            public EntityCountEntry(string category, string name, System.Func<int> countGetter)
            {
                Category = category;
                Name = name;
                _countGetter = countGetter;
                _value = countGetter?.Invoke() ?? 0;
            }

            public override string GetValueString()
            {
                return _value.ToString();
            }

            public void Refresh()
            {
                int newValue = _countGetter?.Invoke() ?? 0;
                UpdateValue(newValue);
            }
        }
    }
}