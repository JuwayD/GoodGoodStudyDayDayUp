using System.Collections.Generic;
using UnityEngine;

namespace NanBeiStudy.BaseDefense
{
    /// <summary>
    /// 敌人生成器 (EnemySpawner)
    /// 负责定时生成敌人并管理波次
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        private static EnemySpawner _instance;
        public static EnemySpawner Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<EnemySpawner>();
                return _instance;
            }
        }

        [Header("生成设置")]
        public GameObject EnemyPrefab;
        public Transform SpawnPoint;
        public float SpawnInterval = 5f;
        public bool AutoSpawn = false;

        private List<EnemyEntity> _activeEnemies = new List<EnemyEntity>();
        private float _nextSpawnTime;

        private void Awake()
        {
            if (_instance == null) _instance = this;
        }

        private void Update()
        {
            if (AutoSpawn && Time.time >= _nextSpawnTime)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + SpawnInterval;
            }

            // 清理已死亡的敌人引用
            _activeEnemies.RemoveAll(e => e == null || e.IsDead);
        }

        public void SpawnEnemy()
        {
            if (EnemyPrefab == null || SpawnPoint == null)
            {
                BDLogger.LogError("Spawn failed: Prefab or SpawnPoint is null.");
                return;
            }

            var go = Object.Instantiate(EnemyPrefab, SpawnPoint.position, Quaternion.identity);
            var enemy = go.GetComponent<EnemyEntity>();

            // 强制应用红色材质 (通过内部引用或动态查找，避免使用 UnityEditor)
            var renderer = go.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                // 如果是默认材质，则变红以示区分
                if (renderer.sharedMaterial.name.Contains("Default"))
                {
                    renderer.material.color = Color.red;
                }
            }
            // Safety: Force position again in case of prefab issues or weird parent logic
            go.transform.position = SpawnPoint.position;

            if (enemy != null)
            {
                FindTargetForEnemy(enemy);
                _activeEnemies.Add(enemy);
                var agent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
                bool isOnNav = agent != null && agent.isOnNavMesh;
                BDLogger.LogInfo($"新敌人 @ {go.transform.position} 已生成！(isOnNavMesh: {isOnNav})");
            }
            else
            {
                BDLogger.LogError("Spawned object missing EnemyEntity component!");
            }
        }

        public static void AI_Spawn()
        {
            if (Instance != null) Instance.SpawnEnemy();
        }

        private void FindTargetForEnemy(EnemyEntity enemy)
        {
            // 简单逻辑：寻找所有 BuildingEntity 中类型为 Core 的
            BuildingEntity[] buildings = FindObjectsOfType<BuildingEntity>();
            foreach (var b in buildings)
            {
                if (b.Data != null && b.Data.Type == BuildingType.Core)
                {
                    enemy.SetTarget(b.transform);
                    return;
                }
            }
        }

        /// <summary>
        /// 获取当前存活敌人数量 (用于 AI 验证)
        /// </summary>
        public int GetActiveEnemyCount()
        {
            return _activeEnemies.Count;
        }

        [ContextMenu("Spawn One Enemy")]
        public void ManualSpawn() => SpawnEnemy();
    }
}
