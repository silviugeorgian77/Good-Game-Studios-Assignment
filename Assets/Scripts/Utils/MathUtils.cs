using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float NormalizeValue(
        float value,
        float newStart,
        float newEnd,
        float originalStart,
        float originalEnd)
    {
        float scale = (newEnd - newStart) / (originalEnd - originalStart);
        return newStart + ((value - originalStart) * scale);
    }

    public static float ClampValue(float value, float minValue, float maxValue)
    {
        return Mathf.Max(Mathf.Min(maxValue, value), minValue);
    }
}
