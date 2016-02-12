using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class HealthBarController : MonoBehaviour
	{
		[SerializeField]
		private Vector3 offset = new Vector3(0, 9, 0);

		[SerializeField]
		private bool showWhenFull = false;

		[SerializeField]
		private Renderer mainRenderer;

		[SerializeField]
		private Color healthyColor = Color.green;

		[SerializeField]
		private Color warningColor = Color.red;

		[SerializeField]
		private float warningThreshold = 0.2f;

		private float maxWidth;

		private Transform tr;

		private bool initialized = false;

		private LTDescr sizeAnimation;

		private Transform _target;
		public Transform Target 
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
				SetPosition();
			}
		}

		public void SetPosition()
		{
			if (!initialized) Initialize();

			tr.position = _target.position + offset;
		}

		public void SetSize(float value, float max, bool animate)
		{
			if (!initialized) Initialize();

			mainRenderer.enabled = value < max || showWhenFull;

			var width = value / max;

			if (width <= warningThreshold) mainRenderer.material.SetColor("_TintColor", warningColor);
			else mainRenderer.material.SetColor("_TintColor", healthyColor);

			if (animate)
			{
				if (sizeAnimation != null) LeanTween.cancel(gameObject, sizeAnimation.id);

				sizeAnimation = LeanTween.scaleX(gameObject, width * maxWidth, 2);
				sizeAnimation.setEase(LeanTweenType.easeInQuad);
			}
			else
			{
				tr.localScale = new Vector3(width * maxWidth, tr.localScale.y, tr.localScale.z);
			}
		}

		void Initialize()
		{
			initialized = true;

			tr = GetComponent<Transform>();

			maxWidth = tr.localScale.x;
		}

		void Update()
		{
			if (Target != null) SetPosition();
		}
	}
}