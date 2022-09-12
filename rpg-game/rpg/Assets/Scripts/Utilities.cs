using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static float MapRange(float rangeAValue, float rangeAMin, float rangeAMax, float rangeBMin, float rangeBMax)
    {
        float dist = Mathf.Abs(rangeBMax - rangeBMin);
        float per = rangeAValue / Mathf.Abs(rangeAMax - rangeAMin);
        return per * dist + rangeBMin;
    }
}
