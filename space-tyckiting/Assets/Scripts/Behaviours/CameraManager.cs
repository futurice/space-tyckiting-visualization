using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField]
		private Camera cinematicCamera;
		[SerializeField]
		private Camera mainCamera;

		private Rect leftViewRect;
		private Rect rightViewRect;

		void Awake()
		{
			leftViewRect = mainCamera.rect;
			rightViewRect = cinematicCamera.rect;
		}

		void Update()
		{
			if (Input.GetKeyUp(KeyCode.Alpha1)) CameraSetup1();
			else if (Input.GetKeyUp(KeyCode.Alpha2)) CameraSetup2();
			else if (Input.GetKeyUp(KeyCode.Alpha3)) CameraSetup3();
		}

		void CameraSetup1()
		{
			cinematicCamera.enabled = true;
			mainCamera.enabled = true;
			cinematicCamera.rect = rightViewRect;
			mainCamera.rect = leftViewRect;
		}

		void CameraSetup2()
		{
			cinematicCamera.enabled = false;
			mainCamera.enabled = true;
			mainCamera.rect = new Rect(0,0,0.85f,1);
		}

		void CameraSetup3()
		{
			cinematicCamera.enabled = true;
			mainCamera.enabled = false;
			cinematicCamera.rect = new Rect(0,0,1,1);
		}
	}
}