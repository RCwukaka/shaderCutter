using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMapRendering
{
    static Color[] colorDatas;
    public static Color[] CalculateModelVerticeColors(float[] physicsDatas)
    {
        colorDatas = new Color[physicsDatas.Length];
        float[] hueColorH = new float[physicsDatas.Length];
        float max = physicsDatas.GetMax();
        //当有限元网格结点值全为0时，颜色全为蓝
        if (max == 0)
        {
            for (int i = 0; i < physicsDatas.Length; i++)
            {
                colorDatas[i] = Color.blue;
            }
            return colorDatas;
        }
        float min = physicsDatas.GetMin();
        float range = max - min;
        for (int i = 0; i < physicsDatas.Length; i++)
        {
            hueColorH[i] = 2.0f / 3 * (max - physicsDatas[i]) / range;
            colorDatas[i] = Color.HSVToRGB(hueColorH[i], 1, 1);
        }
        return colorDatas;
    }
}
public static class ArrayExtension
{
    public static float GetMax(this float[] vs)
    {
        float max = 0;
        for (int i = 0; i < vs.Length; i++)
        {
            if (vs[i] > max)
            {
                max = vs[i];
            }
        }
        return max;
    }
    public static float GetMin(this float[] vs)
    {
        float min = 0;
        for (int i = 0; i < vs.Length; i++)
        {
            if (vs[i] < min)
            {
                min = vs[i];
            }
        }
        return min;
    }
}

