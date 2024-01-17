using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeshCutter
{
    public TempMesh BoundaryMesh { get; private set; }

    private List<Vector3> addedPairs;
    private List<Vector2> addedUVs;

    private readonly List<Vector3> ogVertices;
    private readonly List<int> ogTriangles;
    private readonly List<Vector3> ogNormals;
    private readonly List<Color> ogColors;
    private readonly List<Vector2> ogUvs;

    private readonly Vector3[] intersectPair;
    private readonly Vector2[] intersectUVs;
    private readonly Vector3[] tempTriangle;
    private readonly Vector2[] tempUV;

    private Intersections intersect;


    public MeshCutter(int initialArraySize)
    {
        BoundaryMesh = new TempMesh(initialArraySize);

        addedPairs = new List<Vector3>(initialArraySize);
        addedUVs = new List<Vector2>(initialArraySize);
        ogVertices = new List<Vector3>(initialArraySize);
        ogNormals = new List<Vector3>(initialArraySize);
        ogUvs = new List<Vector2>(initialArraySize);
        ogColors = new List<Color>(initialArraySize);
        ogTriangles = new List<int>(initialArraySize * 3);

        intersectPair = new Vector3[2];
        intersectUVs = new Vector2[2];
        tempTriangle = new Vector3[3];
        tempUV = new Vector2[3];

        intersect = new Intersections();
    }

    public void SliceMesh(Mesh mesh, Transform transform,  Plane slice)
    {
        mesh.GetVertices(ogVertices);

        for (int i = 0; i < mesh.vertexCount; ++i)
        {
            ogVertices[i] = transform.TransformPoint(mesh.vertices[i]);
        }

        mesh.GetTriangles(ogTriangles, 0);
        mesh.GetNormals(ogNormals);
        mesh.GetUVs(0, ogUvs);
        mesh.GetColors(ogColors);

        BoundaryMesh.Clear();
        addedPairs.Clear();
        addedUVs.Clear();

        for (int i = 0; i < ogTriangles.Count; i += 3)
        {
            if (intersect.TrianglePlaneIntersect(ogVertices, ogUvs, ogTriangles, ogColors, i, ref slice, intersectPair, intersectUVs))
            {
                addedPairs.AddRange(intersectPair);
                addedUVs.AddRange(intersectUVs);
            }
        }

        FillBoundaryFace(addedPairs, addedUVs);
    }

    private void FillBoundaryFace(List<Vector3> addedPairs, List<Vector2> addedUVs)
    {
        /*
        using (StreamWriter writer = new StreamWriter(@"data.txt"))
        {
            for (int i = 0; i < addedPairs.Count; i++)
            {
                writer.WriteLine(addedPairs[i].x + "," + addedPairs[i].y+","+ addedPairs[i].z);
            }
        }
        */
        
        MeshUtils.ReorderList(addedPairs, addedUVs);

        addedPairs = MeshUtils.GetUsefulPoint(addedPairs);
        addedUVs = MeshUtils.GetUsefulUV(addedUVs);
        Vector3 center = MeshUtils.FindCenter(addedPairs);
        addedPairs.Add(center);
        addedUVs.Add(new Vector2(0,0));
        addedPairs = MeshUtils.OffsetToPoint(addedPairs, center);

        for (int i = 0; i < addedPairs.Count-1; i+=1) {
            if (i == addedPairs.Count - 2)
            {
                AddTriangle(addedPairs, addedUVs, i, 0, addedPairs.Count - 1);
                break;
            }
            AddTriangle(addedPairs, addedUVs, i, i+1, addedPairs.Count-1);
        }
    }

    private void AddTriangle(List<Vector3> face, List<Vector2> addedUVs, int t1, int t2, int t3)
    {
        tempTriangle[0] = face[t1];
        tempTriangle[1] = face[t2];
        tempTriangle[2] = face[t3];
        tempUV[0] = addedUVs[t1];
        tempUV[1] = addedUVs[t2];
        tempUV[2] = addedUVs[t3];
        BoundaryMesh.AddTriangle(tempTriangle, tempUV);

    }

}

