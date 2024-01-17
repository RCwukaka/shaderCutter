using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class MeshUtils
{

    public static Vector3 FindCenter(List<Vector3> pairs)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        for (int i = 0; i < pairs.Count; i += 2)
        {
            center += pairs[i];
            count++;
        }

        return center / count;
    }

    public static void ReorderList(List<Vector3> pairs, List<Vector2> uvs)
    {
        int i = 0;

        while (i < pairs.Count)
        {
            //找寻长度为一的线段
            for (int j = i + 2; j < pairs.Count; j += 2)
            {
                if (AlmostEqual2sComplement(pairs[j].x, pairs[j + 1].x) && AlmostEqual2sComplement(pairs[j].y, pairs[j + 1].y)
                    && AlmostEqual2sComplement(pairs[j].z, pairs[j + 1].z) && AlmostEqual2sComplement(pairs[j].x, pairs[i + 1].x) && AlmostEqual2sComplement(pairs[j].y, pairs[i + 1].y)
                    && AlmostEqual2sComplement(pairs[j].z, pairs[i + 1].z))
                {
                    SwitchPairs(pairs, i + 2, j);
                    SwitchUVs(uvs, i + 2, j);
                    i += 2;
                }
            }
            //找寻长度不为一的线段
            for (int j = i + 2; j < pairs.Count; j += 2)
            {
                if (AlmostEqual2sComplement(pairs[j].x, pairs[i + 1].x) && AlmostEqual2sComplement(pairs[j].y, pairs[i + 1].y)
                    && AlmostEqual2sComplement(pairs[j].z, pairs[i + 1].z))
                {
                    SwitchPairs(pairs, i + 2, j);
                    SwitchUVs(uvs, i + 2, j);
                    break;
                }
            }
            i += 2;
        }

    }

    public static List<Vector3> OffsetToPoint(List<Vector3> pairs, Vector3 point)
    {
        int i = 0;
        List<Vector3> mid = new List<Vector3>();
        while (i < pairs.Count)
        {
            mid.Add(pairs[i] - point);
            i++;
        }
        return mid;
    }

    public static List<Vector3> GetUsefulPoint(List<Vector3> pairs)
    {
        List<Vector3> mid = new List<Vector3>();
        int i = 0;
        while (i < pairs.Count)
        {
            mid.Add(pairs[i]);
            i += 2;
        }
        return mid;
    }

    public static List<Vector2> GetUsefulUV(List<Vector2> uvs)
    {
        List<Vector2> mid = new List<Vector2>();
        int i = 0;
        while (i < uvs.Count)
        {
            mid.Add(uvs[i]);
            i += 2;
        }
        return mid;
    }

    public static bool AlmostEqual2sComplement(float a, float b)
    {
        float x = a - b;
        if (x >= -0.0001 && x <= 0.0001)
            return true;
        else
            return false;
    }

    public static float AlmostNearComplement(Plane plane, Vector3 point)
    {
        float distance = plane.GetDistanceToPoint(point);
        if (distance <= float.Epsilon && distance >= -float.Epsilon)
            return 0;
        else
            return distance;
    }

    private static void SwitchPairs(List<Vector3> pairs, int pos1, int pos2)
    {
        if (pos1 == pos2) return;

        Vector3 temp1 = pairs[pos1];
        Vector3 temp2 = pairs[pos1 + 1];
        pairs[pos1] = pairs[pos2];
        pairs[pos1 + 1] = pairs[pos2 + 1];
        pairs[pos2] = temp1;
        pairs[pos2 + 1] = temp2;
    }

    private static void SwitchUVs(List<Vector2> uvs, int pos1, int pos2)
    {
        if (pos1 == pos2) return;

        Vector2 temp1 = uvs[pos1];
        Vector2 temp2 = uvs[pos1 + 1];
        uvs[pos1] = uvs[pos2];
        uvs[pos1 + 1] = uvs[pos2 + 1];
        uvs[pos2] = temp1;
        uvs[pos2 + 1] = temp2;
    }

}
