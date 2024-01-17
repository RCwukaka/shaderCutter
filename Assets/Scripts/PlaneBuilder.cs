using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBuilder
{
 
    private static MeshFilter _meshFilter;

    private static MeshRenderer _meshRenderer;

    /// <summary>
    /// 单元格大小
    /// </summary>
    private static Vector2 _cellSize = new Vector2(5, 5);

    /// <summary>
    /// 网格大小
    /// </summary>
    private static Vector2Int _gridSize = new Vector2Int(30, 30);

    /// <summary>
    /// 网格位置
    /// </summary>
    private static Vector3 _position = Config._objectPosition;

    public static void Create()
    {
        GameObject plane = new GameObject("CutterPlane");
        _meshFilter = plane.AddComponent<MeshFilter>();
        _meshRenderer = plane.AddComponent<MeshRenderer>();
        UpdateMesh();
    }

    public static void UpdateMesh()
    {
        Mesh mesh = new Mesh();

        //计算Plane大小
        Vector2 size;
        size.x = _cellSize.x * _gridSize.x;
        size.y = _cellSize.y * _gridSize.y;

        //计算Plane一半大小
        Vector2 halfSize = size / 2;

        //计算顶点及UV
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 vertice = Vector3.zero;
        Vector2 uv = Vector3.zero;

        for (int y = 0; y < _gridSize.y + 1; y++)
        {
            vertice.z = y * _cellSize.y - halfSize.y +_position.z;//计算顶点Y轴
            uv.y = y * _cellSize.y / size.y;//计算顶点纹理坐标V
            vertice.y = _position.y;
            for (int x = 0; x < _gridSize.x + 1; x++)
            {
                vertice.x = x * _cellSize.x - halfSize.x + _position.x;//计算顶点X轴
                uv.x = x * _cellSize.x / size.x;//计算顶点纹理坐标U

                vertices.Add(vertice);//添加到顶点数组
                uvs.Add(uv);//添加到纹理坐标数组
            }
        }

        //顶点序列
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        int startIndex = 0;
        int[] indexs = new int[_gridSize.x * _gridSize.y * 2 * 3];//顶点序列
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                //四边形四个顶点
                a = y * (_gridSize.x + 1) + x;//0
                b = (y + 1) * (_gridSize.x + 1) + x;//1
                c = b + 1;//2
                d = a + 1;//3

                //计算在数组中的起点序号
                startIndex = y * _gridSize.x * 2 * 3 + x * 2 * 3;

                //左上三角形
                indexs[startIndex] = a;//0
                indexs[startIndex + 1] = b;//1
                indexs[startIndex + 2] = c;//2

                //右下三角形
                indexs[startIndex + 3] = c;//2
                indexs[startIndex + 4] = d;//3
                indexs[startIndex + 5] = a;//0
            }
        }

        //
        mesh.SetVertices(vertices);//设置顶点
        mesh.SetUVs(0, uvs);//设置UV
        mesh.SetIndices(indexs, MeshTopology.Triangles, 0);//设置顶点序列
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        _meshFilter.mesh = mesh;
        _meshRenderer.material = new Material(Shader.Find("Standard"));
    }

}
