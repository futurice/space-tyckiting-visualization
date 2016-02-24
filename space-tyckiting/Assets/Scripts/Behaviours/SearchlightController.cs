using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class SearchlightController : MonoBehaviour
	{
		[SerializeField]
		private Material material;
		[SerializeField]
		private Material beamMaterial;
		[SerializeField]
		private GameObject targetObjectPrefab;

		private Mesh mesh;
		private GameObject meshObject;
		private Vector3 targetPosition;
		private Transform tr;
		private Color onColor;
		private Color beamOnColor;
		private GameObject targetObject;

		private bool isOn = false;

		void Awake()
		{
			material = Instantiate(material) as Material;

			targetObject = Instantiate (targetObjectPrefab) as GameObject;

			targetObject.GetComponent<Renderer>().material = material;
			targetObject.GetComponent<Transform>().localScale = Vector3.one * (1 + 2 * Settings.radarArea) * Settings.cellWidth*5;
			targetObject.SetActive(false);
			targetObject.GetComponent<Transform>().parent = GameManager.Instance.GameParent;

			var col = targetObject.GetComponent<Collider>();
			if (col != null) Destroy(col);

			tr = GetComponent<Transform>();
			meshObject = Instantiate (targetObjectPrefab) as GameObject;

			var col2 = meshObject.GetComponent<Collider>();
			if (col2 != null) Destroy(col2);

			meshObject.transform.position = Vector3.zero;
			mesh = meshObject.GetComponent<MeshFilter>().mesh;
			meshObject.GetComponent<Renderer>().material = beamMaterial;
			meshObject.SetActive(false);
			mesh.MarkDynamic();

			meshObject.GetComponent<Transform>().parent = GameManager.Instance.GameParent;

			onColor = material.GetColor("_TintColor");
			beamOnColor = beamMaterial.GetColor("_TintColor");
		}

		public void ShowAnimated(int x, int y)
		{
			SetVertices(tr.position, Settings.GetWorldCoordinate(x, y));

			targetObject.GetComponent<Transform>().position = Settings.GetWorldCoordinate(x, y) + Vector3.up * Settings.cellSize * Settings.radarArea;

			meshObject.SetActive(true);
			targetObject.SetActive(true);

			AnimateLightOn();
		}

		void SetVertices(Vector3 startPosition, Vector3 targetPosition)
		{
			var vertices = mesh.vertices;
			float targetHeight = (1 + 2 * Settings.radarArea) * 0.75f * 0.5f * Settings.cellHeight;
			float targetWidth = (1 + 2 * Settings.radarArea) * 0.5f * Settings.cellWidth;
			float targetHalfWidth = targetWidth * 0.5f;

			vertices[0] = targetPosition + new Vector3(-targetHalfWidth, 0, -targetHeight);
			vertices[1] = targetPosition + new Vector3(targetHalfWidth, 0, -targetHeight);

			vertices[2] = startPosition + new Vector3(0, 0, 0);
			vertices[3] = startPosition + new Vector3(0, 0, 0);

			vertices[4] = targetPosition + new Vector3(-targetWidth, 0, 0);
			vertices[5] = startPosition + new Vector3(0, 0, 0);

			vertices[6] = targetPosition + new Vector3(-targetHalfWidth, 0, targetHeight);
			vertices[7] = startPosition + new Vector3(0, 0, 0);

			vertices[8] = targetPosition + new Vector3(targetHalfWidth, 0, targetHeight);
			vertices[9] = startPosition + new Vector3(0, 0, 0);

			vertices[10] = targetPosition + new Vector3(targetWidth, 0, 0);
			vertices[11] = startPosition + new Vector3(0, 0, 0);

			mesh.vertices = vertices;
		}

		void AnimateLightOn()
		{
			isOn = true;

			// LeanTween.value seems to fail at times, just make sure light is turned off
			Invoke("LightOffCompletion", 1.8f * GameManager.Instance.GameSpeedInverse);

			material.SetColor("_TintColor", Color.black);
			var lightOn = LeanTween.value(gameObject, Color.black, onColor, 0.3f * GameManager.Instance.GameSpeedInverse);
			lightOn.setOnUpdateColor((Color val) =>
			{
				material.SetColor("_TintColor", val);
			});
			beamMaterial.SetColor("_TintColor", Color.black);
			var beamLightOn = LeanTween.value(gameObject, Color.black, beamOnColor, 0.3f * GameManager.Instance.GameSpeedInverse);
			beamLightOn.setOnUpdateColor((Color val) =>
			{
				beamMaterial.SetColor("_TintColor", val);
			});
			lightOn.onComplete = () =>
			{
				AnimateLightOff();
			};
			lightOn.setEase(LeanTweenType.easeInOutBounce);
		}

		void AnimateLightOff()
		{
			var lightOff = LeanTween.value(gameObject, onColor, Color.black, 0.5f * GameManager.Instance.GameSpeedInverse);
			lightOff.setOnUpdateColor((Color val) =>
			{
				material.SetColor("_TintColor", val);
			});
			var beamLightOff = LeanTween.value(gameObject, beamOnColor, Color.black, 0.5f * GameManager.Instance.GameSpeedInverse);
			beamLightOff.setOnUpdateColor((Color val) =>
			{
				beamMaterial.SetColor("_TintColor", val);
			});
			lightOff.onComplete = () =>
			{
				LightOffCompletion();
				
			};
			lightOff.setEase(LeanTweenType.easeInOutBounce);
			var delay = 0.75f + Random.value * 0.5f;
			lightOff.delay = delay * GameManager.Instance.GameSpeedInverse;			
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

			beamMaterial.SetColor("_TintColor", beamOnColor);
			material.SetColor ("_TintColor", onColor);
		}
	}
}