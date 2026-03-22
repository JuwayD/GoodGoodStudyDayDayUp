using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;
using UnityEngine.AI;
using NanBeiStudy.BaseDefense;

public class FontAndNavMeshHelper : EditorWindow
{
    // Note: User requested removal of redundant menu items. 
    // I will keep the logic as static methods for AI to call via 'method' type if needed, but remove the MenuItem attribute for cleanliness.

    public static void ExecuteFix()
    {
        BakeNavMesh();
        FixFont();
        SetEnemyColor();
    }

    public static void ResetEnvironmentAndMaterials()
    {
        // 1. 重置默认材质为白色 (针对 Ground)
        var defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
        if (defaultMat != null)
        {
            defaultMat.color = Color.white;
            EditorUtility.SetDirty(defaultMat);
        }

        // 2. 确保敌人材质为红色
        string enemyMatPath = "Assets/BaseDefense/Materials/EnemyMaterial.mat";
        var enemyMat = AssetDatabase.LoadAssetAtPath<Material>(enemyMatPath);
        if (enemyMat == null)
        {
            enemyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(enemyMat, enemyMatPath);
        }
        enemyMat.color = Color.red;
        EditorUtility.SetDirty(enemyMat);

        // 3. 确保塔材质为蓝色
        string towerMatPath = "Assets/BaseDefense/Materials/TowerMaterial.mat";
        var towerMat = AssetDatabase.LoadAssetAtPath<Material>(towerMatPath);
        if (towerMat == null)
        {
            towerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(towerMat, towerMatPath);
        }
        towerMat.color = new Color(0.2f, 0.5f, 1f);
        EditorUtility.SetDirty(towerMat);

        AssetDatabase.SaveAssets();
        Debug.Log("ResetEnvironmentAndMaterials: Materials updated and saved.");
    }

    public static void SetEnemyColor()
    {
        ResetEnvironmentAndMaterials();

        // The original logic for assigning enemy material to enemies is still needed
        // after ensuring the material exists and is red.
        string matPath = "Assets/BaseDefense/Materials/EnemyMaterial.mat";
        Material enemyMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (enemyMat == null)
        {
            // This should ideally not happen if ResetEnvironmentAndMaterials was called
            // and successfully created the material. But as a fallback:
            enemyMat = new Material(Shader.Find("Standard"));
            enemyMat.color = Color.red;
            Directory.CreateDirectory(Path.GetDirectoryName(matPath));
            AssetDatabase.CreateAsset(enemyMat, matPath);
            AssetDatabase.SaveAssets();
        }

        var enemies = GameObject.FindObjectsOfType<EnemyEntity>(true);
        foreach (var e in enemies)
        {
            var r = e.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = enemyMat;
                EditorUtility.SetDirty(r);
            }
        }

        AssetDatabase.SaveAssets();
    }

    public static void BakeNavMesh()
    {
        var ground = GameObject.Find("Ground");
        if (ground != null)
        {
            GameObjectUtility.SetStaticEditorFlags(ground, StaticEditorFlags.NavigationStatic);
        }
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        Debug.Log("[BD-Utils] NavMesh Baked!");
    }

    public static void FixFont()
    {
        string ttfPath = "Assets/BaseDefense/Fonts/simhei.ttf";
        string assetPath = "Assets/BaseDefense/Fonts/simhei_SDF.asset";

        Font font = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
        if (font == null) return;

        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);

        // If the asset exists but atlas is missing, delete it to recreate
        if (fontAsset != null && (fontAsset.atlasTexture == null || fontAsset.atlasTextures.Length == 0))
        {
            AssetDatabase.DeleteAsset(assetPath);
            fontAsset = null;
        }

        if (fontAsset == null)
        {
            fontAsset = TMP_FontAsset.CreateFontAsset(font);
            AssetDatabase.CreateAsset(fontAsset, assetPath);
            AssetDatabase.SaveAssets();
        }

        if (fontAsset != null) ApplyFontToHUD(fontAsset);
    }

    public static void ApplyFontToHUD(TMP_FontAsset fontAsset)
    {
        var texts = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var t in texts)
        {
            t.font = fontAsset;
            EditorUtility.SetDirty(t);
        }
        Debug.Log("[BD-Utils] Applied font to " + texts.Length + " UI elements.");
    }
}
