using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SpaceTyckiting
{
	public class SideMenuController : MonoBehaviour
	{
		[SerializeField]
		private GameObject uiRoot;

		[SerializeField]
		private ToggleGroup qualityGroup;
		[SerializeField]
		private Toggle qualityLow;
		[SerializeField]
		private Toggle qualityMid;
		[SerializeField]
		private Toggle qualityHigh;

		[SerializeField]
		private Toggle musicToggle;
		[SerializeField]
		private Toggle soundsToggle;

		private bool initialized = false;

		void OnEnable()
		{
			GameManager.GameLoaded += GameManager_GameLoaded;
		}

		void Disable()
		{
			GameManager.GameLoaded -= GameManager_GameLoaded;
		}

		void GameManager_GameLoaded()
		{
			HideMenu();
		}

		void Awake()
		{
			float quality = QualitySettings.GetQualityLevel();
			if (quality <= 0) qualityLow.isOn = true;
			else if (quality == 1) qualityMid.isOn = true;
			else qualityHigh.isOn = true;

			musicToggle.isOn = PlayerPrefs.GetInt("music_on", 1) > 0;
			soundsToggle.isOn = PlayerPrefs.GetInt("sounds_on", 1) > 0;

			Application.targetFrameRate = quality >= 2 ? 60 : 30;
		}

		void Start()
		{
			initialized = true;
		}

		public void ToggleMenu()
		{
			uiRoot.SetActive(!uiRoot.activeInHierarchy);
		}

		public void HideMenu()
		{
			uiRoot.SetActive(false);
		}

		public void SetQuality(int level)
		{
			if (initialized)
			{
				QualitySettings.SetQualityLevel(level);
				Application.targetFrameRate = level >= 2 ? 60 : 30;

				PlayerPrefs.SetInt("quality_level", level);

				PlayerPrefs.Save();
			}
		}

		public void ToggleSounds()
		{
			if (initialized)
			{
				PlayerPrefs.SetInt("sounds_on", soundsToggle.isOn? 1 : 0);

				PlayerPrefs.Save();
			}
		}

		public void ToggleMusic()
		{
			if (initialized)
			{
				PlayerPrefs.SetInt("music_on", musicToggle.isOn ? 1 : 0);

				PlayerPrefs.Save();
			}
		}
	}
}