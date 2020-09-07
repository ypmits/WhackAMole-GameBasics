using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class UI : MonoBehaviour
{
#pragma warning disable 649
	[Header("General-assetGroup")]
	[SerializeField] private Image _backgroundImage;
	[SerializeField] private TextMeshProUGUI _explanationText;
	[SerializeField] private Button _buttonLetsGo;
	[SerializeField] private Button _buttonPause;
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
	public UIExplanation explanation { get; private set; }
	public UIImage background { get; private set; }
	public UIButton pauseButton { get; private set; }
	public UIStartScreen startScreen { get; private set; }
	public UIPauseScreen pauseScreen { get; private set; }
	public UIScoresScreen scoresScreen { get; private set; }
	public UIGameOverScreen gameOver { get; private set; }
	public LifeCounterObjects lifeCounterObjects => _lifeCounterObjects;
	public CountDown countdown => _countdown;
	public VisualTimer timer => _timer;

	private IEnumerator _coroutine;


	private void Awake()
	{
		// Reusable components:
		UIButton buttonRestart = new UIButton(_restartButton, () => { StateManager.gameEvent.Invoke(GameEvent.RestartGame); });
		UIButton buttonEasy = new UIButton(_buttonEasy, () => { Model.levelData = GameController.refs.prefabs.levelEasy; StateManager.gameEvent.Invoke(GameEvent.ShowExplanation); });
		UIButton buttonMedium = new UIButton(_buttonMedium, () => { Model.levelData = GameController.refs.prefabs.levelMedium; StateManager.gameEvent.Invoke(GameEvent.ShowExplanation); });
		UIButton buttonHard = new UIButton(_buttonHard, () => { Model.levelData = GameController.refs.prefabs.levelHard; StateManager.gameEvent.Invoke(GameEvent.ShowExplanation); });
		UIButton buttonLetsGo = new UIButton(_buttonLetsGo, () => { StateManager.gameEvent.Invoke(GameEvent.StartCountDown); });
		UIButton buttonGoHome = new UIButton(_goHomeButton, () => { StateManager.gameEvent.Invoke(GameEvent.GoHome); });
		UIButton buttonContinue = new UIButton(_continueButton, () => { StateManager.gameEvent.Invoke(GameEvent.Continue); });
		pauseButton = new UIButton(_buttonPause, () => { StateManager.gameEvent.Invoke(GameEvent.Pause); });

		background = new UIImage(_backgroundImage);
		explanation = new UIExplanation(new UIText(_explanationText), buttonLetsGo);
		startScreen = new UIStartScreen(new UIText(_titleText), buttonEasy, buttonMedium, buttonHard);
		pauseScreen = new UIPauseScreen(new UIText(_pauseText), buttonContinue, buttonRestart, buttonGoHome);
		scoresScreen = new UIScoresScreen(_scoreText, _hiscoreText, _lifeCounterObjects);
		gameOver = new UIGameOverScreen(new UIText(_gameOverText), buttonRestart, buttonGoHome);

		timer.completeAction = () => StateManager.gameEvent.Invoke(GameEvent.GameOver);
	}

	private void Start()
	{
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.GoHome:
					pauseButton.Hide();
					scoresScreen.Hide();
					startScreen.Show();
					break;

				case GameEvent.ShowExplanation:
					if (_coroutine != null) StopCoroutine(_coroutine);
					_coroutine = ShowExplanation(.5f);
					StartCoroutine(_coroutine);
					break;

				case GameEvent.StartCountDown:
					if (_coroutine != null) StopCoroutine(_coroutine);
					_coroutine = ShowCountdown(.5f);
					timer.Reset();
					StartCoroutine(_coroutine);
					break;

				case GameEvent.StartGame:
					pauseButton.Show();
					scoresScreen.Show();
					if (Model.levelData.timeBased) timer.Setup(20f, true, true);
					break;

				case GameEvent.GameOver:
					if (_coroutine != null) StopCoroutine(_coroutine);
					gameOver.Show();
					pauseButton.Hide();
					_lifeCounterObjects.Hide();
					if (Model.levelData.timeBased) timer.Pause();
					break;

				case GameEvent.Pause:
					pauseScreen.Show();
					if (Model.levelData.timeBased) timer.Pause();
					break;

				case GameEvent.Continue:
					pauseScreen.Hide();
					if (Model.levelData.timeBased) timer.StartTimer();
					break;
			}
		});
	}

	private IEnumerator ShowExplanation(float delay)
	{
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
// TODO: Implement the UITweenType
public enum UITweenType { Fade, Move, Scale }
public interface IUIElement { void Show(); void Hide(); }

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
			value.explanation = value.explanation.Replace("{{startSpeed}}", value.startSpeed.ToString());
			value.explanation = value.explanation.Replace("{{totalTime}}", value.totalTime.ToString());
			_text.text = value.explanation;
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
public class UIExplanation
{
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

	public UIExplanation(UIText text, UIButton letsGoButton)
	{
		_text = text;
		_letsGoButton = letsGoButton;
		_elements = new List<IUIElement>() { GameController.refs.ui.background, _text, _letsGoButton };
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		_elements.ForEach(el => el.Show());
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		_elements.ForEach(el => el.Hide());
	}
}

/**
<summary>
</summary>
*/
public class UIStartScreen
{
	private UIText _titleText;
	private List<IUIElement> _elements;

	public bool IsShowing = false;

	public UIStartScreen(UIText titleText, UIButton buttonEasy, UIButton buttonMedium, UIButton buttonHard)
	{
		_titleText = titleText;
		_elements = new List<IUIElement>() { GameController.refs.ui.background, titleText, buttonEasy, buttonMedium, buttonHard };
	}

	public void Show()
	{
		if (IsShowing) return;
		IsShowing = true;

		_elements.ForEach(el => el.Show());
	}

	public void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;

		_elements.ForEach(el => el.Hide());
	}
}

/**
<summary>
When the game pauses the game this handles all the items of the pause-screen
</summary>
*/
public class UIPauseScreen
{
	private List<IUIElement> _elements;

	public UIPauseScreen(UIText pauseText, UIButton continueButton, UIButton restartButton, UIButton goHomeButton)
	{
		_elements = new List<IUIElement>() { GameController.refs.ui.background, pauseText, continueButton, restartButton, goHomeButton };
	}

	/**
	<summary>
	Shows a pausetext and the continue- restart- and gohome-button
	</summary>
	*/
	public void Show()
	{
		_elements.ForEach(el => el.Show());
	}

	/**
	<summary>
	</summary>
	*/
	public void Hide()
	{
		_elements.ForEach(el => el.Hide());
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
	private List<IUIElement> _elements;

	public UIGameOverScreen(UIText gameOverText, UIButton restartButton, UIButton goHomeButton)
	{
		_elements = new List<IUIElement>() { GameController.refs.ui.background, gameOverText, restartButton, goHomeButton };
	}

	public void Show()
	{
		_elements.ForEach(el => el.Show());
	}

	public void Hide()
	{
		_elements.ForEach(el => el.Hide());
	}
}