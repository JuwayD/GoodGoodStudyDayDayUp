using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// UI 管理器 (UIManager)
    /// 负责 Canvas、EventSystem 的基础构建以及 UI 面板的生命周期管理
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("UIManager");
                        _instance = go.AddComponent<UIManager>();
                    }
                }
                return _instance;
            }
        }

        public Canvas MainCanvas { get; private set; }
        public EventSystem MainEventSystem { get; private set; }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) { Destroy(gameObject); return; }

            DontDestroyOnLoad(gameObject);
            EnsureRequiredComponents();
        }

        public void EnsureRequiredComponents()
        {
            // 确保 Canvas 存在
            MainCanvas = FindObjectOfType<Canvas>();
            if (MainCanvas == null)
            {
                GameObject canvasGo = new GameObject("MainCanvas");
                MainCanvas = canvasGo.AddComponent<Canvas>();
                MainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<CanvasScaler>();
                canvasGo.AddComponent<GraphicRaycaster>();
            }

            // 确保 EventSystem 存在
            MainEventSystem = FindObjectOfType<EventSystem>();
            if (MainEventSystem == null)
            {
                GameObject esGo = new GameObject("EventSystem");
                MainEventSystem = esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }

            // 确保 HUD 存在
            if (MainCanvas != null && GameObject.Find("InGameHUD_Root") == null)
            {
                // 尝试从 Resource 加载（或者假设 AI 已经生成过）
                // 在本 AI 托管场景下，我们假设 UI 应该已存在。
                // 如果是运行时缺失，我们尝试触发 UIAutomationHelper（由于它是 Editor 类，此处无法直接在运行时调用）
                // 所以我们依赖预制的 Prefab 或者 Editor 阶段的 Setup。
                BDLogger.LogWarning("测到场景中缺失 InGameHUD_Root，请确保已执行过 AI_SetupUI 并保存场景。");
            }
        }

        /// <summary>
        /// AI 专项：在当前场景强制注入 UI 系统
        /// </summary>
        public static void AI_SetupUI()
        {
            var mgr = Instance;
            mgr.EnsureRequiredComponents();
            BDLogger.LogInfo("UI 框架基础环境已就绪 (Canvas & EventSystem)");
        }
    }
}
