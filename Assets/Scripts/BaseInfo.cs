using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BaseInfo : MonoBehaviour
{
    public Text baseInfo;

    public Text stressInfo;

    void Start()
    {
        GameObject ModelObject = GameObject.Find("ModelObject");
        Bounds bounds = ModelObject.GetComponent<Renderer>().bounds;
        baseInfo.text = "基本信息："+ Mathf.FloorToInt(bounds.size.x)+"*" + Mathf.FloorToInt(bounds.size.y) + "*" + Mathf.FloorToInt(bounds.size.z);
        stressInfo.text = "最大应力：" + Mathf.FloorToInt(bounds.size.x) + "\t" +
            "平均应力：" + Mathf.FloorToInt(bounds.size.x) + "\t" +
            "最小应力：" + Mathf.FloorToInt(bounds.size.x) + "\t";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
