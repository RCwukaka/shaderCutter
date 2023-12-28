using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRenderer : MonoBehaviour
{
    private float[] physicsDatas;
    // Start is called before the first frame update
    void Awake()
    {
        var toSlice = GameObject.FindGameObjectsWithTag("Sliceable");
        GameObject obj;
        for (int i = 0; i < toSlice.Length; ++i)
        {
            obj = toSlice[i];
            // We multiply by the inverse transpose of the worldToLocal Matrix, a.k.a the transpose of the localToWorld Matrix
            // Since this is how normal are transformed


            physicsDatas = new float[obj.GetComponent<MeshFilter>().mesh.vertices.Length];
            for (int j = 0; j < obj.GetComponent<MeshFilter>().mesh.vertices.Length; j++)
            {
                physicsDatas[j] = j * 10f;
            }
            
        }
    }

    void Start()
    {
        var toSlice = GameObject.FindGameObjectsWithTag("Sliceable");
        GameObject obj;
        for (int i = 0; i < toSlice.Length; ++i)
        {
            obj = toSlice[i];
            Color[] colors = CloudMapRendering.CalculateModelVerticeColors(physicsDatas);
            obj.GetComponent<MeshFilter>().mesh.colors = colors;
        }
    }

        // Update is called once per frame
    void Update()
    {
        
    }
}
