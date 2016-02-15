using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class MissileController : MonoBehaviour
	{
		[SerializeField]
		private GameObject explosionEffectPrefab;
		[SerializeField]
		private GameObject trail;
		[SerializeField]
		private LineRenderer targetIndicator;

		private Vector3 targetWorldPosition;
		private Transform tr;

		private bool exploded = false;

		void Awake()
		{
			tr = GetComponent<Transform>();
		}

		public void Shoot(int x, int y)
		{
			targetWorldPosition = Settings.GetWorldCoordinate(x, y);
			var midPoint = (targetWorldPosition + tr.position) * 0.5f + Vector3.up * 40;
			var controlStart = tr.position - tr.forward;
			var controlEnd = targetWorldPosition + tr.forward;
			LTSpline ltSpline = new LTSpline(new Vector3[] { controlStart, tr.position, midPoint, targetWorldPosition, controlEnd });

			var move = LeanTween.moveSpline(gameObject, ltSpline.pts, 1f);
			move.setEase(LeanTweenType.easeInCubic);
			move.setOnComplete(Explode);
			move.setOrientToPath(true);

			Invoke("Explode", 1.05f);

			SoundEffectPlayer.Instance.PlayMissile ();
		}

		void Update()
		{
			targetIndicator.SetPosition(0, tr.position);
			targetIndicator.SetPosition(1, targetWorldPosition);
		}

		void Explode()
		{
			if (exploded) return;

			trail.transform.parent = null;
			Destroy(trail, 2);
			var go = Instantiate(explosionEffectPrefab, targetWorldPosition, Quaternion.identity) as GameObject;
			go.GetComponent<Transform>().parent = GameManager.Instance.GameParent;
			SoundEffectPlayer.Instance.PlayExplosion ();
			Destroy(gameObject);
		}
	}
}