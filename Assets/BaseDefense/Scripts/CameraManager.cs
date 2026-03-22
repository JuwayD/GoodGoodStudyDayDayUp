using UnityEngine;

namespace BaseDefense
{
    /// <summary>
    /// 相机管理器，单例模式。
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;
        private CameraController controller;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera != null)
            {
                controller = mainCamera.GetComponent<CameraController>();
                if (controller == null)
                {
                    controller = mainCamera.gameObject.AddComponent<CameraController>();
                }
            }
        }

        public Camera GetMainCamera() => mainCamera;
        public CameraController GetCameraController() => controller;
    }
}
