using UnityEngine;

public class CameraUtils : MonoBehaviour
{
    public static Vector2 GetCameraSize(Camera camera)
    {
        Vector2 size = new Vector2();
        size.y = 2 * camera.orthographicSize;
        size.x = size.y * camera.aspect;
        return size;
    }
}
