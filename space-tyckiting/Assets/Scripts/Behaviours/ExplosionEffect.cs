using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	public class ExplosionEffect : MonoBehaviour
	{
		[SerializeField]
		private Light effectLight;
		[SerializeField]
		private ParticleSystem effectParticles;

		[SerializeField]
		private float lightIntensity = 8;

		[SerializeField]
		private float animateInTime = 0.25f;
		[SerializeField]
		private float animateStayTime = 0f;
		[SerializeField]
		private float animateOutTime = 1f;

		// Use this for initialization
		void Start()
		{
			if (effectLight != null)
			{
				effectLight.intensity = 0;
				effectLight.enabled = false;
			}
			
			Play();
		}

		void Play()
		{
			if (effectLight != null) AnimateLightOn();
			
			Destroy(gameObject, animateInTime + animateStayTime + animateOutTime + 0.1f);

			effectParticles.Play();
		}

		void AnimateLightOn()
		{
			effectLight.enabled = true;
			var lightOn = LeanTween.value(gameObject, 0, lightIntensity, animateInTime);
			lightOn.setOnUpdate((float val) =>
			{
				effectLight.intensity = val;
			});
			lightOn.onComplete = () =>
			{
				AnimateLightOff();
			};
			lightOn.setEase(LeanTweenType.easeOutBounce);
		}

		void AnimateLightOff()
		{
			var lightOff = LeanTween.value(gameObject, lightIntensity, 0, animateOutTime);
			lightOff.setOnUpdate((float val) =>
			{
				effectLight.intensity = val;
			});
			lightOff.onComplete = () =>
			{
				Destroy(gameObject);
			};
			lightOff.delay = animateStayTime;
			lightOff.setEase(LeanTweenType.easeInQuad);
		}
	}
}