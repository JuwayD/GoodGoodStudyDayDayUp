using UnityEngine;
using UnityEditor;
using NanBeiStudy.BaseDefense;
using System.Collections.Generic;
using System.Linq;

namespace NanBeiStudy.Editor
{
    /// <summary>
    /// UI 自动化注入工具
    /// 允许 AI 在不打开 Prefab 编辑器的情况下，通过静态方法在场景中构建 UGUI 结构
    /// </summary>
    public static class UIAutomationHelper
    {
        public static void SetupIngameUI()
        {
            // 如果已存在，先清理
            var existing = GameObject.Find("InGameHUD_Root");
            if (existing != null) Object.DestroyImmediate(existing);

            UIManager.AI_SetupUI();
            // 清理旧资源
            var oldRoot = GameObject.Find("InGameHUD_Root");
            if (oldRoot != null) Object.DestroyImmediate(oldRoot);

            var canvas = GameObject.Find("MainCanvas");

            // 创建根节点
            GameObject uiRoot = new GameObject("InGameHUD_Root", typeof(RectTransform));
            uiRoot.transform.SetParent(canvas.transform, false);
            var rootRect = uiRoot.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;

            // 1. 创建 ResourceBar
            CreateResourceBar(uiRoot.transform);

            // 2. 创建 BuildingPalette
            CreateBuildingPalette(uiRoot.transform);

            // 3. 调整相机为俯视角
            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(0, 25, -20);
                cam.transform.rotation = Quaternion.Euler(55, 0, 0);
            }

            BDLogger.LogInfo("InGameHUD 已自动化生成，相机已调整为俯视角。");
        }

        private static void CreateResourceBar(Transform parent)
        {
            GameObject barGo = new GameObject("ResourceBar", typeof(RectTransform), typeof(ResourceBarUI));
            barGo.transform.SetParent(parent, false);
            var rect = barGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -20);
            rect.sizeDelta = new Vector2(0, 50);

            // 简单背景
            var img = barGo.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0, 0, 0, 0.5f);

            // 创建文本 (需要 TMPro)
            var gold = CreateText(barGo.transform, "GoldText", "Gold: 0", new Vector2(-200, 0));
            var wood = CreateText(barGo.transform, "WoodText", "Wood: 0", new Vector2(0, 0));
            var energy = CreateText(barGo.transform, "EnergyText", "Energy: 0", new Vector2(200, 0));

            var script = barGo.GetComponent<ResourceBarUI>();
            script.GoldText = gold;
            script.WoodText = wood;
            script.EnergyText = energy;
        }

        private static void CreateBuildingPalette(Transform parent)
        {
            GameObject paletteGo = new GameObject("BuildingPalette", typeof(RectTransform), typeof(BuildingPaletteUI));
            paletteGo.transform.SetParent(parent, false);
            var rect = paletteGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 20);
            rect.sizeDelta = new Vector2(600, 100);

            var img = paletteGo.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0, 0, 0, 0.5f);

            // 敌人计数
            var count = CreateText(paletteGo.transform, "EnemyCount", "Enemies: 0", new Vector2(0, 40));

            // 操作按钮：Spawn Enemy
            var spawnBtn = CreateActionButton(paletteGo.transform, "SpawnBtn", "Spawn Enemy", new Vector2(-200, 40));

            // 创建建筑列表容器 (带布局组件)
            GameObject containerGo = new GameObject("ButtonContainer", typeof(RectTransform), typeof(UnityEngine.UI.HorizontalLayoutGroup));
            containerGo.transform.SetParent(paletteGo.transform, false);
            var layout = containerGo.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            var containerRect = containerGo.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(400, 50);
            containerRect.anchoredPosition = new Vector2(0, 0);

            var script = paletteGo.GetComponent<BuildingPaletteUI>();
            script.ButtonContainer = containerRect;

            // 创建一个简单的 Button Prefab 供运行时实例化建筑按钮
            var btnGo = new GameObject("BuildingButton_Template", typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
            btnGo.transform.SetParent(parent, false);
            btnGo.SetActive(false); // 隐藏模版
            var bRect = btnGo.GetComponent<RectTransform>();
            bRect.sizeDelta = new Vector2(80, 30);
            var bImg = btnGo.GetComponent<UnityEngine.UI.Image>();
            bImg.color = Color.white; // 设置为白色底色

            var bTxt = CreateText(btnGo.transform, "Text", "Building", Vector2.zero);
            bTxt.color = Color.black;
            bTxt.fontSize = 14;
            bTxt.alignment = TMPro.TextAlignmentOptions.Center;
            bTxt.rectTransform.anchorMin = Vector2.zero;
            bTxt.rectTransform.anchorMax = Vector2.one;
            bTxt.rectTransform.offsetMin = Vector2.zero;
            bTxt.rectTransform.offsetMax = Vector2.zero;

            script.EnemyCountText = count;
            script.SpawnEnemyBtn = spawnBtn;
            script.ButtonPrefab = btnGo;
        }

        private static TMPro.TextMeshProUGUI CreateText(Transform parent, string name, string content, Vector2 pos)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(TMPro.TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(200, 50); // 给文字一个合理的尺寸空间

            var text = go.GetComponent<TMPro.TextMeshProUGUI>();
            text.text = content;
            text.fontSize = 24;
            text.color = Color.white; // 强制设为白色对冲背景
            text.alignment = TMPro.TextAlignmentOptions.Center;

            // 重要：在编辑器下查找默认字体，否则脚本创建的 TMP 默认没有字体资产
            string[] guides = AssetDatabase.FindAssets("t:TMP_FontAsset LiberationSans SDF");
            if (guides.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guides[0]);
                text.font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(path);
            }
            else
            {
                // 备选方案：尝试找任何一个 TMP 字体资产
                guides = AssetDatabase.FindAssets("t:TMP_FontAsset");
                if (guides.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guides[0]);
                    text.font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(path);
                }
            }

            return text;
        }

        private static UnityEngine.UI.Button CreateActionButton(Transform parent, string name, string label, Vector2 pos)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 40);
            rect.anchoredPosition = pos;

            var btn = go.GetComponent<UnityEngine.UI.Button>();

            var txt = CreateText(go.transform, "Text", label, Vector2.zero);
            txt.color = Color.black;
            txt.fontSize = 20;

            // 确保文字填满按钮
            txt.rectTransform.anchorMin = Vector2.zero;
            txt.rectTransform.anchorMax = Vector2.one;
            txt.rectTransform.sizeDelta = Vector2.zero;

            return btn;
        }
    }
}
