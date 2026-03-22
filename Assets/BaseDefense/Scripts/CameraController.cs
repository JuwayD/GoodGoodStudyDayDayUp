using UnityEngine;

namespace BaseDefense
{
    /// <summary>
    /// 相机控制器，处理平移、缩放和旋转。
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("移动设置")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float ShiftMultiplier = 2f;

        [Header("旋转设置")]
        [SerializeField] private float rotationSpeed = 100f;

        [Header("缩放设置")]
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minHeight = 5f;
        [SerializeField] private float maxHeight = 40f;

        [SerializeField] private Vector3 initialPosition = new Vector3(0, 20, -15);
        [SerializeField] private Vector3 initialRotation = new Vector3(60, 0, 0);

        private Vector3 targetPosition;
        private Quaternion targetRotation;

        private void Start()
        {
            // 如果是在编辑器环境下手动调整过，这里可以作为默认值
            if (transform.position == Vector3.zero)
            {
                transform.position = initialPosition;
                transform.rotation = Quaternion.Euler(initialRotation);
            }

            targetPosition = transform.position;
            targetRotation = transform.rotation;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();

            // 平滑移动
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
            moveDir.y = 0; // 保持在水平面移动
            moveDir.Normalize();

            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? ShiftMultiplier : 1f);
            targetPosition += moveDir * speed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            if (Input.GetMouseButton(1)) // 右键旋转
            {
                float mouseX = Input.GetAxis("Mouse X");
                targetRotation *= Quaternion.Euler(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);
            }
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Vector3 zoomDir = transform.forward;
                targetPosition += zoomDir * scroll * zoomSpeed * 10f * Time.deltaTime;

                // 限制高度
                targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);
            }
        }

        /// <summary>
        /// 重置相机位置到初始状态
        /// </summary>
        public void ResetCamera(Vector3 position, Quaternion rotation)
        {
            targetPosition = position;
            targetRotation = rotation;
            transform.position = position;
            transform.rotation = rotation;
        }

        public void SetTargetPosition(Vector3 position)
        {
            targetPosition = position;
        }
    }
}
