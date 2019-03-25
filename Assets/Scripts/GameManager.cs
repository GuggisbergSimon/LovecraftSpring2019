using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] private AudioMixer generalMixer = null;
	[SerializeField] private Message endMessage = null;
	public static GameManager Instance { get; private set; }
	private Coroutine _timeScaleCoroutine;
	private UIManager _uiManager;
	public UIManager UIManager => _uiManager;
	private PlayerController _player;
	public PlayerController Player => _player;
	private bool _fadeOutToBlack = false;
	private bool _isQuitting;
	private int _numberNotesRequired;
	private int _currentNumberNotes;
	private float _sensitivityX = 2.0f;
	private float _sensitivityY = 1.5f;
	private bool _invertX;
	private bool _invertY;
	public float SensitivityX
	{
		get => _sensitivityX;
		set => _sensitivityX = value;
	}
	public float SensitivityY
	{
		get => _sensitivityY;
		set => _sensitivityY = value;
	}

	public bool InvertX
	{
		get => _invertX;
		set => _invertX = value;
	}

	public bool InvertY
	{
		get => _invertY;
		set => _invertY = value;
	}

	public int CurrentNumberNotes
	{
		get => _currentNumberNotes;
		set
		{
			_currentNumberNotes = value;
			if (_currentNumberNotes >= _numberNotesRequired)
			{
				_uiManager.PrintPopUp(endMessage);
				//todo add code here to unlock WinEnd (come back to start or something
			}
		}
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
		_numberNotesRequired = FindObjectsOfType<PickUpNote>().Length;
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
		while (_uiManager.IsFadingToBlack)
		{
			yield return null;
		}

		LoadLevel(nameLevel);
	}

	public void ChangeTimeScale(float timeScale)
	{
		if (Time.timeScale.CompareTo(0) == 0)
		{
			AudioListener.pause = true;
		}
		else
		{
			AudioListener.pause = false;
			generalMixer.SetFloat("Pitch", Time.timeScale);
		}

		Time.timeScale = timeScale;
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