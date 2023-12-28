using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using OfficeOpenXml;

public class ObjectManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        /*
        var toSlice = GameObject.FindGameObjectsWithTag("Sliceable");
        GameObject obj;
        Application excel = new Application();
        FileInfo fileInfo = new FileInfo(Application.dataPath + "/Scripts/object.xlsx");
        ExcelPackage excelPackage = new ExcelPackage(fileInfo);
        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
        for (int i = 0; i < toSlice.Length; ++i) {
            Debug.Log(toSlice.Length);
            obj = toSlice[i];
            Vector3 vertices = new Vector3();
            float[] physicsDatas = new float[obj.GetComponent<MeshFilter>().mesh.vertices.Length];
            for (int j = 0; j < obj.GetComponent<MeshFilter>().mesh.vertices.Length; j++)
            {
                vertices = obj.GetComponent<MeshFilter>().mesh.vertices[j];
                Debug.Log(vertices.x);
                worksheet.Cells[j+1, 1].Value = vertices.x;
                worksheet.Cells[j+1, 2].Value = vertices.y;
                worksheet.Cells[j+1, 3].Value = vertices.z;
            }
            excelPackage.Save();
        }
        */

    }

    public void LoadObject()
    {
        SceneManager.LoadScene("Demo");
    }
}
