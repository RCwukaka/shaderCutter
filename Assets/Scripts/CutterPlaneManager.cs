using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CutterPlaneManager : MonoBehaviour
{

    private static GameObject cutterPlane;

    private static MeshCutter meshCutter;

    private static GameObject obj;

    private static GameObject stressPlane;

    void Start () 
    {
        PlaneBuilder.Create();
        meshCutter = new MeshCutter(256);
        SliceObject();
    }

    public static void SliceObject()
    {
        obj = GameObject.Find("ModelObject");
        cutterPlane = GameObject.Find("CutterPlane");
        Mesh mesh = cutterPlane.GetComponent<MeshFilter>().mesh;
        Vector3 normal = mesh.normals[0];
        Plane slicePlane = new Plane();
        slicePlane.SetNormalAndPosition(normal, cutterPlane.transform.position);

        Mesh objectMesh = obj.GetComponent<MeshFilter>().mesh;

        meshCutter.SliceMesh(objectMesh, obj.transform, slicePlane);

        CreateStressPlane();
    }

    private static void CreateStressPlane()
    {
        Material material = obj.GetComponent<MeshRenderer>().material;
        stressPlane = GameObject.Find("Dynamic Stress");
        if (stressPlane) {
            GameObject.Destroy(stressPlane);
        }
        if(meshCutter.BoundaryMesh.vertices.Count == 0)
        {
            return;
        }
        stressPlane = new GameObject("Dynamic Stress");
        stressPlane.AddComponent<MeshRenderer>();
        stressPlane.GetComponent<MeshRenderer>().material = material;
        MeshFilter plane_meshFilter = stressPlane.AddComponent<MeshFilter>();
        var planeMesh = plane_meshFilter.mesh;
        planeMesh.SetVertices(meshCutter.BoundaryMesh.vertices);
        planeMesh.SetTriangles(meshCutter.BoundaryMesh.triangles, 0);
        planeMesh.SetUVs(0, meshCutter.BoundaryMesh.uvs);

        Quaternion rotation = Quaternion.FromToRotation(meshCutter.BoundaryMesh.normals[0], Vector3.up);
        stressPlane.transform.SetPositionAndRotation(new Vector3(0, 0, 0), rotation);
    }

}
