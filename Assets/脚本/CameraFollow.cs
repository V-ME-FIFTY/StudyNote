using UnityEngine;

public class camerafollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    public Vector3 offset;
    public float dampingFactor = 0.8f;
    public float inertiaFactor = 0.1f; // 惯性因子

    void Update()
    {
        if (target == null)
            return;

        // 计算目标位置
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        // 先应用惯性
        Vector3 newTargetPosition = targetPosition + velocity * inertiaFactor;

        // 使用平滑阻尼来移动摄像机，并应用阻尼因子
        transform.position = Vector3.SmoothDamp(transform.position, newTargetPosition, ref velocity, smoothTime);
        transform.position = Vector3.Lerp(transform.position, newTargetPosition, dampingFactor);
    }
}