using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBuilder
{
 
    private static MeshFilter _meshFilter;

    private static MeshRenderer _meshRenderer;

    /// <summary>
    /// ��Ԫ���С
    /// </summary>
    private static Vector2 _cellSize = new Vector2(5, 5);

    /// <summary>
    /// �����С
    /// </summary>
    private static Vector2Int _gridSize = new Vector2Int(30, 30);

    /// <summary>
    /// ����λ��
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

        //����Plane��С
        Vector2 size;
        size.x = _cellSize.x * _gridSize.x;
        size.y = _cellSize.y * _gridSize.y;

        //����Planeһ���С
        Vector2 halfSize = size / 2;

        //���㶥�㼰UV
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 vertice = Vector3.zero;
        Vector2 uv = Vector3.zero;

        for (int y = 0; y < _gridSize.y + 1; y++)
        {
            vertice.z = y * _cellSize.y - halfSize.y +_position.z;//���㶥��Y��
            uv.y = y * _cellSize.y / size.y;//���㶥����������V
            vertice.y = _position.y;
            for (int x = 0; x < _gridSize.x + 1; x++)
            {
                vertice.x = x * _cellSize.x - halfSize.x + _position.x;//���㶥��X��
                uv.x = x * _cellSize.x / size.x;//���㶥����������U

                vertices.Add(vertice);//��ӵ���������
                uvs.Add(uv);//��ӵ�������������
            }
        }

        //��������
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        int startIndex = 0;
        int[] indexs = new int[_gridSize.x * _gridSize.y * 2 * 3];//��������
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                //�ı����ĸ�����
                a = y * (_gridSize.x + 1) + x;//0
                b = (y + 1) * (_gridSize.x + 1) + x;//1
                c = b + 1;//2
                d = a + 1;//3

                //�����������е�������
                startIndex = y * _gridSize.x * 2 * 3 + x * 2 * 3;

                //����������
                indexs[startIndex] = a;//0
                indexs[startIndex + 1] = b;//1
                indexs[startIndex + 2] = c;//2

                //����������
                indexs[startIndex + 3] = c;//2
                indexs[startIndex + 4] = d;//3
                indexs[startIndex + 5] = a;//0
            }
        }

        //
        mesh.SetVertices(vertices);//���ö���
        mesh.SetUVs(0, uvs);//����UV
        mesh.SetIndices(indexs, MeshTopology.Triangles, 0);//���ö�������
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        _meshFilter.mesh = mesh;
        _meshRenderer.material = new Material(Shader.Find("Standard"));
    }

}
