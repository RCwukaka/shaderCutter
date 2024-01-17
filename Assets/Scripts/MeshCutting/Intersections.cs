using System;
using System.Collections.Generic;
using System.Net;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using UnityEngine;
using UnityEngine.AI;

public class Intersections
{
    #region Static functions

    public static bool BoundPlaneIntersect(Mesh mesh, ref Plane plane)
    {
        float r = mesh.bounds.extents.x * Mathf.Abs(plane.normal.x) +
            mesh.bounds.extents.y * Mathf.Abs(plane.normal.y) +
            mesh.bounds.extents.z * Mathf.Abs(plane.normal.z);

        float s = Vector3.Dot(plane.normal, mesh.bounds.center) - (-plane.distance);

        return Mathf.Abs(s) <= r;
    }

    #endregion

    private readonly Vector3[] v;
    private readonly Vector2[] u;
    private readonly Color[] c;
    private readonly int[] t;
    private readonly bool[] positive;

    // Used in intersect method
    private Ray edgeRay;

    public Intersections()
    {
        v = new Vector3[3];
        u = new Vector2[3];
        t = new int[3];
        c = new Color[3];
        positive = new bool[3];
    }


    public ValueTuple<Vector3, Vector2> Intersect(Plane plane, Vector3 first, Vector3 second, Vector2 uv1, Vector2 uv2)
    {
        edgeRay.origin = first;
        edgeRay.direction = (second - first).normalized;
        float dist;
        float maxDist = Vector3.Distance(first, second);

        if (!plane.Raycast(edgeRay, out dist))
            throw new UnityException("Line-Plane intersect in wrong direction");


        var returnVal = new ValueTuple<Vector3, Vector2>
        {
            Item1 = edgeRay.GetPoint(dist)
        };

        if (Mathf.Abs(returnVal.Item1.x) < 0.0001) 
        {
            returnVal.Item1.x = 0;
        }
        if (Mathf.Abs(returnVal.Item1.y) < 0.0001)
        {
            returnVal.Item1.y = 0;
        }
        if (Mathf.Abs(returnVal.Item1.z) < 0.0001)
        {
            returnVal.Item1.z = 0;
        }

        var relativeDist = dist / maxDist;
        returnVal.Item2.x = Mathf.Lerp(uv1.x, uv2.x, relativeDist);
        returnVal.Item2.y = 0;
        return returnVal;
    }

    /*
     * Small diagram for reference :)
     *       |      |  /|
     *       |      | / |P1       
     *       |      |/  |         
     *       |    I1|   |
     *       |     /|   |
     *      y|    / |   |
     *       | P0/__|___|P2
     *       |      |I2
     *       |      |
     *       |___________________
     */

    public bool TrianglePlaneIntersect(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, List<Color> colors, int startIdx, ref Plane plane, Vector3[] intersectVectors, Vector2[] intersectUVs)
    {
        int i;

        for(i = 0; i < 3; ++i)
        {
            t[i] = triangles[startIdx + i];
            v[i] = vertices[t[i]];
            u[i] = uvs[t[i]];
        }

        positive[0] = MeshUtils.AlmostNearComplement(plane, v[0]) > 0;
        positive[1] = MeshUtils.AlmostNearComplement(plane, v[1]) > 0;
        positive[2] = MeshUtils.AlmostNearComplement(plane, v[2]) > 0;

        if (positive[0] == positive[1] && positive[1] == positive[2])
        {
            return false;
        }

        int lonelyPoint = 0;
        if (positive[0] != positive[1])
            lonelyPoint = positive[0] != positive[2] ? 0 : 1;
        else
            lonelyPoint = 2;

        int prevPoint = lonelyPoint - 1;
        if (prevPoint == -1) prevPoint = 2;
        int nextPoint = lonelyPoint + 1;
        if (nextPoint == 3) nextPoint = 0;

        ValueTuple<Vector3, Vector2> newPointPrev = Intersect(plane, v[lonelyPoint], v[prevPoint], u[lonelyPoint], u[prevPoint]);
        ValueTuple<Vector3, Vector2> newPointNext = Intersect(plane, v[lonelyPoint], v[nextPoint], u[lonelyPoint], u[nextPoint]);

        if (positive[lonelyPoint])
        {
            intersectVectors[0] = newPointPrev.Item1;
            intersectVectors[1] = newPointNext.Item1;
            intersectUVs[0] = newPointPrev.Item2;
            intersectUVs[1] = newPointNext.Item2;
        } else
        {
            intersectVectors[0] = newPointNext.Item1;
            intersectVectors[1] = newPointPrev.Item1;
            intersectUVs[0] = newPointNext.Item2;
            intersectUVs[1] = newPointPrev.Item2;
        }
        return true;
    }



}
