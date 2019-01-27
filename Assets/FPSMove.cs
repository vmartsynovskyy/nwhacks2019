using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMove : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
	public const int LAYER_GRABBABLE = 9;
    public float sensitivity = 15F;
    private float minimumX = -360F;
    private float maximumX = 360F;
    private float minimumY = -60F;
    private float maximumY = 60F;
    public float moveSpeed = 1F;

    private bool isObjectGrabbed = false;
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
		CharacterController controller = GetComponent<CharacterController>();

        if (Input.GetKey("w")) {
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			controller.SimpleMove(forward * moveSpeed);
		}
        if (Input.GetKey("s"))
		{
			Vector3 back = transform.TransformDirection(Vector3.back);
			controller.SimpleMove(back * moveSpeed);
		}
        if (Input.GetKey("a"))
		{
			Vector3 left = transform.TransformDirection(Vector3.left);
			controller.SimpleMove(left * moveSpeed);
		}
        if (Input.GetKey("d"))
		{
			Vector3 right = transform.TransformDirection(Vector3.right);
			controller.SimpleMove(right * moveSpeed);
		}

        if (Input.GetKeyDown("g")) {
            RaycastHit hit;  
            bool didHit = Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);
            if (didHit)
			{
				grabbedObject = hit.transform.gameObject;
				if (grabbedObject.layer == LAYER_GRABBABLE)
				{
					isObjectGrabbed = true;
					grabbedDistance = hit.distance;
					Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
					if (rb != null)
					{
						if (rb.useGravity == true)
						{
							rb.useGravity = false;
						}
						if (rb.isKinematic == true)
						{
							rb.isKinematic = false;
						}
					}
				}
            }
        }

        if (Input.GetKeyUp("g") && isObjectGrabbed) {
            isObjectGrabbed = false;
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.useGravity = true;
            }
        }

        if (Input.GetKey("g") && isObjectGrabbed) {
            Vector3 desiredPosition = this.transform.position + this.transform.forward*grabbedDistance;
            Vector3 desiredPositionDiff = desiredPosition - grabbedObject.transform.position;   
            Vector3 clampledDiff = Vector3.ClampMagnitude(desiredPositionDiff, Time.deltaTime*3);
            Vector3 preCollision = grabbedObject.transform.position + clampledDiff;
            preCollision.y = Mathf.Max(0.5f, preCollision.y);;

            grabbedObject.transform.position = preCollision;
        }

		if (isObjectGrabbed)
		{
			bool left = Input.GetKey("left"); // left arrow
			bool right = Input.GetKey("right"); // right arrow

			if (left || right) // if either arrow
			{
				// why "!left" instead of "right"?
				// it's if both are pressed - then it will skip both
				if (!left) // right is pressed, rotate "right"
				{
					grabbedObject.transform.Rotate(Vector3.up * Time.deltaTime * moveSpeed * 360);
				}
				if (!right) // left is pressed, rotate "left"
				{
					grabbedObject.transform.Rotate(Vector3.down * Time.deltaTime * moveSpeed * 360);
				}
				// if both, do neither
			}

			bool up = Input.GetKey("up"); // up arrow
			bool down = Input.GetKey("down"); // down arrow

			if (up || down) // if either arrow
			{
				// why "!up" instead of "down"?
				// it's if both are pressed - then it will skip both
				if (!up) // down is pressed, pull object
				{
					grabbedDistance *= 0.98f;
				}
				if (!down) // up is pressed, push object
				{
					grabbedDistance *= 1.02f;
				}
				// if both, do neither
			}
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
