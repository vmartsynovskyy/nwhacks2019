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
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
		// detect oculus remote
		string[] joystickNames = Input.GetJoystickNames();
		// if oculus remote detected
		if (joystickNames.Length > 0 && joystickNames[0] == "Oculus Remote")
		{
			// TODO: handle remote controls
		}

        if (Input.GetKey("w"))
		{
			moveForward();
		}
		if (Input.GetKey("s"))
		{
			moveBackward();
		}
		if (Input.GetKey("a"))
		{
			moveLeft();
		}
		if (Input.GetKey("d"))
		{
			moveRight();
		}

		if (Input.GetKeyDown("g")) {
			if (isObjectGrabbed == false)
			{
				grabObject();
			}
			else
			{
				releaseObject();
			}
		}
		
		if (isObjectGrabbed)
		{
			// object following player
			objectFollowPlayer();

			// object rotation
			bool left = Input.GetKey("left"); // left arrow
			bool right = Input.GetKey("right"); // right arrow
			objectRotate(left, right);

			// object push and pull
			bool up = Input.GetKey("up"); // up arrow
			bool down = Input.GetKey("down"); // down arrow
			objectPushPull(up, down);
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

	private void objectPushPull(bool up, bool down)
	{
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

	private void objectRotate(bool left, bool right)
	{
		if (left || right) // if either arrow
		{
			// why "!left" instead of "right"?
			// it's if both are pressed - then it will skip both
			if (!left) // right is pressed, rotate "right"
			{
				grabbedObject.transform.Rotate(Vector3.up * Time.deltaTime * moveSpeed * 90);
			}
			if (!right) // left is pressed, rotate "left"
			{
				grabbedObject.transform.Rotate(Vector3.down * Time.deltaTime * moveSpeed * 90);
			}
			// if both, do neither
		}
	}

	private void objectFollowPlayer()
	{
		Vector3 desiredPosition = transform.position + transform.forward * grabbedDistance;
		Vector3 desiredPositionDiff = desiredPosition - grabbedObject.transform.position;
		Vector3 clampledDiff = Vector3.ClampMagnitude(desiredPositionDiff, Time.deltaTime * 3);
		Vector3 preCollision = grabbedObject.transform.position + clampledDiff;
		preCollision.y = Mathf.Max(0.25f, preCollision.y);
		grabbedObject.transform.position = preCollision;

		if (Input.GetKey("f"))
		{
			isObjectGrabbed = false;
			Destroy(grabbedObject);
		}
	}

	private void releaseObject()
	{
		isObjectGrabbed = false;
		Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.useGravity = true;
		}
	}

	private void grabObject()
	{
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

	private void moveRight()
	{
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 right = transform.TransformDirection(Vector3.right);
		controller.SimpleMove(right * moveSpeed);
	}

	private void moveLeft()
	{
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 left = transform.TransformDirection(Vector3.left);
		controller.SimpleMove(left * moveSpeed);
	}

	private void moveBackward()
	{
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 back = transform.TransformDirection(Vector3.back);
		controller.SimpleMove(back * moveSpeed);
	}

	private void moveForward()
	{
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		controller.SimpleMove(forward * moveSpeed);
	}
}
