using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class UI : MonoBehaviour
{
#pragma warning disable 649
	[Header("General")]
	[SerializeField] private Image _backgroundImage;
	[SerializeField] private Button _buttonPause;
	[SerializeField] private GameObject _newHiScore;
	[Header("ExplanationScreen")]
	[SerializeField] private TextMeshProUGUI _explanationText;
	[SerializeField] private Button _buttonLetsGo;
	[Header("StartScreen")]
	[SerializeField] private GameObject _titleText;
	[SerializeField] private Button _buttonEasy;
	[SerializeField] private Button _buttonMedium;
	[SerializeField] private Button _buttonHard;
	[Header("ScoreScreen")]
	[SerializeField] private TextMeshProUGUI _scoreText;
	[SerializeField] private TextMeshProUGUI _hiscoreText;
	[SerializeField] private LifeCounterObjects _lifeCounterObjects;
	[Header("Timer")]
	[SerializeField] private CountDown _countdown;
	[SerializeField] private VisualTimer _timer;
	[Header("PauseScreen")]
	[SerializeField] private TextMeshProUGUI _pauseText;
	[SerializeField] private Button _continueButton;
	[SerializeField] private Button _restartButton_pause;
	[SerializeField] private Button _goHomeButton_pause;
	[Header("GameOverScreen")]
	[SerializeField] private TextMeshProUGUI _gameOverText;
	[SerializeField] private Button _restartButton_gameover;
	[SerializeField] private Button _goHomeButton_gameover;
	[SerializeField] private TextMeshProUGUI _yourScoreText;
	[SerializeField] private TextMeshProUGUI _hiScoreGameOverText;
#pragma warning restore 649

	// Publics:
	public UIExplanationScreen explanation { get; private set; }
	public UIScaler newHiScore { get; private set; }
	public UIImage background { get; private set; }
	public UIButton pauseButton { get; private set; }
	public UIStartScreen startScreen { get; private set; }
	public UIPauseScreen pauseScreen { get; private set; }
	public UIScoresScreen scoresScreen { get; private set; }
	public UIGameOverScreen gameOverScreen { get; private set; }
	public LifeCounterObjects lifeCounterObjects => _lifeCounterObjects;
	public CountDown countdown => _countdown;
	public VisualTimer timer => _timer;

	private List<IUIScreen> _screens = new List<IUIScreen>();
	private IEnumerator _coroutine;

#region Privates
	private void Awake()
	{
		Prefabs prefabs = GameController.refs.prefabs;
		// Reusable components:
		UIButton buttonRestart_pause = new UIButton(_restartButton_pause, () =>
		{
			StateManager.gameEvent.Invoke(GameEvent.RestartGame);
		});
		UIButton buttonRestart_gameover = new UIButton(_restartButton_gameover, () =>
		{
			StateManager.gameEvent.Invoke(GameEvent.RestartGame);
		});
		UIButton buttonEasy = new UIButton(_buttonEasy, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClick);
			GameController.SetDifficulty(Difficulty.Easy);
		});
		UIButton buttonMedium = new UIButton(_buttonMedium, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClick);
			GameController.SetDifficulty(Difficulty.Normal);
		});
		UIButton buttonHard = new UIButton(_buttonHard, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClick);
			GameController.SetDifficulty(Difficulty.Hard);
		});
		UIButton buttonLetsGo = new UIButton(_buttonLetsGo, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClickConfirm);
			StateManager.gameEvent.Invoke(GameEvent.StartCountDown);
		});
		UIButton buttonGoHome_pause = new UIButton(_goHomeButton_pause, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClickBack);
			StateManager.gameEvent.Invoke(GameEvent.GoHome);
		});
		UIButton buttonGoHome_gameover = new UIButton(_goHomeButton_gameover, () =>
		{
			AudioManager.StopMusic();
			AudioManager.PlaySound(prefabs.buttonClickBack);
			StateManager.gameEvent.Invoke(GameEvent.GoHome);
		});
		UIButton buttonContinue = new UIButton(_continueButton, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClickConfirm);
			StateManager.gameEvent.Invoke(GameEvent.Continue);
		});

		newHiScore = new UIScaler(_newHiScore);
		pauseButton = new UIButton(_buttonPause, () =>
		{
			AudioManager.PlaySound(prefabs.buttonClickBack);
			StateManager.gameEvent.Invoke(GameEvent.Pause);
		});
		background = new UIImage(_backgroundImage);
		explanation = new UIExplanationScreen(new UIText(_explanationText), buttonLetsGo);
		startScreen = new UIStartScreen(new UIScaler(_titleText), buttonEasy, buttonMedium, buttonHard);
		pauseScreen = new UIPauseScreen(new UIText(_pauseText), buttonContinue, buttonRestart_pause, buttonGoHome_pause);
		scoresScreen = new UIScoresScreen(_scoreText, _hiscoreText, _lifeCounterObjects);
		gameOverScreen = new UIGameOverScreen(new UIText(_gameOverText), new UIText(_yourScoreText), new UIText(_hiScoreGameOverText), buttonRestart_gameover, buttonGoHome_gameover);

		// Setup the timer so that it invokes a GameOver-event when the time is up
		timer.completeAction = () => StateManager.gameEvent.Invoke(GameEvent.GameOver);
	}

	private void Start()
	{
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.GoHome:
					AudioManager.PlayMusic(GameController.refs.prefabs.musicMenu, true, 1f, 1f);
					if (_coroutine != null) StopCoroutine(_coroutine);
					_coroutine = ShowHomeScreen(.5f);
					StartCoroutine(_coroutine);
					break;

				case GameEvent.ShowExplanation:
					if (_coroutine != null) StopCoroutine(_coroutine);
					_coroutine = ShowExplanation(.5f);
					StartCoroutine(_coroutine);
					break;

				case GameEvent.StartCountDown:
					AudioManager.StopMusic();
					if (_coroutine != null) StopCoroutine(_coroutine);
					_coroutine = ShowCountdown(.5f);
					StartCoroutine(_coroutine);
					newHiScore.HideImmediately();
					break;

				case GameEvent.RestartGame:
					lifeCounterObjects.Reset();
					timer.Reset();
					gameOverScreen.Hide();
					background.Hide();
					pauseScreen.Hide();
					timer.Reset();
					StateManager.gameEvent.Invoke(GameEvent.StartCountDown);
					break;

				case GameEvent.StartGame:
					AudioManager.PlayMusic(GameController.refs.prefabs.musicGame);
					lifeCounterObjects.Reset();
					timer.Reset();
					gameOverScreen.Hide();
					background.Hide();
					pauseButton.Show();
					scoresScreen.Show();
					if (Model.levelData.timeBased) timer.Setup(Model.levelData.totalTime, true, true);
					newHiScore.HideImmediately();
					break;

				case GameEvent.GameOver:
					AudioManager.StopMusic();
					AudioManager.PlaySound(GameController.refs.prefabs.failureTune);
					bool newHiscore = ScoreManager.CheckHIScores();
					if (newHiscore)
					{
						Debug.Log("Yay! Congrats, new hiscore achieved!");
						newHiScore.Show();
						AudioManager.PlaySound(GameController.refs.prefabs.hiScoreSound);
					}
					if (_coroutine != null) StopCoroutine(_coroutine);
					_hiscoreText.text = ScoreManager.hiscore.ToString();
					_yourScoreText.text = $"Your score: {ScoreManager.score.ToString()}";
					_hiScoreGameOverText.text = $"Hiscore: {ScoreManager.hiscore.ToString()}";
					scoresScreen.UpdateScore();
					gameOverScreen.Show();
					pauseButton.Hide();
					background.Show();
					lifeCounterObjects.Hide();
					if (Model.levelData.timeBased) timer.Pause();
					break;

				case GameEvent.Pause:
					AudioManager.PitchMusic(.3f);
					background.Show();
					pauseScreen.Show();
					pauseButton.Hide();
					if (Model.levelData.timeBased) timer.Pause();
					newHiScore.HideImmediately();
					break;

				case GameEvent.Continue:
					AudioManager.PitchMusic(1f);
					background.Hide();
					pauseScreen.Hide();
					pauseButton.Show();
					if (Model.levelData.timeBased) timer.StartTimer();
					newHiScore.HideImmediately();
					break;

				case GameEvent.Whack:
					ScoreManager.AddScore(1);
					scoresScreen.UpdateScore();
					break;
			}
		});
	}

	private IEnumerator ShowHomeScreen(float delay)
	{
		gameOverScreen.Hide();
		pauseScreen.Hide();
		pauseButton.Hide();
		lifeCounterObjects.Hide();
		scoresScreen.ShowHiScoreOnly();
		newHiScore.HideImmediately();
		timer.Reset();
		timer.Hide();

		yield return new WaitForSeconds(delay);

		startScreen.Show();
		background.Show();
		yield return null;
	}

	private IEnumerator ShowExplanation(float delay)
	{
		background.Show();
		explanation.levelData = Model.levelData;
		startScreen.Hide();

		yield return new WaitForSeconds(delay);

		explanation.Show();
		yield return null;
	}

	private IEnumerator ShowCountdown(float delay)
	{
		explanation.Hide();
		startScreen.Hide();
		scoresScreen.UpdateScore();
		
		yield return new WaitForSeconds(delay);
		
		countdown.StartCountDown(() =>
		{
			StateManager.gameEvent.Invoke(GameEvent.StartGame);
		});
		yield return null;
	}
#endregion
}