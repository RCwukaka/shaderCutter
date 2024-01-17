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
        baseInfo.text = "������Ϣ��"+ Mathf.FloorToInt(bounds.size.x)+"*" + Mathf.FloorToInt(bounds.size.y) + "*" + Mathf.FloorToInt(bounds.size.z);
        stressInfo.text = "���Ӧ����" + Mathf.FloorToInt(bounds.size.x) + "\t" +
            "ƽ��Ӧ����" + Mathf.FloorToInt(bounds.size.x) + "\t" +
            "��СӦ����" + Mathf.FloorToInt(bounds.size.x) + "\t";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
