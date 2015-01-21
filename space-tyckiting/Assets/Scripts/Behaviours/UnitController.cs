using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class UnitController : MonoBehaviour
	{
		[SerializeField]
		private MissileController missilePrefab;
		[SerializeField]
		private SearchlightController searchlight;
		[SerializeField]
		private int maxHitPoints = 10;
		[SerializeField]
		private GameObject destroyEffect;
		[SerializeField]
		private GameObject modelRoot;
		[SerializeField]
		private HealthBarController healthBarPrefab;
		[SerializeField]
		private Transform missileSpawnPos;
		[SerializeField]
		private GameObject spottedIndicatorPrefab;
		[SerializeField]
		private ParticleSystem spottedParticles;

		public BotData Data { get; set; }

		private LTDescr spotFade;

		private Renderer spottedIndicator;

		public int PositionX { get; private set; }
		public int PositionY { get; private set; }

		public int HitPoints { get { return maxHitPoints - damage; } }

		public int FactionId { get; set; }
		public int ActorId { get; set; }

		private float heightOffsetPerUnit = 9;

		private Transform tr;

		private HealthBarController healthBar;

		private int damage = 0;

		private bool isDead = false;

		public Vector3 GridWorldPos { get; private set; }

		void Awake()
		{
			tr = GetComponent<Transform>();
			CreateHealthBar();
			CreateSpottedIndicator();
		}

		void CreateSpottedIndicator()
		{
			var go = Instantiate(spottedIndicatorPrefab.gameObject) as GameObject;
			go.GetComponent<Transform>().parent = GameManager.Instance.GameParent;
			spottedIndicator = go.GetComponent<Renderer>();
			go.SetActive(false);
		}

		void CreateHealthBar()
		{
			var go = Instantiate(healthBarPrefab.gameObject) as GameObject;
			go.GetComponent<Transform>().parent = GameManager.Instance.GameParent;
			healthBar = go.GetComponent<HealthBarController>();
			healthBar.Target = modelRoot.GetComponent<Transform>();
			healthBar.SetSize(maxHitPoints, maxHitPoints, false);
		}

		public void SetPos(int x, int y)
		{
			PositionX = x;
			PositionY = y;

			var offset = LeanTween.moveLocalY(modelRoot, GameManager.Instance.UnitsInCell(x, y) * heightOffsetPerUnit, 0.75f);
			offset.setEase(LeanTweenType.easeInExpo);

			GridWorldPos = Settings.GetWorldCoordinate(PositionX, PositionY);

			tr.position = GridWorldPos;
		}

		public void MoveTo(int x, int y)
		{
			var targetPosition = Settings.GetWorldCoordinate(x, y);
		
			var targetRotation = Quaternion.LookRotation(targetPosition - tr.position).eulerAngles;
			var rotate = LeanTween.rotate(gameObject, targetRotation, 0.6f);
			rotate.setEase(LeanTweenType.easeOutExpo);

			var offset = LeanTween.moveLocalY(modelRoot, GameManager.Instance.UnitsInCell(x, y) * heightOffsetPerUnit, 0.75f);
			offset.setEase(LeanTweenType.easeInExpo);

			var move = LeanTween.move(gameObject, targetPosition, 1);
			move.setEase(LeanTweenType.easeInOutQuad);
			move.delay = Random.Range(0, 0.2f);

			PositionX = x;
			PositionY = y;

			GridWorldPos = targetPosition;
		}

		public void MoveBy(int x, int y)
		{
			PositionX += x;
			PositionY += y;

			MoveTo(x, y);
		}

		public void Shoot(int x, int y, float maxDelay = 0)
		{
			if (maxDelay > 0) StartCoroutine(Shoot_Coroutine(x, y, maxDelay));
			else ShootImmediate(x, y);
		}

		private IEnumerator Shoot_Coroutine(int x, int y, float maxDelay)
		{
			yield return new WaitForSeconds(Random.Range(0, maxDelay));

			ShootImmediate(x, y);
		}

		private void ShootImmediate(int x, int y)
		{
			if (x != PositionX && y != PositionY)
			{
				var targetPosition = Settings.GetWorldCoordinate(x, y);
				var targetRotation = Quaternion.LookRotation(targetPosition - tr.position).eulerAngles;
				var rotate = LeanTween.rotate(gameObject, targetRotation, 0.3f);
				rotate.setEase(LeanTweenType.easeOutExpo);
			}

			var go = Instantiate(missilePrefab.gameObject, missileSpawnPos.position, missileSpawnPos.rotation) as GameObject;
			go.GetComponent<Transform>().parent = GameManager.Instance.GameParent;
			var missile = go.GetComponent<MissileController>();
			missile.Shoot(x, y);
		}

		public void Radar(int x, int y)
		{
			if (x == PositionX && y == PositionY)
			{
				searchlight.ShowAnimated(x, y);
			}
			else
			{
				var targetPosition = Settings.GetWorldCoordinate(x, y);
				var targetRotation = Quaternion.LookRotation(targetPosition - tr.position).eulerAngles;
				var rotate = LeanTween.rotate(gameObject, targetRotation, 0.2f);
				rotate.setEase(LeanTweenType.easeInOutExpo);
				rotate.setOnComplete(() => { searchlight.ShowAnimated(x, y); });
				rotate.delay = Random.Range(0, 0.2f);
			}
		}

		public void TakeDamage(int amount)
		{
			damage += amount;

			if (isDead) return;

			healthBar.SetSize(maxHitPoints - damage, maxHitPoints, true);

			if (damage >= maxHitPoints) Die();
		}

		public void Die()
		{
			if (isDead) return;

			isDead = true;

			Destroy(healthBar.gameObject);

			Destroy(spottedIndicator);

			Instantiate(destroyEffect, GridWorldPos, Quaternion.identity);

			Destroy(gameObject);
		}

		public void Reveal(bool wasHiddenLastTurn)
		{
			if (spotFade != null) spotFade.cancel();

			spottedIndicator.gameObject.SetActive(true);
			var color = spottedIndicator.material.GetColor("_TintColor");
			color.a = 1;
			spottedIndicator.material.SetColor("_TintColor", color);
			var spTr = spottedIndicator.GetComponent<Transform>();
			var pos = modelRoot.GetComponent<Transform>().position;
			pos.x = GridWorldPos.x;
			pos.z = GridWorldPos.z;
			spTr.position = pos;
			spotFade = LeanTween.alpha(spottedIndicator.gameObject, 0, 8);

			if (wasHiddenLastTurn) spottedParticles.Play();
		}
	}
}