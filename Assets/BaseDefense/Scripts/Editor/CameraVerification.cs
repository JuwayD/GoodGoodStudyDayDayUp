using UnityEngine;
using UnityEditor;
using System.IO;

namespace BaseDefense.Editor
{
    /// <summary>
    /// 相机系统验证工具，符合 AI 闭环托管规范。
    /// </summary>
    public static class CameraVerification
    {
        private const string ReportDir = "Assets/VerificationReports/CameraSystem";

        [MenuItem("BaseDefense/Camera/AutoConfigure")]
        public static void AutoConfigure()
        {
            // 找 Main Camera
            GameObject camObj = GameObject.FindWithTag("MainCamera");
            if (camObj == null)
            {
                camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            // 添加控制脚本
            if (!camObj.GetComponent<CameraController>())
            {
                camObj.AddComponent<CameraController>();
            }

            // 找 GameManagers 并添加 CameraManager
            GameObject managers = GameObject.Find("GameManagers");
            if (managers == null)
            {
                managers = new GameObject("GameManagers");
            }

            if (!managers.GetComponent<CameraManager>())
            {
                managers.AddComponent<CameraManager>();
            }

            Debug.Log("[CameraVerification] Auto-configuration completed.");
        }

        [MenuItem("BaseDefense/Camera/VerifyState")]
        public static void VerifyState()
        {
            bool success = true;
            string log = "Camera System State Verification:\n";

            GameObject cam = GameObject.FindWithTag("MainCamera");
            if (cam != null && cam.GetComponent<CameraController>() != null)
            {
                log += "- MainCamera with CameraController found: OK\n";
            }
            else
            {
                log += "- MainCamera or CameraController MISSING: FAIL\n";
                success = false;
            }

            GameObject managers = GameObject.Find("GameManagers");
            if (managers != null && managers.GetComponent<CameraManager>() != null)
            {
                log += "- GameManagers with CameraManager found: OK\n";
            }
            else
            {
                log += "- GameManagers or CameraManager MISSING: FAIL\n";
                success = false;
            }

            if (success)
            {
                Debug.Log("[CameraVerification] Verification SUCCESS:\n" + log);
            }
            else
            {
                Debug.LogError("[CameraVerification] Verification FAILED:\n" + log);
            }
        }
    }
}
