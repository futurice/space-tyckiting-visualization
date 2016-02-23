using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace SpaceTyckiting
{
	public class GameDataLoader : MonoBehaviour
	{
		public static event System.Action<GameplayData> GameplayDataLoaded;

		public GameplayData LastLoadedData { get; private set; }

		[SerializeField]
		private GameObject loadingIndicator;
		[SerializeField]
		private TextAsset testJsonFile;

		private bool _isLoading;
		public bool IsLoading 
		{
			get
			{
				return _isLoading;
			}
			private set
			{
				_isLoading = value;
				if (loadingIndicator != null) loadingIndicator.SetActive(_isLoading);
			}
		}

		void Start()
		{
			IsLoading = false;

			if (Application.isEditor) 
			{
				LoadGame (null);
			}
			else if (Application.isWebPlayer || Application.platform == RuntimePlatform.WebGLPlayer) 
			{
				Application.ExternalCall ("OnUnityReady");
			}
			else 
			{
				string[] arguments = System.Environment.GetCommandLineArgs();

				foreach (var arg in arguments)
				{
					var parts = arg.Split(new string[]{"="}, System.StringSplitOptions.RemoveEmptyEntries);
					Debug.Log(string.Join(" ", parts));
					if (parts.Length == 2 && parts[0] == "timescale")
					{
						int timeScale = 1;
						System.Int32.TryParse(parts[1], out timeScale);
						Time.timeScale = timeScale;
					}
					else if (parts.Length == 2 && parts[0] == "path")
					{
						#if UNITY_STANDALONE_WIN
						LoadGame("file:///" + parts[1]);
						#endif
						#if UNITY_STANDALONE_OSX
						LoadGame("file://" + parts[1]);
						#endif
					}
					else if (parts.Length == 2 && parts[0] == "url")
					{
						StartCoroutine(LoadGame_Coroutine(parts[1]));
					}
				}
			}
		}

		public void LoadGame(string url)
		{
			if (IsLoading) return;

			StartCoroutine(LoadGame_Coroutine(url));
		}

		public void LoadGameJson(string json)
		{
			var data = GameplayData.FromJson(json);

			HandleGameLoaded(data);
		}

		public void SetTimeScale(float timeScale)
		{
			Time.timeScale = timeScale;
		}

		void HandleGameLoaded(GameplayData data)
		{
			LastLoadedData = data;

			if (GameplayDataLoaded != null) GameplayDataLoaded(data);
		}

		IEnumerator LoadGame_Coroutine(string url)
		{
			IsLoading = true;

			Debug.Log("Loading game from " + url);

			if (url == null || url == "test")
			{
				yield return new WaitForSeconds(1);

				var data = GameplayData.FromJson(testJsonFile.text);

				IsLoading = false;

				HandleGameLoaded(data);

				yield break;
			}

			var www = new WWW(url);

			yield return www;

			if (www.error != null && www.error != string.Empty)
			{
				Debug.LogError("Error loading game: " + www.error);
			}
			else
			{
				var json = www.text;

				LoadGameJson (json);
			}
			IsLoading = false;
		}
 	
	}
}