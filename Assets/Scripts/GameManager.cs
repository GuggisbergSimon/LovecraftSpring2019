using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] private float fadeInTimescaleTime = 0.1f;
	public static GameManager Instance { get; private set; }
	private Coroutine _timeScaleCoroutine;
	private UIManager _uiManager;
	private PlayerController _player;
	public PlayerController Player => _player;
	private bool _fadeOutToBlack = false;
	private bool _isQuitting;

	public bool FadeOutToBlack
	{
		get => _fadeOutToBlack;
		set => _fadeOutToBlack = value;
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoadingScene;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoadingScene;
	}

	//this function is activated every time a scene is loaded
	private void OnLevelFinishedLoadingScene(Scene scene, LoadSceneMode mode)
	{
		Setup();
		if (_fadeOutToBlack)
		{
			_uiManager.FadeToBlack(false);
			_fadeOutToBlack = false;
		}
	}

	private void Setup()
	{
		//alternative way to get elements. cons : if there is no element with such tag it creates an error
		//_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		_player = FindObjectOfType<PlayerController>();
		_uiManager = FindObjectOfType<UIManager>();
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		Setup();
	}

	public void LoadLevel(string nameLevel)
	{
		SceneManager.LoadScene(nameLevel);
	}


	public void LoadLevelFadeInAndOut(string nameLevel)
	{
		_uiManager.FadeToBlack(true);
		_fadeOutToBlack = true;
		StartCoroutine(LoadingLevel(nameLevel));
	}

	public void LoadLevel(string nameLevel, bool fadeInToBlack, bool fadeOutToBlack)
	{
		if (fadeInToBlack)
		{
			//_player.StopMoving();
			_uiManager.FadeToBlack(true);
			StartCoroutine(LoadingLevel(nameLevel));
		}
		else
		{
			LoadLevel(nameLevel);
		}

		_fadeOutToBlack = fadeOutToBlack;
	}

	private IEnumerator LoadingLevel(string nameLevel)
	{
		/*while (UIManager.IsFadingToBlack)
		{
			yield return null;
		}*/
		yield return null;
		LoadLevel(nameLevel);
	}

	public void ChangeTimeScale(float timeScale)
	{
		if (_timeScaleCoroutine != null)
		{
			StopCoroutine(_timeScaleCoroutine);
		}

		if (fadeInTimescaleTime.CompareTo(0) > 0)
		{
			_timeScaleCoroutine = StartCoroutine(ChangingTimeScale(timeScale));
		}
		else
		{
			Time.timeScale = timeScale;
		}
	}

	private IEnumerator ChangingTimeScale(float timeScale)
	{
		float timer = 0.0f;
		float initTimeScale = Time.timeScale;
		while (timer < fadeInTimescaleTime)
		{
			timer += Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Lerp(initTimeScale, timeScale, timer / fadeInTimescaleTime);
			yield return null;
		}
	}

	private IEnumerator QuittingGame()
	{
		while (_uiManager.IsFadingToBlack)
		{
			yield return null;
		}
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public void ReloadCurrentScene()
	{
		LoadLevel(SceneManager.GetActiveScene().name, true, true);
	}

	public void QuitGame()
	{
		if (!_isQuitting)
		{
			_isQuitting = true;
			_uiManager.FadeToBlack(true);
			StartCoroutine(QuittingGame());
		}
	}
}