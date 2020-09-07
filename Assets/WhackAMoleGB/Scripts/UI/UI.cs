using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
#pragma warning disable 649
	[Header("General-assetGroup")]
	[SerializeField] private Image _backgroundImage;
	[Header("StartScreen-assetGroup")]
	[SerializeField] private TextMeshProUGUI _titleText;
	[SerializeField] private Button _buttonEasy;
	[SerializeField] private Button _buttonMedium;
	[SerializeField] private Button _buttonHard;
	[Header("Score-assetGroup")]
	[SerializeField] private TextMeshProUGUI _scoreText;
	[SerializeField] private TextMeshProUGUI _hiscoreText;
	[SerializeField] private LifeCounterObjects _lifeCounterObjects;
	[Header("Timer-assetGroup")]
	[SerializeField] private CountDown _countdown;
	[SerializeField] private VisualTimer _timer;
	[Header("PauseScreen-assetGroup")]
	[SerializeField] private TextMeshProUGUI _pauseText;
	[SerializeField] private Button _continueButton;
	[Header("GameOverScreen-assetGroup")]
	[SerializeField] private TextMeshProUGUI _gameOverText;
	[SerializeField] private Button _restartButton;
	[SerializeField] private Button _goHomeButton;
#pragma warning restore 649

	// Publics:
	public UIBackground background { get; private set; }
	public UIStartScreen startScreen { get; private set; }
	public UIPauseScreen pauseScreen { get; private set; }
	public UIScoresScreen scoresScreen { get; private set; }
	public UIGameOverScreen gameOver { get; private set; }
	public CountDown countdown => _countdown;
	public VisualTimer timer => _timer;


	private void Awake()
	{
		background = new UIBackground(_backgroundImage);
		startScreen = new UIStartScreen(_titleText, _buttonEasy, _buttonMedium, _buttonHard);
		pauseScreen = new UIPauseScreen(_pauseText, _continueButton, _restartButton, _goHomeButton);
		scoresScreen = new UIScoresScreen(_scoreText, _hiscoreText, _lifeCounterObjects);
		gameOver = new UIGameOverScreen(_gameOverText, _restartButton, _goHomeButton);
		timer.completeAction = () => StateManager.gameEvent.Invoke(GameEvent.GameOver);
	}

	private void Start()
	{
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.GoHome:
					Debug.Log("UI Home");
					startScreen.Show();
					break;

				case GameEvent.StartCountDown:
					startScreen.Hide();
					scoresScreen.UpdateScore();
					countdown.StartCountDown(() =>
					{
						StateManager.gameEvent.Invoke(GameEvent.StartGame);
					});
					break;

				case GameEvent.StartGame:
					Debug.Log("UI Startgame");
					scoresScreen.Show();
					timer.Setup(20f, true, true);
					break;

				case GameEvent.GameOver:
					gameOver.Show();
					break;
			}
		});
	}
}







/**
<summary>
General
</summary>
*/
public class UIBackground
{
	private Image _backgroundImage;

	public UIBackground(Image backgroundImage)
	{
		_backgroundImage = backgroundImage;
	}

	public void Show()
	{
		float duration = .3f;
		Ease ease = Ease.OutExpo;

		_backgroundImage.gameObject.SetActive(true);
		_backgroundImage.DOFade(.8f, duration).SetEase(ease);
	}

	public void Hide()
	{
		float duration = .25f;
		Ease ease = Ease.InExpo;

		_backgroundImage.DOFade(0f, duration).SetEase(ease).OnComplete(() => _backgroundImage.gameObject.SetActive(false));
	}
}

/**
<summary>
</summary>
*/
public class UIStartScreen
{
	private TextMeshProUGUI _titleText;
	private List<Button> _buttons = new List<Button>();
	private List<LevelData> _levelDatas = new List<LevelData>();
	private List<CanvasGroup> _canvasGroups = new List<CanvasGroup>();

	private bool isShowing = false;

	public UIStartScreen(TextMeshProUGUI titleText, Button buttonEasy, Button buttonMedium, Button buttonHard)
	{
		_titleText = titleText;
		_buttons.AddRange( new Button[3]{buttonEasy, buttonMedium, buttonHard });
		_levelDatas.AddRange( new LevelData[3]{GameController.refs.prefabs.levelEasy, GameController.refs.prefabs.levelMedium, GameController.refs.prefabs.levelHard });
		for (int i = 0; i < _buttons.Count; i++) {
			LevelData ld = _levelDatas[i];
			_buttons[i].onClick.AddListener(()=>{
				if(!isShowing) return;
				Model.levelData = ld;
				StateManager.gameEvent.Invoke(GameEvent.StartCountDown);
			});
			CanvasGroup cg = _buttons[i].GetComponent<CanvasGroup>();
			cg.alpha = 0f;
			_canvasGroups.Add(cg);
			_buttons[i].gameObject.SetActive(false);
		}
		_titleText.alpha = 0f;
		_titleText.gameObject.SetActive(false);
	}

	public void Show()
	{
		if(isShowing) return;
		isShowing = true;

		float duration = .3f;
		Ease ease = Ease.OutExpo;

		GameController.refs.ui.background.Show();

		int n = 0;
		_canvasGroups.ForEach(cg => { cg.DOFade(1f, duration).SetEase(ease).SetDelay(n++*.1f).OnStart(()=>{cg.gameObject.SetActive(true);}); });
		_titleText.DOFade(1f, duration).SetEase(ease).OnStart(()=>{_titleText.gameObject.SetActive(true);});
	}

	public void Hide()
	{
		if(!isShowing) return;
		isShowing = false;

		float duration = .25f;
		Ease ease = Ease.InExpo;

		GameController.refs.ui.background.Hide();
		_canvasGroups.ForEach(cg => {
			cg.DOFade(0f, duration).SetEase(ease).OnComplete(() => cg.gameObject.SetActive(false));
		});
		_titleText.DOFade(0f, duration).SetEase(ease).OnComplete(() => _titleText.gameObject.SetActive(false));
	}
}

/**
<summary>
When the user pauses the game this handles all the items of the pause-screen
</summary>
*/
public class UIPauseScreen
{
	private References _refs;
	private TextMeshProUGUI _pauseText;
	private Button _continueButton;
	private CanvasGroup _continueButtonCG;
	private Button _restartButton;
	private CanvasGroup _restartButtonCG;
	private Button _goHomeButton;
	private CanvasGroup _goHomeButtonCG;

	public UIPauseScreen(TextMeshProUGUI pauseText, Button continueButton, Button restartButton, Button goHomeButton)
	{
		_refs = GameController.refs;
		_pauseText = pauseText;
		_continueButton = continueButton;
		_continueButtonCG = continueButton.GetComponent<CanvasGroup>();
		_restartButton = restartButton;
		_restartButtonCG = _restartButton.GetComponent<CanvasGroup>();
		_goHomeButton = goHomeButton;
		_goHomeButtonCG = _goHomeButton.GetComponent<CanvasGroup>();

		_pauseText.alpha = 0f;
		_pauseText.gameObject.SetActive(false);
		_continueButton.gameObject.SetActive(false);
		_restartButton.gameObject.SetActive(false);
		_goHomeButton.gameObject.SetActive(false);
	}

	/**
	<summary>
	Shows a pausetext and the continue- restart- and gohome-button
	</summary>
	*/
	public void Show()
	{
		float duration = .3f;
		Ease ease = Ease.OutExpo;

		_refs.ui.background.Show();
		_pauseText.gameObject.SetActive(true);
		_pauseText.DOFade(1f, duration).SetEase(ease);
		_continueButton.gameObject.SetActive(true);
		_continueButtonCG.DOFade(1f, duration).SetEase(ease);
		_restartButton.gameObject.SetActive(true);
		_restartButtonCG.DOFade(1f, duration).SetEase(ease);
		_goHomeButton.gameObject.SetActive(true);
		_goHomeButtonCG.DOFade(1f, duration).SetEase(ease);
	}

	/**
	<summary>
	</summary>
	*/
	public void Hide()
	{
		float duration = .25f;
		Ease ease = Ease.InExpo;

		_refs.ui.background.Hide();
		_pauseText.DOFade(0f, duration).SetEase(ease).OnComplete(() => _pauseText.gameObject.SetActive(false));
		_continueButtonCG.DOFade(0f, duration).SetEase(ease).OnComplete(() => _continueButton.gameObject.SetActive(false));
		_restartButtonCG.DOFade(0f, duration).SetEase(ease).OnComplete(() => _restartButton.gameObject.SetActive(false));
		_goHomeButtonCG.DOFade(0f, duration).SetEase(ease).OnComplete(() => _goHomeButton.gameObject.SetActive(false));
	}
}

/**
<summary>
</summary>
*/
public class UIScoresScreen
{
	private TextMeshProUGUI _scoreText;
	private TextMeshProUGUI _hiscoreText;
	private LifeCounterObjects _lifeCounterObjects;
	private RectTransform _scoreTextRT => _scoreText.transform as RectTransform;
	private RectTransform _hiscoreTextRT => _hiscoreText.transform as RectTransform;

	private float hideYPos = 80f;
	private float showYPos = -82.2f;

	public UIScoresScreen(TextMeshProUGUI scoreText, TextMeshProUGUI hiscoreText, LifeCounterObjects lifeCounterObjects)
	{
		_scoreText = scoreText;
		_hiscoreText = hiscoreText;
		_lifeCounterObjects = lifeCounterObjects;

		Vector3 _scorePos = _scoreTextRT.anchoredPosition;
		_scorePos.y = hideYPos;
		Vector3 _hiscorePos = _hiscoreTextRT.anchoredPosition;
		_hiscorePos.y = hideYPos;
		_scoreTextRT.anchoredPosition = _scorePos;
		_hiscoreTextRT.anchoredPosition = _hiscorePos;

		UpdateScore();

		_scoreText.gameObject.SetActive(false);
		_hiscoreText.gameObject.SetActive(false);
	}

	public void Show()
	{
		float duration = .3f;
		Ease ease = Ease.OutExpo;

		_scoreText.gameObject.SetActive(true);
		_scoreTextRT.DOAnchorPosY(showYPos, duration).SetEase(ease);
		_hiscoreText.gameObject.SetActive(true);
		_hiscoreTextRT.DOAnchorPosY(showYPos, duration).SetEase(ease);
		_lifeCounterObjects.Show();

	}

	public void Hide()
	{
		float duration = .25f;
		Ease ease = Ease.InExpo;
		_scoreTextRT.DOAnchorPosY(80f, duration).SetEase(ease).OnComplete(() =>
		{
			_scoreText.gameObject.SetActive(false);
		});
		_hiscoreTextRT.DOAnchorPosY(80f, duration).SetEase(ease).OnComplete(() =>
		{
			_hiscoreText.gameObject.SetActive(false);
		});
		_lifeCounterObjects.Hide();
	}

	public void UpdateScore()
	{
		_scoreText.text = ScoreManager.score.ToString();
		_hiscoreText.text = ScoreManager.hiscore.ToString();
	}
}

/**
<summary>
</summary>
*/
public class UIGameOverScreen
{
	private TextMeshProUGUI _gameOverText;

	public UIGameOverScreen(TextMeshProUGUI gameOverText, Button restartButton, Button goHomeButton)
	{
		_gameOverText = gameOverText;
		_gameOverText.alpha = 0f;
		_gameOverText.gameObject.SetActive(false);
	}

	public void Show()
	{
		float duration = .3f;
		Ease ease = Ease.OutExpo;

		GameController.refs.ui.background.Show();
		_gameOverText.gameObject.SetActive(true);
		_gameOverText.DOFade(1f, duration).SetEase(ease);
	}

	public void Hide()
	{
		float duration = .25f;
		Ease ease = Ease.InExpo;

		GameController.refs.ui.background.Hide();
		GameController.refs.ui.timer.Hide();
		_gameOverText.DOFade(0f, duration).SetEase(ease).OnComplete(() => _gameOverText.gameObject.SetActive(false));
	}
}