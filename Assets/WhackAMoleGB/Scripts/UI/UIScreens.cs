using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


#region Interfaces and Abstracts
public interface IUIScreen { void Show(); void Hide(); }


public class AUIScreen : IUIScreen
{
	public bool IsShowing = false;
	protected List<IUIElement> pElements;

	public virtual void Show()
	{
		if (IsShowing) return;
		IsShowing = true;
		if (pElements != null && pElements.Count > 0) pElements.ForEach(el => el.Show());
	}

	public virtual void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;
		if (pElements != null && pElements.Count > 0) pElements.ForEach(el => el.Hide());
	}
}
#endregion


#region Screens
/**
<summary>
The screen you see when you've selected a difficulty.
It shows you a small explanation of the difficulty-level and an actionbutton.
</summary>
*/
public class UIExplanationScreen : AUIScreen
{
	private UIText _text;
	private UIButton _letsGoButton;

	public LevelData levelData { set { _text.levelData = value; } }

	public UIExplanationScreen(UIText text, UIButton letsGoButton)
	{
		_text = text;
		_letsGoButton = letsGoButton;
		pElements = new List<IUIElement>() { _text, _letsGoButton };
	}
}


/**
<summary>
The screen at the start of the game with the title and the difficulty-buttons
</summary>
*/
public class UIStartScreen : AUIScreen
{
	public UIStartScreen(IUIElement titleText, IUIElement buttonEasy, IUIElement buttonMedium, IUIElement buttonHard)
	{
		pElements = new List<IUIElement>() { titleText, buttonEasy, buttonMedium, buttonHard };
	}
}


/**
<summary>
When the game pauses the game this handles all the items of the pause-screen
</summary>
*/
public class UIPauseScreen : AUIScreen
{
	public UIPauseScreen(UIText pauseText, UIButton continueButton, UIButton restartButton, UIButton goHomeButton)
	{
		pElements = new List<IUIElement>() { pauseText, continueButton, restartButton, goHomeButton };
	}
}


/**
<summary>
The topbar where the score and hiscores are shown
</summary>
*/
public class UIScoresScreen : AUIScreen
{
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

#region Publics
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

	override public void Show()
	{
		base.Show();
		ShowScore();
		_lifeCounterObjects.Show();
	}

	public void ShowHiScoreOnly()
	{
		HideScore();
		ShowHiScore();
	}

	override public void Hide()
	{
		base.Hide();
		HideScore();
		HideHiScore();
		_lifeCounterObjects.Hide();
	}

	public void UpdateScore()
	{
		_scoreText.text = $"{ScoreManager.score.ToString()}";
		_hiscoreText.text = $"Hiscore: {ScoreManager.hiscore.ToString()}";
	}
#endregion

#region Privates
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

	private void HideScore()
	{
		_scoreTextRT.DOAnchorPosY(80f, hideDuration).SetEase(hideEase).OnComplete(() => _scoreText.gameObject.SetActive(false));
	}

	private void HideHiScore()
	{
		_hiscoreTextRT.DOAnchorPosY(80f, hideDuration).SetEase(hideEase).OnComplete(() => _hiscoreText.gameObject.SetActive(false));
	}
#endregion
}


/**
<summary>
</summary>
*/
public class UIGameOverScreen : AUIScreen
{
	public UIGameOverScreen(UIText gameOverText, UIText yourscoreText, UIText hiscoreText, UIButton restartButton, UIButton goHomeButton)
	{
		pElements = new List<IUIElement>() { gameOverText, yourscoreText, hiscoreText, restartButton, goHomeButton };
	}
}
#endregion