using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CloudMapRendering
{
    public static Mesh CalculateModelUVS(Mesh mesh, Transform transform, List<List<float>> infos)
    {
        float[] stress = getStressInfo(infos);
        float max_stress = stress.GetMax();
        float min_stress = 0;
        float range_stress = max_stress - min_stress;

        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[mesh.uv.Length];
        for (int i = 0; i < infos.Count; i++)
        {
            for (int j = 0; j < vertices.Length; j++)
            {
                uvs[j] = new Vector2(CaculateUV(transform.TransformPoint(vertices[j]), infos[i], range_stress), 0);
            }
        }

        mesh.uv = Normalization(uvs);
        return mesh;
    }

    public static float[] getStressInfo(List<List<float>> infos)
    {
        float[] stress = new float[infos.Count];
        for (int i = 0; i < infos.Count; i++)
        {
            stress[i] = infos[i][3];
        }
        return stress;
    }

    public static float CaculateUV(Vector3 vertices, List<float> info, float range)
    {
        Vector3 stressPoint = new Vector3(info[0], info[1], info[2]);
        float distance = Vector3.Distance(vertices, stressPoint);
        float maxDistance = Mathf.Log(info[3]);
        return (maxDistance / (1 + distance)) * (info[3] / range);
    }

    public static Vector2[] Normalization(Vector2[] uvs)
    {
        float max = 0;
        float min = 0;
        for (int j = 0; j < uvs.Length; j++)
        {
            if (uvs[j].x > max)
            {
                max = uvs[j].x;
            }
            if (uvs[j].x < min)
            {
                min = uvs[j].x;
            }
        }

        for (int j = 0; j < uvs.Length; j++)
        {
            uvs[j].x = (uvs[j].x - min) / (max - min);
        }

        return uvs;
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

