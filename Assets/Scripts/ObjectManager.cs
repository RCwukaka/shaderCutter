using System.IO;
using Excel;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data;
using System.Collections.Generic;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

public class ObjectManager : MonoBehaviour
{
    private float[] physicsDatas;
    private Vector3 _objectCenter = Config._objectPosition;
    GameObject ModelObject;

    // Start is called before the first frame update
    void Awake()
    {
        Transform panelParent = GameObject.Find("SliceObjects").transform;
        ModelObject = GameObject.Instantiate(Resources.Load<GameObject>("ModelObject"), panelParent);
        ModelObject.transform.position = _objectCenter;
        ModelObject.name = "ModelObject";

        List<List<float>> infos = new List<List<float>>();
        ReadExcel("/Scripts/stressPoint.xlsx", infos);
        Mesh mesh = ModelObject.GetComponent<MeshFilter>().mesh;

        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        foreach (Material m in materials) {
            if (m.name == "M1") {
                ModelObject.GetComponent<MeshRenderer>().material = m;
            }
        }
        CloudMapRendering.CalculateModelUVS(mesh, ModelObject.transform, infos);

    }

    public void ReadExcel(string xmlName, List<List<float>> infos)
    {
        FileStream stream = File.Open(Application.dataPath + xmlName, FileMode.Open, FileAccess.Read, FileShare.Read);
        //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);//读取 Excel 1997-2003版本
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);//读取 2007及以后的版本

        DataSet result = excelReader.AsDataSet();

        if (stream != null)
        {
            stream.Close();
        }

        int[] counts = GetCount(result.Tables[0]);
        int rows = counts[0];
        List<float> info = new List<float>();
        for (int i = 0; i < rows; i++)
        {
            info.Add(Convert.ToSingle(result.Tables[0].Rows[i][0].ToString()));
            info.Add(Convert.ToSingle(result.Tables[0].Rows[i][1].ToString()));
            info.Add(Convert.ToSingle(result.Tables[0].Rows[i][2].ToString()));
            info.Add(Convert.ToSingle(result.Tables[0].Rows[i][3].ToString()));
            infos.Add(info);
        }
    }

    private int[] GetCount(DataTable dt)
    {
        int i = dt.Rows.Count;
        for (int m = 0; m < dt.Rows.Count; m++)
        {
            if (string.IsNullOrEmpty(dt.Rows[m][0].ToString()))
            {
                i = m;
                break;
            }
        }

        int j = dt.Columns.Count;
        for (int n = 0; n < dt.Columns.Count; n++)
        {
            if (string.IsNullOrEmpty(dt.Rows[0][n].ToString()))
            {
                j = n;
                break;
            }
        }
        return new int[] { i, j };
    }

    void Update()
    {
        W_A_S_D();
    }

    private void W_A_S_D()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ModelObject.transform.RotateAround(_objectCenter, new Vector3(0, 0, 1), 1f);
            CutterPlaneManager.SliceObject();
        }else if (Input.GetKey(KeyCode.S))
        {
            ModelObject.transform.RotateAround(_objectCenter, new Vector3(0, 0, -1), 1f);
            CutterPlaneManager.SliceObject();
        }else if (Input.GetKey(KeyCode.A))
        {
            ModelObject.transform.RotateAround(_objectCenter, new Vector3(0, 1, 0), 1f);
            CutterPlaneManager.SliceObject();
        }else if (Input.GetKey(KeyCode.D))
        {
            ModelObject.transform.RotateAround(_objectCenter, new Vector3(0, -1, 0), 1f);
            CutterPlaneManager.SliceObject();
        }
    }

}
