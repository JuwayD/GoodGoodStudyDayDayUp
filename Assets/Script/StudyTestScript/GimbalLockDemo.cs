using UnityEngine;

/// <summary>
/// 复现“欧拉角万向锁症状”的最小脚本：
/// - 故意用 transform.eulerAngles 读写并累加（这是坑点）
/// - 当 Pitch 接近 90° 时，Yaw/Roll 会出现耦合、表现异常
/// 控制：
///   W/S : Pitch +/-
///   A/D : Yaw +/-
///   Q/E : Roll +/-
///   R   : 重置
/// </summary>
public class GimbalLockDemo : MonoBehaviour
{
    [Header("Degrees per second")]
    public float pitchSpeed = 60f;
    public float yawSpeed   = 60f;
    public float rollSpeed  = 60f;

    // 我们自己维护一套欧拉角（避免每帧从Unity“挑选的那组欧拉解”里读回导致混乱更大）
    private float pitch; // X
    private float yaw;   // Y
    private float roll;  // Z

    void Start()
    {
        var e = transform.eulerAngles;
        pitch = e.x;
        yaw   = e.y;
        roll  = e.z;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // 输入：刻意用欧拉角累加（gimbal lock 最常见的“工程错误用法”）
        if (Input.GetKey(KeyCode.W)) pitch += pitchSpeed * dt;
        if (Input.GetKey(KeyCode.S)) pitch -= pitchSpeed * dt;

        if (Input.GetKey(KeyCode.D)) yaw   += yawSpeed * dt;
        if (Input.GetKey(KeyCode.A)) yaw   -= yawSpeed * dt;

        if (Input.GetKey(KeyCode.E)) roll  += rollSpeed * dt;
        if (Input.GetKey(KeyCode.Q)) roll  -= rollSpeed * dt;

        if (Input.GetKeyDown(KeyCode.R))
        {
            pitch = yaw = roll = 0f;
        }

        // 关键：把 pitch 夹到接近 90°，但不让它轻易跨过去（跨过去会触发欧拉解跳变，更难观察）
        pitch = Mathf.Clamp(pitch, -89.9f, 89.9f);

        // 直接用欧拉角写回（就是“会锁”的那条路）
        transform.rotation = Quaternion.Euler(pitch, yaw, roll);

        // 提示：当 pitch 接近 ±90°，你会发现：
        // - A/D（Yaw）和 Q/E（Roll）开始变得像在做同一件事
        // - 某些方向变得不直观，甚至像“反着来”
    }

    void OnGUI()
    {
        GUILayout.Label("GimbalLockDemo");
        GUILayout.Label("W/S: Pitch  A/D: Yaw  Q/E: Roll  R: Reset");
        GUILayout.Label($"Pitch(X)={pitch:F1}  Yaw(Y)={yaw:F1}  Roll(Z)={roll:F1}");
        GUILayout.Label("把 Pitch 推到接近 89°，再分别按 A/D 与 Q/E，观察两者逐渐耦合。");
    }
}