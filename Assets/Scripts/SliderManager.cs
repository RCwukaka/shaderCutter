using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    private GameObject cutterPlean;
    void Start()
    {
        GameObject ModelObject = GameObject.Find("ModelObject");
        Bounds bounds = ModelObject.GetComponent<Renderer>().bounds;
        slider.minValue = -bounds.size.z;
        slider.maxValue = bounds.size.z;
    }

    public void Scrollbar(float value) {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) { 
            return;
        }
        cutterPlean = GameObject.Find("CutterPlane");
        cutterPlean.transform.position = new Vector3(0, value, 0);
        CutterPlaneManager.SliceObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
