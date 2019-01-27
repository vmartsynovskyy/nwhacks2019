using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMove : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivity = 15F;
    private float minimumX = -360F;
    private float maximumX = 360F;
    private float minimumY = -60F;
    private float maximumY = 60F;
    public float moveSpeed = 1F;

    private bool isObjectGrabbed = true;
    private float grabbedDistance = 0F;
    private GameObject grabbedObject;


    float rotationY = 0F;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w")) {
            transform.Translate(0, 0, Time.deltaTime*moveSpeed);
        }
        if (Input.GetKey("s")) {
            transform.Translate(0, 0, -Time.deltaTime*moveSpeed);
        }
        if (Input.GetKey("a")) {
            transform.Translate(-Time.deltaTime*moveSpeed, 0, 0);
        }
        if (Input.GetKey("d")) {
            transform.Translate(Time.deltaTime*moveSpeed, 0, 0);
        }
        if (Input.GetKey("space")) {
            transform.Translate(0, Time.deltaTime*moveSpeed, 0);
        }

        if (Input.GetKeyDown("g")) {
            RaycastHit hit;  
            bool didHit = Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);
            if (didHit) {
                isObjectGrabbed = true;
                grabbedDistance = hit.distance;
                grabbedObject = hit.transform.gameObject;
                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null) {
                    if (rb.useGravity == true) {
                        rb.useGravity = false;
                    }
                }
            }
        }

        if (Input.GetKeyUp("g")) {
            isObjectGrabbed = false;
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.useGravity = true;
            }
        }

        if (Input.GetKey("g")) {
            Vector3 desiredPosition = this.transform.position + this.transform.forward*grabbedDistance;
            Vector3 desiredPositionDiff = desiredPosition - grabbedObject.transform.position;   
            Vector3 clampledDiff = Vector3.ClampMagnitude(desiredPositionDiff, Time.deltaTime*3);
            Vector3 preCollision = grabbedObject.transform.position + clampledDiff;
            preCollision.y = Mathf.Max(0.5f, preCollision.y);;

            grabbedObject.transform.position = preCollision;
        }

        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            
            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }
}
