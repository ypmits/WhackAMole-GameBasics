using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Text;

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


	private void Awake()
	{
		Prefabs prefabs = GameController.refs.prefabs;
		// Reusable components:
		UIButton buttonRestart_pause = new UIButton(_restartButton_pause, () => {
			StateManager.gameEvent.Invoke(GameEvent.RestartGame); 
		});
		UIButton buttonRestart_gameover = new UIButton(_restartButton_gameover, () => {
			StateManager.gameEvent.Invoke(GameEvent.RestartGame); 
		});
		UIButton buttonEasy = new UIButton(_buttonEasy, () => {
			AudioManager.PlaySound(prefabs.buttonClick);
			Model.levelData = GameController.refs.prefabs.levelEasy;
			StateManager.gameEvent.Invoke(GameEvent.ShowExplanation); 
		});
		UIButton buttonMedium = new UIButton(_buttonMedium, () => {
			AudioManager.PlaySound(prefabs.buttonClick);
			Model.levelData = GameController.refs.prefabs.levelMedium;
			StateManager.gameEvent.Invoke(GameEvent.ShowExplanation); 
		});
		UIButton buttonHard = new UIButton(_buttonHard, () => {
			AudioManager.PlaySound(prefabs.buttonClick);
			Model.levelData = GameController.refs.prefabs.levelHard;
			StateManager.gameEvent.Invoke(GameEvent.ShowExplanation); 
		});
		UIButton buttonLetsGo = new UIButton(_buttonLetsGo, () => {
			AudioManager.PlaySound(prefabs.buttonClickConfirm);
			StateManager.gameEvent.Invoke(GameEvent.StartCountDown); 
		});
		UIButton buttonGoHome_pause = new UIButton(_goHomeButton_pause, () => {
			AudioManager.PlaySound(prefabs.buttonClickBack);
			StateManager.gameEvent.Invoke(GameEvent.GoHome); 
		});
		UIButton buttonGoHome_gameover = new UIButton(_goHomeButton_gameover, () => {
			AudioManager.PlaySound(prefabs.buttonClickBack);
			StateManager.gameEvent.Invoke(GameEvent.GoHome); 
		});
		UIButton buttonContinue = new UIButton(_continueButton, () => {
			AudioManager.PlaySound(prefabs.buttonClickConfirm);
			StateManager.gameEvent.Invoke(GameEvent.Continue); 
		});

		newHiScore = new UIScaler(_newHiScore);
		pauseButton = new UIButton(_buttonPause, () => {
			AudioManager.PlaySound(prefabs.buttonClickBack);
			StateManager.gameEvent.Invoke(GameEvent.Pause); });
		background = new UIImage(_backgroundImage);
		explanation = new UIExplanationScreen(this, new UIText(_explanationText), buttonLetsGo);
		startScreen = new UIStartScreen(this, new UIScaler(_titleText), buttonEasy, buttonMedium, buttonHard);
		pauseScreen = new UIPauseScreen(this, new UIText(_pauseText), buttonContinue, buttonRestart_pause, buttonGoHome_pause);
		scoresScreen = new UIScoresScreen(this, _scoreText, _hiscoreText, _lifeCounterObjects);
		gameOverScreen = new UIGameOverScreen(this, new UIText(_gameOverText), new UIText(_yourScoreText), new UIText(_hiScoreGameOverText), buttonRestart_gameover, buttonGoHome_gameover);

		timer.completeAction = () => StateManager.gameEvent.Invoke(GameEvent.GameOver);
	}

	private void Start()
	{
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.GoHome:
					AudioManager.PlayMusic(GameController.refs.prefabs.musicMenu);
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

	public void AddActiveScreen(IUIScreen screen)
	{
		if (_screens.Contains(screen)) return;
		_screens.Add(screen);
	}

	public void RemoveActiveScreen(IUIScreen screen)
	{
		if (!_screens.Contains(screen)) return;
		_screens.Remove(screen);
	}

	private IEnumerator ShowHomeScreen(float delay)
	{
		gameOverScreen.Hide();
		pauseScreen.Hide();
		pauseButton.Hide();
		lifeCounterObjects.Hide();
		// scoresScreen.Hide();
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
		explanation.text = Model.levelData;
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
}







/**
<summary>
General purpose Image which has all tweening-functionality inside
</summary>
*/
public interface IUIElement { void Show(); void Hide(); }
public interface IUIScreen { void Show(); void Hide(); }

public class UIImage : IUIElement
{
	private Image _img;

	public bool IsShowing;

	public UIImage(Image img)
	{
		_img = img;
		_img.color = new Color(_img.color.r, _img.color.g, _img.color.b, 0f);
		_img.gameObject.SetActive(false);
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		_img.gameObject.SetActive(true);
		_img.DOFade(.8f, .3f)
			.SetEase(Ease.OutExpo)
			.SetAutoKill();
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		_img.DOFade(0f, .25f)
			.SetEase(Ease.InExpo)
			.SetAutoKill()
			.OnComplete(() => _img.gameObject.SetActive(false));
	}
}

/**
<summary>
</summary>
*/
public class UIScaler : IUIElement
{
	private RectTransform _rt;

	public UIScaler(GameObject gameObject)
	{
		_rt = gameObject.transform as RectTransform;
		_rt.localScale = new Vector3(0f, 0f, 0f);
		_rt.gameObject.SetActive(false);
	}

	public void Show()
	{
		_rt.gameObject.SetActive(true);
		_rt.DOScale(1f, .35f).SetEase(Ease.OutExpo);
	}

	public void Hide()
	{
		_rt.DOScale(0f, .25f).SetEase(Ease.OutExpo).OnComplete(()=>{
			_rt.gameObject.SetActive(false);
		});
	}

	public void HideImmediately()
	{
		_rt.gameObject.SetActive(false);
	}
}

/**
<summary>
General purpose TMPro-text which has all tweening-functionality inside
</summary>
*/
public class UIText : IUIElement
{
	private TextMeshProUGUI _text;

	public bool IsShowing;
	public LevelData text
	{
		set
		{
			string s = new StringBuilder(value.explanation).ToString();
			s = s.Replace("{{startSpeed}}", value.spawnSpeed.ToString());
			s = s.Replace("{{totalTime}}", value.totalTime.ToString());
			_text.text = s;
		}
	}

	public UIText(TextMeshProUGUI text)
	{
		_text = text;
		_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 0f);
		_text.gameObject.SetActive(false);
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		_text.gameObject.SetActive(true);
		_text.DOFade(1f, .3f)
			.SetEase(Ease.OutExpo)
			.SetAutoKill();
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		_text.DOFade(0f, .25f)
			.SetEase(Ease.InExpo)
			.OnComplete(() => _text.gameObject.SetActive(false))
			.SetAutoKill();
	}
}

/**
<summary>
General purpose UIButton which has all tweening-functionality inside
Use this to quickly build a UIButton with predefined base-functionality
- clickAction: when the user pushes the button
</summary>
*/
public class UIButton : IUIElement
{
	private Button _button;
	private UnityAction _clickAction;
	private RectTransform _rt => _button.transform as RectTransform;
	private float _smallScale = .2f;

	public bool IsShowing;

	public UIButton(Button button, UnityAction clickAction = null)
	{
		_button = button;
		_clickAction = clickAction;
		_rt.localScale = new Vector3(_smallScale, _smallScale, 1f);
		_button.onClick.AddListener(() =>
		{
			if (!IsShowing || clickAction == null) return;
			clickAction.Invoke();
		});
		_button.gameObject.SetActive(false);
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		float duration = .3f;
		float delay = .1f;
		Ease ease = Ease.OutExpo;
		_button.gameObject.SetActive(true);
		_rt.DOScaleX(1f, duration)
			.SetEase(ease)
			.SetAutoKill();
		_rt.DOScaleY(1, duration)
			.SetEase(ease)
			.SetDelay(delay)
			.SetAutoKill();
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		float duration = .25f;
		Ease ease = Ease.InBack;

		_rt.DOScale(_smallScale, duration)
			.SetEase(ease)
			.OnComplete(() => { _button.gameObject.SetActive(false); })
			.SetAutoKill();
	}

	public void SetClickAction(UnityAction clickAction)
	{
		_clickAction = clickAction;
	}
}

/**
<summary>
</summary>
*/
public class UIExplanationScreen : IUIScreen
{
	private UI _ui;
	private UIText _text;
	private UIButton _letsGoButton;
	private List<IUIElement> _elements;

	public bool IsShowing = false;
	public LevelData text
	{
		set
		{
			_text.text = value;
		}
	}

	public UIExplanationScreen(UI ui, UIText text, UIButton letsGoButton)
	{
		_ui = ui;
		_text = text;
		_letsGoButton = letsGoButton;
		_elements = new List<IUIElement>() { _text, _letsGoButton };
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		_elements.ForEach(el => el.Show());
		_ui.AddActiveScreen(this);
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		_elements.ForEach(el => el.Hide());
		_ui.RemoveActiveScreen(this);
	}
}

/**
<summary>
</summary>
*/
public class UIStartScreen : IUIScreen
{
	private UI _ui;
	private List<IUIElement> _elements;

	public bool IsShowing = false;

	public UIStartScreen(UI ui, IUIElement titleText, IUIElement buttonEasy, IUIElement buttonMedium, IUIElement buttonHard)
	{
		_ui = ui;
		_elements = new List<IUIElement>() { titleText, buttonEasy, buttonMedium, buttonHard };
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		_elements.ForEach(el => el.Show());
		_ui.AddActiveScreen(this);
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		_elements.ForEach(el => el.Hide());
		_ui.RemoveActiveScreen(this);
	}
}

/**
<summary>
When the game pauses the game this handles all the items of the pause-screen
</summary>
*/
public class UIPauseScreen : IUIScreen
{
	private UI _ui;
	private List<IUIElement> _elements;

	public UIPauseScreen(UI ui, UIText pauseText, UIButton continueButton, UIButton restartButton, UIButton goHomeButton)
	{
		_ui = ui;
		_elements = new List<IUIElement>() { pauseText, continueButton, restartButton, goHomeButton };
	}

	/**
	<summary>
	Shows a pausetext and the continue- restart- and gohome-button
	</summary>
	*/
	public void Show()
	{
		_elements.ForEach(el => el.Show());
		_ui.AddActiveScreen(this);
	}

	/**
	<summary>
	</summary>
	*/
	public void Hide()
	{
		_elements.ForEach(el => el.Hide());
		_ui.RemoveActiveScreen(this);
	}
}

/**
<summary>
</summary>
*/
public class UIScoresScreen : IUIScreen
{
	private UI _ui;
	private TextMeshProUGUI _scoreText;
	private TextMeshProUGUI _hiscoreText;
	private LifeCounterObjects _lifeCounterObjects;
	private RectTransform _scoreTextRT => _scoreText.transform as RectTransform;
	private RectTransform _hiscoreTextRT => _hiscoreText.transform as RectTransform;

	private float showDuration = .3f;
	private Ease showEase = Ease.OutExpo;
	private float hideDuration = .25f;
	private Ease hideEase = Ease.InExpo;
	private float hideYPos = 80f;
	private float showYPos = -82.2f;
	private float showYPosHI = -15f;

	public UIScoresScreen(UI ui, TextMeshProUGUI scoreText, TextMeshProUGUI hiscoreText, LifeCounterObjects lifeCounterObjects)
	{
		_ui = ui;
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
		ShowScore();
		_lifeCounterObjects.Show();

		_ui.AddActiveScreen(this);
	}

	private void ShowScore()
	{
		_scoreText.gameObject.SetActive(true);
		_scoreTextRT.DOAnchorPosY(showYPos, showDuration).SetEase(showEase);
	}

	private void ShowHiScore()
	{
		_hiscoreText.gameObject.SetActive(true);
		_hiscoreTextRT.DOAnchorPosY(showYPosHI, showDuration).SetEase(showEase);
	}

	public void ShowHiScoreOnly()
	{
		HideScore();
		ShowHiScore();
	}

	public void Hide()
	{
		HideScore();
		HideHiScore();
		_lifeCounterObjects.Hide();
		_ui.RemoveActiveScreen(this);
	}

	private void HideScore()
	{
		_scoreTextRT.DOAnchorPosY(80f, hideDuration).SetEase(hideEase).OnComplete(() => _scoreText.gameObject.SetActive(false));
	}

	private void HideHiScore()
	{
		_hiscoreTextRT.DOAnchorPosY(80f, hideDuration).SetEase(hideEase).OnComplete(() => _hiscoreText.gameObject.SetActive(false));
	}

	public void UpdateScore()
	{
		_scoreText.text = $"{ScoreManager.score.ToString()}";
		_hiscoreText.text = $"Hiscore: {ScoreManager.hiscore.ToString()}";
	}
}

/**
<summary>
</summary>
*/
public class UIGameOverScreen : IUIScreen
{
	private UI _ui;
	private List<IUIElement> _elements;

	public UIGameOverScreen(UI ui, UIText gameOverText, UIText yourscoreText, UIText hiscoreText, UIButton restartButton, UIButton goHomeButton)
	{
		_ui = ui;
		_elements = new List<IUIElement>() { gameOverText, yourscoreText, hiscoreText, restartButton, goHomeButton };
	}

	public void Show()
	{
		_elements.ForEach(el => el.Show());
		_ui.AddActiveScreen(this);
	}

	public void Hide()
	{
		_elements.ForEach(el => el.Hide());
		_ui.RemoveActiveScreen(this);
	}
}