using UnityEngine;
using System.Collections;
using System.Linq;

namespace SpaceTyckiting
{
	[RequireComponent(typeof(Camera))]
	public class TopDownCamereController : MonoBehaviour 
	{
		private Camera targetCamera;

		private Vector3 targetFocus;
		private float targetSize;

		private Vector3 positionOffset;

		[SerializeField]
		private float moveSpeed;
		[SerializeField]
		private float zoomSpeed;
		[SerializeField]
		private float zoomMargin;
		[SerializeField]
		private float minSize;

		void Start()
		{
			targetCamera = GetComponent<Camera> ();
			targetFocus = targetCamera.transform.position;
			targetSize = targetCamera.orthographicSize;
			positionOffset = transform.position;
		}

		void Update () 
		{
			int unitCount = 0;
			if (GameManager.Instance.Units != null) 
			{
				unitCount = GameManager.Instance.Units.Count (x => x != null && x.isActiveAndEnabled);
			}

			if (unitCount > 0) 
			{
				Vector3 maxPosition = Vector3.zero;
				Vector3 minPosition = Vector3.zero;

				for (int i = 0; i < GameManager.Instance.Units.Count; i++) 
				{
					if (GameManager.Instance.Units [i] == null || !GameManager.Instance.Units [i].isActiveAndEnabled) 
					{
						continue;
					}

					var position = GameManager.Instance.Units [i].transform.position;

					minPosition.x = Mathf.Min (position.x, minPosition.x);
					minPosition.z = Mathf.Min (position.z, minPosition.z);
					maxPosition.x = Mathf.Max (position.x, maxPosition.x);
					maxPosition.z = Mathf.Max (position.z, maxPosition.z);
				}

				var center = (minPosition + maxPosition) * 0.5f;

				targetFocus = center + positionOffset;

				var sizeX = maxPosition.x - minPosition.x;
				var sizeZ = maxPosition.z - minPosition.z;

				targetSize = Mathf.Max(minSize, Mathf.Max (sizeX, sizeZ) * 0.5f + zoomMargin);
			}

			transform.position = Vector3.MoveTowards (transform.position, targetFocus, Time.deltaTime * moveSpeed);
			targetCamera.orthographicSize = Mathf.MoveTowards (targetCamera.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
		}
	}
}