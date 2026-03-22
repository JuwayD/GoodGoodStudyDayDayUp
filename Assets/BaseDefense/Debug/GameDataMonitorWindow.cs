#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Linq;
using UnityEngine;

namespace NanBeiStudy.BaseDefense.Debug
{
    /// <summary>
    /// 游戏数据监控窗口
    /// 使用 IMGUI 绘制，提供快速查看游戏运行时数据的能力
    /// 仅在开发模式（Development Build）或编辑器中可用
    /// </summary>
    public class GameDataMonitorWindow : MonoBehaviour
    {
        private const float WINDOW_WIDTH = 400f;
        private const float WINDOW_HEIGHT = 500f;
        private const float HEADER_HEIGHT = 30f;
        private const float LINE_HEIGHT = 22f;

        private GUIStyle _windowStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _valueStyle;
        private GUIStyle _searchStyle;
        private bool _stylesInitialized = false;

        private string _searchKeyword = string.Empty;
        private Vector2 _scrollPosition;
        private bool _showDetails = false;

        // 折叠状态
        private System.Collections.Generic.Dictionary<string, bool> _categoryFoldout = new System.Collections.Generic.Dictionary<string, bool>();

        private GameDataMonitor Monitor => GameDataMonitor.Instance;

        private void OnGUI()
        {
            if (!Monitor.IsWindowVisible || !GameDataMonitor.IsDevelopmentMode) return;

            InitializeStyles();

            // 防止在播放模式外绘制
            if (!Application.isPlaying) return;

            float x = Screen.width - WINDOW_WIDTH - 10f;
            float y = 10f;
            float height = Mathf.Min(WINDOW_HEIGHT, Screen.height - 20f);

            Rect windowRect = new Rect(x, y, WINDOW_WIDTH, height);
            GUI.Box(windowRect, GUIContent.none, _windowStyle);

            Rect contentRect = new Rect(x + 5, y + 5, WINDOW_WIDTH - 10, height - 10);
            GUILayout.BeginArea(contentRect);
            DrawContent();
            GUILayout.EndArea();
        }

        private void InitializeStyles()
        {
            if (_stylesInitialized) return;

            _windowStyle = new GUIStyle(GUI.skin.box);
            _windowStyle.normal.background = MakeTex(new Color(0.1f, 0.1f, 0.1f, 0.9f));

            _headerStyle = new GUIStyle(GUI.skin.label);
            _headerStyle.fontSize = 14;
            _headerStyle.fontStyle = FontStyle.Bold;
            _headerStyle.normal.textColor = Color.cyan;

            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.fontSize = 11;
            _labelStyle.normal.textColor = Color.white;

            _valueStyle = new GUIStyle(GUI.skin.label);
            _valueStyle.fontSize = 11;
            _valueStyle.normal.textColor = Color.yellow;
            _valueStyle.alignment = TextAnchor.MiddleRight;

            _searchStyle = new GUIStyle(GUI.skin.textField);
            _searchStyle.fontSize = 11;

            _stylesInitialized = true;
        }

        private Texture2D MakeTex(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private void DrawContent()
        {
            // 标题栏
            GUILayout.BeginHorizontal();
            GUILayout.Label("游戏数据监控 (F3)", _headerStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                Monitor.HideWindow();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 搜索框
            GUILayout.BeginHorizontal();
            GUILayout.Label("搜索:", GUILayout.Width(40));
            string newSearch = GUILayout.TextField(_searchKeyword, _searchStyle);
            if (newSearch != _searchKeyword)
            {
                _searchKeyword = newSearch;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 统计信息
            int totalCount = Monitor.GetTotalEntryCount();
            GUILayout.Label($"共监控 {totalCount} 个数据条目", _labelStyle);

            // 详细信息开关
            GUILayout.BeginHorizontal();
            _showDetails = GUILayout.Toggle(_showDetails, "显示详细信息");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            // 数据列表
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            if (string.IsNullOrEmpty(_searchKeyword))
            {
                DrawAllCategories();
            }
            else
            {
                DrawSearchResults();
            }

            GUILayout.EndScrollView();

            // 底部提示
            GUILayout.Space(5);
            GUILayout.Label("按 F3 键切换监控面板", _labelStyle);
        }

        private void DrawAllCategories()
        {
            foreach (var category in Monitor.GetAllCategories())
            {
                string categoryName = category.Key;
                var entries = category.Value;

                if (!_categoryFoldout.ContainsKey(categoryName))
                {
                    _categoryFoldout[categoryName] = true;
                }

                bool foldout = _categoryFoldout[categoryName];
                foldout = EditorGUILayout折叠(categoryName, foldout, entries.Count);
                _categoryFoldout[categoryName] = foldout;

                if (foldout)
                {
                    foreach (var entry in entries)
                    {
                        DrawEntry(entry);
                    }
                }
            }
        }

        private void DrawSearchResults()
        {
            var results = Monitor.SearchEntries(_searchKeyword);
            foreach (var entry in results)
            {
                DrawEntry(entry);
            }

            if (results.Count == 0)
            {
                GUILayout.Label("未找到匹配的监控数据", _labelStyle);
            }
        }

        private void DrawEntry(IDataMonitorEntry entry)
        {
            GUILayout.BeginHorizontal();

            // 名称
            GUILayout.Label(entry.Name, _labelStyle, GUILayout.Width(120));
            GUILayout.FlexibleSpace();

            // 值
            string valueStr = entry.GetValueString();
            GUILayout.Label(valueStr, _valueStyle, GUILayout.Width(150));

            GUILayout.EndHorizontal();

            // 详细信息
            if (_showDetails)
            {
                string detail = entry.GetDetailString();
                if (!string.IsNullOrEmpty(detail))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUIStyle detailStyle = new GUIStyle(GUI.skin.label);
                    detailStyle.fontSize = 10;
                    detailStyle.normal.textColor = Color.gray;
                    GUILayout.Label(detail, detailStyle);
                    GUILayout.EndHorizontal();
                }
            }
        }

        // 折叠辅助方法（使用 GUI.skin）
        private bool EditorGUILayout折叠(string title, bool isFoldout, int count)
        {
            GUILayout.BeginHorizontal();
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            string buttonText = isFoldout ? "▼" : "▶";
            bool newFoldout = GUILayout.Toggle(isFoldout, buttonText, GUI.skin.toggle, GUILayout.Width(20));
            GUILayout.Label($"{title} ({count})", _labelStyle);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = oldColor;
            return newFoldout;
        }
    }
}
#endif