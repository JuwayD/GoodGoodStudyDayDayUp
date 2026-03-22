using UnityEngine;
using NanBeiStudy.Editor;
using System.Linq;
using MCPForUnity.Editor.Helpers;
using Newtonsoft.Json.Linq;
using NanBeiStudy.BaseDefense;
using UnityEditor;

namespace MCPForUnity.Editor.Tools
{
    /// <summary>
    /// BaseDefense 专用 AI 验证工具
    /// 允许 AI 通过 MCP 接口查询游戏运行状态数据
    /// </summary>
    [McpForUnityTool("bd_tool", Description = "BaseDefense AI 托管工具。query: 查询数据; execute: 执行编辑器逻辑(参数 param 为静态方法全名 or 菜单路径)")]
    public static class BDVerificationTool
    {
        public static object HandleCommand(JObject @params)
        {
            if (@params == null)
            {
                return new ErrorResponse("参数不能为空");
            }

            var p = new ToolParams(@params);
            string action = p.Get("action"); // query | execute
            string key = p.Get("key", "");      // 对于 query 是数据类型，对于 execute 是方法类型
            if (string.IsNullOrEmpty(key) && action == "query") return new ErrorResponse("Query action requires a key");
            string param = p.Get("param", "");

            try
            {
                if (action == "execute")
                {
                    switch (key)
                    {
                        case "verify_data":
                            return VerifyData();
                        case "select_unit":
                            if (int.TryParse(param, out int id))
                            {
                                NanBeiStudy.BaseDefense.UnitController.AI_SelectUnit(id);
                                return new SuccessResponse($"Selected unit {id}");
                            }
                            return new ErrorResponse("Invalid unit id");
                        case "move_units":
                            string[] xyz = param.Split(',');
                            if (xyz.Length == 3 && float.TryParse(xyz[0], out float x) && float.TryParse(xyz[1], out float y) && float.TryParse(xyz[2], out float z))
                            {
                                NanBeiStudy.BaseDefense.UnitController.AI_MoveSelectedUnits(x, y, z);
                                return new SuccessResponse($"Moving units to {x}, {y}, {z}");
                            }
                            return new ErrorResponse("Invalid coordinates format. Use x,y,z");
                        default:
                            return ExecuteEditorLogic(key, param);
                    }
                }

                // 默认执行查询逻辑
                string result = NanBeiStudy.Editor.AIVerificationHubEditor.QueryState(key, param);
                BDLogger.LogInfo($"AI MCP Query: key={key}, param={param} -> result={result}");

                return new SuccessResponse($"查询成功: {key}", new
                {
                    key = key,
                    param = param,
                    value = result
                });
            }
            catch (System.Exception ex)
            {
                BDLogger.LogError($"工具执行异常: {ex.Message}");
                return new ErrorResponse($"工具执行异常: {ex.Message}");
            }
        }

        private static object ExecuteEditorLogic(string type, string target)
        {
            if (type == "menu")
            {
                bool success = EditorApplication.ExecuteMenuItem(target);
                return success ? new SuccessResponse($"成功执行菜单项: {target}") : new ErrorResponse($"菜单项执行失败: {target}");
            }

            if (type == "method")
            {
                // 通过反射执行静态方法 (Format: Namespace.Class.Method)
                var lastDot = target.LastIndexOf('.');
                var typeName = target.Substring(0, lastDot);
                var methodName = target.Substring(lastDot + 1);

                var targetType = System.Type.GetType(typeName);
                if (targetType == null)
                {
                    // 备选方案：手动在 Assembly-CSharp 中查找
                    foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                    {
                        targetType = assembly.GetType(typeName);
                        if (targetType != null) break;
                    }
                }

                if (targetType == null) return new ErrorResponse($"找不到类型: {typeName}");

                var method = targetType.GetMethod(methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (method == null) return new ErrorResponse($"找不到静态方法: {methodName}");

                method.Invoke(null, null);
                return new SuccessResponse($"成功调用方法: {target}");
            }

            return new ErrorResponse($"不支持的执行类型: {type}");
        }
        public static void TestPlacement()
        {
            string path = "Assets/BaseDefense/Data/Buildings/TowerData.asset";
            var towerData = AssetDatabase.LoadAssetAtPath<BuildingData>(path);
            var placer = Object.FindObjectOfType<BuildingPlacer>();
            if (towerData != null && placer != null)
            {
                placer.AI_ForcePlace(towerData, new Vector2Int(5, 5));
                Debug.Log($"TestPlacement: Forced tower placement at (5, 5) using {towerData.BuildingName}");

                // Move Camera to see both Core(0,0) and Tower(5,5)
                var camCtrl = Object.FindObjectOfType<BaseDefense.CameraController>();
                if (camCtrl != null)
                {
                    camCtrl.SetTargetPosition(new Vector3(2.5f, 20, -12.5f));
                    Debug.Log("TestPlacement: Camera focused on (2.5, 2.5)");
                }
            }
            else
            {
                Debug.LogError($"TestPlacement Failed: towerData null? {towerData == null}, BuildingPlacer null? {placer == null}");
            }
        }

        public static object VerifyData()
        {
            var report = new JObject();

            // 1. Font Check
            var texts = Object.FindObjectsOfType<TMPro.TextMeshProUGUI>(true);
            var fontIssueCount = texts.Count(t => t.font == null || t.font.name != "simhei_SDF");
            report["font_status"] = fontIssueCount == 0 ? "PASS" : $"FAIL ({fontIssueCount} issues)";

            // 2. Tower Placement Check
            var towers = Object.FindObjectsOfType<TowerEntity>();
            var originTowers = towers.Count(t => Vector3.Distance(t.transform.position, Vector3.zero) < 0.1f && t.gameObject.name != "Core_Base");
            report["tower_placement"] = originTowers == 0 ? "PASS" : $"FAIL ({originTowers} at origin)";

            var core = GameObject.Find("Core_Base");
            if (core != null)
            {
                var coreTower = core.GetComponent<TowerEntity>();
                report["core_data_status"] = (coreTower != null && coreTower.Data != null) ? "PASS" : "FAIL (No Data)";
            }

            // 3. Enemy Check
            var enemies = Object.FindObjectsOfType<EnemyEntity>();
            report["enemy_count"] = enemies.Length;
            var originEnemies = enemies.Count(e => Vector3.Distance(e.transform.position, Vector3.zero) < 0.1f);
            report["enemy_at_origin"] = originEnemies;

            // Movement Check
            if (enemies.Length > 0)
            {
                var agent = enemies[0].GetComponent<UnityEngine.AI.NavMeshAgent>();
                report["nav_mesh_active"] = (agent != null && agent.isOnNavMesh) ? "PASS" : "FAIL";
                report["has_target"] = (enemies[0].GetComponent<EnemyEntity>() != null) ? "CHECKED" : "NA";
            }

            // 4. Ground/Material Check
            var ground = GameObject.Find("Ground");
            if (ground != null)
            {
                var renderer = ground.GetComponent<MeshRenderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    report["ground_material"] = renderer.sharedMaterial.name;
                    report["ground_color"] = renderer.sharedMaterial.color.ToString();
                }
            }

            return new SuccessResponse("Verification Data Summary", report);
        }

        public static string InitScene()
        {
            // 0. 准备材质
            FontAndNavMeshHelper.ResetEnvironmentAndMaterials();

            // 1. 设置 UI
            UIAutomationHelper.SetupIngameUI();

            // 清理场景中的临时实体
            var allEntities = Object.FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mono in allEntities)
            {
                if (mono is TowerEntity || mono is EnemyEntity)
                {
                    if (Application.isPlaying) Object.Destroy(mono.gameObject);
                    else Object.DestroyImmediate(mono.gameObject);
                }
            }

            // 2. 放置核心建筑
            string corePrefabPath = "Assets/BaseDefense/Prefabs/CorePrefab.prefab";
            var corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(corePrefabPath);
            if (corePrefab != null)
            {
                var core = (GameObject)Object.Instantiate(corePrefab);
                core.transform.position = Vector3.zero;
                core.transform.rotation = Quaternion.identity;
                core.name = "Core_Base";
                core.tag = "Player";

                // 应用蓝色材质
                var towerMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/BaseDefense/Materials/TowerMaterial.mat");
                if (towerMat != null)
                {
                    foreach (var r in core.GetComponentsInChildren<Renderer>()) r.sharedMaterial = towerMat;
                }
            }

            // 3. 修复 Data 引用
            string coreDataPath = "Assets/BaseDefense/Data/Buildings/CoreData.asset";
            var coreData = AssetDatabase.LoadAssetAtPath<BuildingData>(coreDataPath);
            if (coreData != null && corePrefab != null)
            {
                coreData.Prefab = corePrefab;
                EditorUtility.SetDirty(coreData);

                var coreInstance = GameObject.Find("Core_Base")?.GetComponent<TowerEntity>();
                if (coreInstance != null)
                {
                    coreInstance.Data = coreData;
                }
            }

            // 4. 修复 TowerData 引用
            string towerDataPath = "Assets/BaseDefense/Data/Buildings/TowerData.asset";
            string towerPrefabPath = "Assets/BaseDefense/Prefabs/TowerPrefab.prefab";
            var towerData = AssetDatabase.LoadAssetAtPath<BuildingData>(towerDataPath);
            var towerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(towerPrefabPath);
            if (towerData != null && towerPrefab != null)
            {
                towerData.Prefab = towerPrefab;
                EditorUtility.SetDirty(towerData);
                BDLogger.LogInfo("TowerData Prefab Restored via Automation");
            }

            AssetDatabase.SaveAssets();
            return "Scene Initialized and Data Fixed";
        }
    }
}
