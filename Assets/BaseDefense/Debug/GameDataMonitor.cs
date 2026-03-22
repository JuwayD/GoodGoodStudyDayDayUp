using System;
using System.Collections.Generic;
using UnityEngine;

namespace NanBeiStudy.BaseDefense.Debug
{
    /// <summary>
    /// 游戏数据监控器核心类
    /// 负责管理所有监控数据条目，提供注册/注销/查询功能
    /// 仅在开发模式（Development Build）或 Debug 模式下生效
    /// </summary>
    public class GameDataMonitor : MonoBehaviour
    {
        private static GameDataMonitor _instance;
        public static GameDataMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("[GameDataMonitor]");
                    _instance = go.AddComponent<GameDataMonitor>();
                    DontDestroyOnLoad(go);
                    BDLogger.LogInfo("[GameDataMonitor] 监控器初始化完成");
                }
                return _instance;
            }
        }

        // 开发模式标识（仅开发构建或编辑器中显示）
        public static bool IsDevelopmentMode
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return Debug.isDebugBuild;
#endif
            }
        }

        // 监控数据注册表
        private Dictionary<string, List<IDataMonitorEntry>> _entriesByCategory = new Dictionary<string, List<IDataMonitorEntry>>();
        private Dictionary<string, IDataMonitorEntry> _allEntries = new Dictionary<string, IDataMonitorEntry>();

        // 监控面板可见性
        private bool _isWindowVisible = false;
        public bool IsWindowVisible => _isWindowVisible;

        // 热键设置
        private const KeyCode TOGGLE_KEY = KeyCode.F3;

        // 事件
        public event Action<string, IDataMonitorEntry> OnEntryRegistered;
        public event Action<string, IDataMonitorEntry> OnEntryUnregistered;

        private void Update()
        {
            if (!IsDevelopmentMode) return;

            // 热键切换监控面板
            if (Input.GetKeyDown(TOGGLE_KEY))
            {
                ToggleWindow();
            }
        }

        /// <summary>
        /// 切换监控窗口可见性
        /// </summary>
        public void ToggleWindow()
        {
            _isWindowVisible = !_isWindowVisible;
            BDLogger.LogDetail($"[GameDataMonitor] 监控窗口{(_isWindowVisible ? "显示" : "隐藏")}");
        }

        /// <summary>
        /// 显示监控窗口
        /// </summary>
        public void ShowWindow()
        {
            _isWindowVisible = true;
        }

        /// <summary>
        /// 隐藏监控窗口
        /// </summary>
        public void HideWindow()
        {
            _isWindowVisible = false;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// 注册监控数据条目
        /// </summary>
        public void RegisterEntry(IDataMonitorEntry entry)
        {
            if (entry == null)
            {
                BDLogger.LogWarning("[GameDataMonitor] 尝试注册空监控条目");
                return;
            }

            string key = $"{entry.Category}/{entry.Name}";
            if (_allEntries.ContainsKey(key))
            {
                BDLogger.LogWarning($"[GameDataMonitor] 监控条目已存在: {key}");
                return;
            }

            if (!_entriesByCategory.ContainsKey(entry.Category))
            {
                _entriesByCategory[entry.Category] = new List<IDataMonitorEntry>();
            }

            _entriesByCategory[entry.Category].Add(entry);
            _allEntries[key] = entry;

            BDLogger.LogDetail($"[GameDataMonitor] 注册监控条目: {key}");
            OnEntryRegistered?.Invoke(entry.Category, entry);
        }

        /// <summary>
        /// 注销监控数据条目
        /// </summary>
        public void UnregisterEntry(IDataMonitorEntry entry)
        {
            if (entry == null) return;

            string key = $"{entry.Category}/{entry.Name}";
            if (!_allEntries.ContainsKey(key)) return;

            if (_entriesByCategory.ContainsKey(entry.Category))
            {
                _entriesByCategory[entry.Category].Remove(entry);
            }

            _allEntries.Remove(key);
            BDLogger.LogDetail($"[GameDataMonitor] 注销监控条目: {key}");
            OnEntryUnregistered?.Invoke(entry.Category, entry);
        }

        /// <summary>
        /// 根据分类获取所有条目
        /// </summary>
        public IEnumerable<KeyValuePair<string, List<IDataMonitorEntry>>> GetAllCategories()
        {
            return _entriesByCategory;
        }

        /// <summary>
        /// 获取所有条目数量
        /// </summary>
        public int GetTotalEntryCount()
        {
            return _allEntries.Count;
        }

        /// <summary>
        /// 根据关键字搜索条目
        /// </summary>
        public List<IDataMonitorEntry> SearchEntries(string keyword)
        {
            var results = new List<IDataMonitorEntry>();
            if (string.IsNullOrEmpty(keyword)) return results;

            keyword = keyword.ToLower();
            foreach (var entry in _allEntries.Values)
            {
                if (entry.Category.ToLower().Contains(keyword) ||
                    entry.Name.ToLower().Contains(keyword) ||
                    entry.GetValueString().ToLower().Contains(keyword))
                {
                    results.Add(entry);
                }
            }
            return results;
        }
    }
}