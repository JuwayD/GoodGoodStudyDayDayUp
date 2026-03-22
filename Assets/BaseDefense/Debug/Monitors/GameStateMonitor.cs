using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace NanBeiStudy.BaseDefense.Debug.Monitors
{
    /// <summary>
    /// 游戏状态监控器
    /// 监控游戏运行时的基本状态（FPS、时间、场景等）
    /// </summary>
    public class GameStateMonitor : MonoBehaviour
    {
        [Header("监控设置")]
        [Tooltip("是否启用游戏状态监控")]
        public bool enableMonitor = true;

        [Header("FPS 监控")]
        [Tooltip("FPS 采样间隔（秒）")]
        public float fpsSampleInterval = 0.5f;

        [Header("其他状态更新间隔（秒）")]
        public float otherUpdateInterval = 2f;

        private List<GameStateEntry> _entries = new List<GameStateEntry>();
        private float _otherUpdateTimer = 0f;
        private float _fpsSampleTimer = 0f;
        private int _frameCount = 0;
        private float _fps = 0f;

        private void Start()
        {
            if (!GameDataMonitor.IsDevelopmentMode || !enableMonitor)
            {
                enabled = false;
                return;
            }

            InitializeEntries();
            RegisterToMonitor();

            BDLogger.LogInfo("[GameStateMonitor] 游戏状态监控器初始化完成");
        }

        private void OnDestroy()
        {
            if (!GameDataMonitor.IsDevelopmentMode) return;
            UnregisterFromMonitor();
        }

        private void Update()
        {
            if (!GameDataMonitor.IsDevelopmentMode) return;

            // FPS 计算
            _frameCount++;
            _fpsSampleTimer += Time.unscaledDeltaTime;
            if (_fpsSampleTimer >= fpsSampleInterval)
            {
                _fps = _frameCount / _fpsSampleTimer;
                _frameCount = 0;
                _fpsSampleTimer = 0f;

                foreach (var entry in _entries)
                {
                    if (entry.Name == "FPS")
                    {
                        entry.UpdateValue(Mathf.RoundToInt(_fps));
                    }
                }
            }

            // 其他条目使用独立计时器更新
            _otherUpdateTimer += Time.deltaTime;
            if (_otherUpdateTimer >= otherUpdateInterval)
            {
                _otherUpdateTimer = 0f;
                foreach (var entry in _entries)
                {
                    if (entry.Name != "FPS")
                    {
                        entry.Refresh();
                    }
                }
            }
        }

        private void InitializeEntries()
        {
            _entries.Add(new GameStateEntry("游戏状态", "FPS", () => Mathf.RoundToInt(_fps)));
            _entries.Add(new GameStateEntry("游戏状态", "时间 Scale", () => Time.timeScale, "当前 Time.timeScale 值"));
            _entries.Add(new GameStateEntry("游戏状态", "游戏时间", () => Time.time, "从游戏开始的总时间"));
            _entries.Add(new GameStateEntry("游戏状态", "实时时间", () => Time.realtimeSinceStartup, "从应用启动的总时间（不受 timeScale 影响）"));
            _entries.Add(new GameStateEntry("游戏状态", "场景名称", () => SceneManager.GetActiveScene().name));
            _entries.Add(new GameStateEntry("游戏状态", "场景索引", () => SceneManager.GetActiveScene().buildIndex));
            _entries.Add(new GameStateEntry("游戏状态", "屏幕分辨率", () => $"{Screen.width}x{Screen.height}"));
            _entries.Add(new GameStateEntry("游戏状态", "目标帧率", () => Application.targetFrameRate));
            _entries.Add(new GameStateEntry("游戏状态", "平台", () => Application.platform.ToString()));
            _entries.Add(new GameStateEntry("游戏状态", "开发模式", () => GameDataMonitor.IsDevelopmentMode ? "是" : "否"));
            _entries.Add(new GameStateEntry("游戏状态", "是否运行", () => Application.isPlaying ? "是" : "否"));
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
        /// 游戏状态监控条目
        /// </summary>
        private class GameStateEntry : DataMonitorEntry<object>
        {
            private System.Func<object> _valueGetter;

            public override string Category { get; }
            public override string Name { get; }
            public override string GetDetailString() => _detail ?? string.Empty;

            public GameStateEntry(string category, string name, System.Func<object> valueGetter, string detail = null)
            {
                Category = category;
                Name = name;
                _valueGetter = valueGetter;
                _detail = detail;
                _value = valueGetter?.Invoke();
            }

            public override string GetValueString()
            {
                return _value?.ToString() ?? "N/A";
            }

            public void Refresh()
            {
                object newValue = _valueGetter?.Invoke();
                UpdateValue(newValue);
            }
        }
    }
}