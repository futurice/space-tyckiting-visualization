using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private Camera targetCamera;
		[SerializeField]
		private Transform rotationTransform;
		[SerializeField]
		private Transform moveTransform;

		[SerializeField]
		private float moveSpeed = 10;
		[SerializeField]
		private float rotateSpeed = 1;
		[SerializeField]
		private float upDownSpeed = 1;
		[SerializeField]
		private float boostFactor = 3;

		[SerializeField]
		private float autoRotateSpeed = 10;
		[SerializeField]
		private float autoRotateMoveSpeed = 10;
		[SerializeField]
		private float autoRotateDelay = 4;
		[SerializeField]
		private float autoRotateHeight = 100;
		[SerializeField]
		private Vector3 autoRotateLookAtPos = new Vector3(0, 10, 0);
		[SerializeField]
		private float autoRotateDistance = 400;

		private Vector3 lastMousePosition;

		private float lastTouchTime = 0;

		private bool isAutoRotating = false;

		// Update is called once per frame
		void Update()
		{
			if (!targetCamera.enabled) return;

			if (Input.anyKey || Input.GetMouseButton(1))
			{
				isAutoRotating = false;
				lastTouchTime = Time.time;
			}
			else
			{
				isAutoRotating = Time.time - lastTouchTime > autoRotateDelay;
			}
			
			if (isAutoRotating)
			{
				var targetX = Mathf.Sin(Time.time * autoRotateSpeed) * autoRotateDistance;
				var targetY = Mathf.Cos(Time.time * autoRotateSpeed) * autoRotateDistance;
				var targetPosition = new Vector3(targetX, autoRotateHeight, targetY);

				moveTransform.position = Vector3.MoveTowards(moveTransform.position, targetPosition, Time.deltaTime * autoRotateMoveSpeed);

				var targetRotation = Quaternion.LookRotation(autoRotateLookAtPos - rotationTransform.position, Vector3.up);
				var targetRotationEuler = targetRotation.eulerAngles;
				moveTransform.eulerAngles = Vector3.up * targetRotationEuler.y;
				rotationTransform.localEulerAngles = Vector3.right * targetRotationEuler.x;
			}
			else
			{
				if (Input.GetMouseButton(1))
				{
					var mouseDelta = Input.mousePosition - lastMousePosition;
					var deltaX = mouseDelta.x;
					var deltaZ = mouseDelta.y;

					moveTransform.Rotate(Vector3.up * deltaX * rotateSpeed);
					rotationTransform.Rotate(-Vector3.right * deltaZ * rotateSpeed);
				}

				var forwardSpeed = Input.GetAxis("Vertical") * moveSpeed;
				var sidewaysSpeed = Input.GetAxis("Horizontal") * moveSpeed;
				var upSpeed = Input.mouseScrollDelta.y * upDownSpeed;

				var movement = Vector3.forward * forwardSpeed + Vector3.right * sidewaysSpeed + Vector3.up * upSpeed;

				if (Input.GetKey(KeyCode.LeftShift)) movement *= boostFactor;
				moveTransform.Translate(Time.deltaTime * movement);

				lastMousePosition = Input.mousePosition;
			}
		}
	}
}