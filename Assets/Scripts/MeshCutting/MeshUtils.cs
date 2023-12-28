using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshUtils
{

    /// <summary>
    /// Find center of polygon by averaging vertices
    /// </summary>
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


    /// <summary>
    /// Reorder a list of pairs of vectors (one dimension list where i and i + 1 defines a line segment)
    /// So that it forms a closed polygon 
    /// </summary>
    public static void ReorderList(List<Vector3> pairs)
    {
        int i = 0;

        while (i < pairs.Count)
        {
            // Find next adjacent edge
            for (int j = i + 2; j < pairs.Count; j += 2)
            {
                if (AlmostEqual2sComplement(pairs[j].x, pairs[i + 1].x, 5) && AlmostEqual2sComplement(pairs[j].y, pairs[i + 1].y, 5)
                    && AlmostEqual2sComplement(pairs[j].z, pairs[i + 1].z, 5))
                {
                    // Put j at i+2
                    SwitchPairs(pairs, i + 2, j);
                    break;
                }
            }
            i += 2;
        }
    }

    public static bool AlmostEqual2sComplement(float a, float b, int maxDeltaBits)
    {
        int aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
        if (aInt < 0)
            aInt = Int32.MinValue - aInt;  // Int32.MinValue = 0x80000000

        int bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
        if (bInt < 0)
            bInt = Int32.MinValue - bInt;

        int intDiff = Math.Abs(aInt - bInt);
        return intDiff <= (1 << maxDeltaBits);
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

}
