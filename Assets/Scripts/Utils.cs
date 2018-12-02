using UnityEngine;

public static class Utils
{
    public static float RoundDirection(float x)
    {
        float sign = Mathf.Sign(x);
        if (Mathf.Abs(sign - x) < Mathf.Abs(x))
            return sign;
        else
            return 0;
    }

    public static Vector2 RoundDirection(Vector2 v)
    {
        return new Vector2(RoundDirection(v.x), RoundDirection(v.y));
    }
}
