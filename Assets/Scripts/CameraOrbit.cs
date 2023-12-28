using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {

    public Transform target;
    private Vector3 fixedPosition;

    public float distance = 2.0f;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float distanceMin = 2f;
    public float distanceMax = 10f;
    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
 

    // Use this for initialization
    void Start () {

        Vector3 angles = new Vector3(0, 0.5f, 45);
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }

        // Clone the target's position so that it stays fixed
        if (target)
            fixedPosition = target.position;
    }




}
