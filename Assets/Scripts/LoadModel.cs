using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class LoadModel : MonoBehaviour
{
    private string _fileName = "";
    private string _trianglescount = "";

    private int _total;//三角形个数，_total*3即为三角形顶点个数
    private int _number;
    private BinaryReader _binaryReader;

    private List<Vector3> _vertices;
    private List<int> _triangles;

    /// <summary>
    ///获得STL模型名称和三角形数量
    /// </summary>
    private void GetFileNameAndTrianglesCount()
    {
        //string fullPath = Path.GetFullPath(AssetDatabase.GetAssetPath(target));
        string fullPath = "D:\\UnityProject\\DrawMesh\\Assets\\ImportSTL\\徽章.stl";

        using (BinaryReader br = new BinaryReader(File.Open(fullPath, FileMode.Open)))
        {
            _fileName = Encoding.UTF8.GetString(br.ReadBytes(80));//stl二进制文件中，前80个字节为模型名称
            _trianglescount = BitConverter.ToInt32(br.ReadBytes(4), 0).ToString();//之后4个字节存储的是模型三角形数量
        }
    }
    /// <summary>
    /// 创建STL模型实例
    /// </summary>
    private void CreateInstance()
    {
        string fullPath = "D:\\UnityProject\\DrawMesh\\Assets\\ImportSTL\\徽章.stl";
        int gameObjectCount = 60000;//代表一个物体中包含多少个点（Unity中单个Mesh顶点数量最多为65000个）

        _total = int.Parse(_trianglescount);
        _number = 0;
        _binaryReader = new BinaryReader(File.Open(fullPath, FileMode.Open));

        //抛弃前84个字节
        _binaryReader.ReadBytes(84);

        _vertices = new List<Vector3>();//存储三角形顶点坐标
        _triangles = new List<int>();//存储三角形索引

        while (_number < _total)
        {
            byte[] bytes;
            //每50个字节一组，存储着三角形的法矢量和三个点的顶点数据
            bytes = _binaryReader.ReadBytes(50);

            if (bytes.Length < 50)
            {
                _number += 1;
                continue;
            }
            //这里只用了三角形的顶点数据，忽略矢量
            Vector3 vec1 = new Vector3(BitConverter.ToSingle(bytes, 12), BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20));
            Vector3 vec2 = new Vector3(BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28), BitConverter.ToSingle(bytes, 32));
            Vector3 vec3 = new Vector3(BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40), BitConverter.ToSingle(bytes, 44));

            _vertices.Add(vec1);
            _vertices.Add(vec2);
            _vertices.Add(vec3);

            _number += 1;
        }

        //将索引写入列表中，值为0到每个物体中顶点个数减一
        for (int triNum = 0; triNum < _vertices.Count; triNum++)
        {
            int gameObhectIndex = triNum / gameObjectCount;//标记着当前正在给第几个物体赋值
            _triangles.Add(triNum - gameObhectIndex * gameObjectCount);
        }

        for (int meshNumber = 0; meshNumber < _vertices.Count; meshNumber += gameObjectCount)
        {
            //创建GameObject
            GameObject tem = new GameObject(Path.GetFileNameWithoutExtension(fullPath));
            tem.name = meshNumber.ToString();
            MeshFilter mf = tem.AddComponent<MeshFilter>();
            MeshRenderer mr = tem.AddComponent<MeshRenderer>();

            Mesh m = new Mesh();
            mr.name = meshNumber.ToString();
            if ((_vertices.Count - meshNumber) >= gameObjectCount)
            {
                m.vertices = _vertices.ToArray().Skip(meshNumber).Take(gameObjectCount).ToArray();
                m.triangles = _triangles.ToArray().Skip(meshNumber).Take(gameObjectCount).ToArray();
            }
            else
            {
                m.vertices = _vertices.ToArray().Skip(meshNumber).Take(_vertices.Count - meshNumber).ToArray();
                m.triangles = _triangles.ToArray().Skip(meshNumber).Take(_vertices.Count - meshNumber).ToArray();
            }
            m.RecalculateNormals();

            mf.mesh = m;
            mr.material = new Material(Shader.Find("Standard"));

            _binaryReader.Close();

            //Debug.Log(tem.name + ":顶点数量 " + _vertices.Count);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetFileNameAndTrianglesCount();
        CreateInstance();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
