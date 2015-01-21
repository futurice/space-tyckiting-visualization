using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class SearchlightController : MonoBehaviour
	{
		[SerializeField]
		private Material material;

		private Mesh mesh;
		private GameObject meshObject;
		private Vector3 targetPosition;
		private Transform tr;
		private Color onColor;
		private GameObject targetObject;

		private bool isOn = false;

		void Awake()
		{
			material = Instantiate(material) as Material;

			targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			targetObject.GetComponent<Renderer>().material = material;
			targetObject.GetComponent<Transform>().localScale = Vector3.one * Settings.radarArea * Settings.cellSize;
			targetObject.SetActive(false);
			targetObject.GetComponent<Transform>().parent = GameManager.Instance.GameParent;

			var col = targetObject.GetComponent<Collider>();
			if (col != null) Destroy(col);

			tr = GetComponent<Transform>();
			meshObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

			var col2 = meshObject.GetComponent<Collider>();
			if (col2 != null) Destroy(col2);

			meshObject.transform.position = Vector3.zero;
			mesh = meshObject.GetComponent<MeshFilter>().mesh;
			meshObject.GetComponent<Renderer>().material = material;
			meshObject.SetActive(false);
			mesh.MarkDynamic();

			meshObject.GetComponent<Transform>().parent = GameManager.Instance.GameParent;

			onColor = material.GetColor("_TintColor");
		}

		public void ShowAnimated(int x, int y)
		{
			SetVertices(tr.position, Settings.GetWorldCoordinate(x, y));

			targetObject.GetComponent<Transform>().position = Settings.GetWorldCoordinate(x, y) + Vector3.up * Settings.cellSize * Settings.radarArea * 0.5f;

			meshObject.SetActive(true);
			targetObject.SetActive(true);

			AnimateLightOn();
		}

		void SetVertices(Vector3 startPosition, Vector3 targetPosition)
		{
			var vertices = mesh.vertices;
			float targetSize = Settings.cellSize * 0.5f * Settings.radarArea;
			float startSize = 0.5f;
			vertices[0] = targetPosition + new Vector3(targetSize, 0, targetSize);
			vertices[1] = targetPosition + new Vector3(-targetSize, 0, targetSize);

			vertices[2] = startPosition + new Vector3(startSize, 0, startSize);
			vertices[3] = startPosition + new Vector3(-startSize, 0, startSize);
			vertices[4] = startPosition + new Vector3(startSize, 0, -startSize);
			vertices[5] = startPosition + new Vector3(-startSize, 0, -startSize);

			vertices[6] = targetPosition + new Vector3(targetSize, 0, -targetSize);
			vertices[7] = targetPosition + new Vector3(-targetSize, 0, -targetSize);

			vertices[8] = startPosition + new Vector3(startSize, 0, startSize);
			vertices[9] = startPosition + new Vector3(-startSize, 0, startSize);
			vertices[10] = startPosition + new Vector3(startSize, 0, -startSize);
			vertices[11] = startPosition + new Vector3(-startSize, 0, -startSize);

			vertices[12] = targetPosition + new Vector3(targetSize, 0, -targetSize);
			vertices[13] = targetPosition + new Vector3(-targetSize, 0, targetSize);
			vertices[14] = targetPosition + new Vector3(-targetSize, 0, -targetSize);
			vertices[15] = targetPosition + new Vector3(targetSize, 0, targetSize);
			vertices[16] = targetPosition + new Vector3(-targetSize, 0, targetSize);

			vertices[17] = startPosition + new Vector3(-startSize, 0, -startSize);

			vertices[18] = targetPosition + new Vector3(-targetSize, 0, -targetSize);

			vertices[19] = startPosition + new Vector3(-startSize, 0, startSize);

			vertices[20] = targetPosition + new Vector3(targetSize, 0, -targetSize);

			vertices[21] = startPosition + new Vector3(startSize, 0, startSize);

			vertices[22] = targetPosition + new Vector3(targetSize, 0, targetSize);

			vertices[23] = startPosition + new Vector3(startSize, 0, -startSize);

			mesh.vertices = vertices;
		}

		void AnimateLightOn()
		{
			isOn = true;

			// LeanTween.value seems to fail at times, just make sure light is turned off
			Invoke("LightOffCompletion", 1.8f);

			material.SetColor("_TintColor", Color.black);
			var lightOn = LeanTween.value(gameObject, Color.black, onColor, 0.3f);
			lightOn.setOnUpdateColor((Color val) =>
			{
				material.SetColor("_TintColor", val);
			});
			lightOn.onComplete = () =>
			{
				AnimateLightOff();
			};
			lightOn.setEase(LeanTweenType.easeInOutBounce);
		}

		void AnimateLightOff()
		{
			var lightOff = LeanTween.value(gameObject, onColor, Color.black, 0.4f);
			lightOff.setOnUpdateColor((Color val) =>
			{
				material.SetColor("_TintColor", val);
			});
			lightOff.onComplete = () =>
			{
				LightOffCompletion();
				
			};
			lightOff.setEase(LeanTweenType.easeInOutBounce);
			var delay = 0.5f + Random.value * 0.5f;
			lightOff.delay = delay;			
		}

		void LightOffCompletion()
		{
			if (!isOn) return;

			isOn = false;
			meshObject.SetActive(false);
			targetObject.SetActive(false);
		}

		void OnDestroy()
		{
			if (meshObject != null) Destroy(meshObject);
			if (meshObject != null) Destroy(targetObject);
		}
	}
}