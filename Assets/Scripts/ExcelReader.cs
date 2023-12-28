using System.Data;
using System.IO;
using Excel;
using UnityEngine;

public class ExcelTool : MonoBehaviour
{
    void Start()
    {
        ReadExcel("/Scripts/data.xlsx");
    }

    public void ReadExcel(string xmlName)
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
        int columns = counts[1];
        for (int i = 0; i < rows; i++)
        { 
            for (int j = 0; j < columns; j++)
            {
                // Debug.Log(result.Tables[0].Rows[i][j].ToString());
            }
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
}