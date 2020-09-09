using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Text;
using UnityEngine.Events;

public interface IUIElement { void Show(); void Hide(); }


public class AUIElement : IUIElement
{
	public bool IsShowing = false;

	public virtual void Show()
	{
		if (IsShowing) return;
		IsShowing = true;
	}

	public virtual void Hide()
	{
		if (!IsShowing) return;
		IsShowing = false;
	}
}


/**
<summary>
General purpose Image which has all tweening-functionality inside
</summary>
*/
public class UIImage : AUIElement
{
	private Image _img;

	public UIImage(Image img)
	{
		_img = img;
		_img.color = new Color(_img.color.r, _img.color.g, _img.color.b, 0f);
		_img.gameObject.SetActive(false);
	}

	override public void Show()
	{
		base.Show();

		_img.gameObject.SetActive(true);
		_img.DOFade(.8f, .3f)
			.SetEase(Ease.OutExpo)
			.SetAutoKill();
	}

	override public void Hide()
	{
		base.Hide();

		_img.DOFade(0f, .25f)
			.SetEase(Ease.InExpo)
			.SetAutoKill()
			.OnComplete(() => _img.gameObject.SetActive(false));
	}
}

/**
<summary>
General purpose Image which has all tweening-functionality inside
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
		_rt.DOScale(0f, .25f).SetEase(Ease.OutExpo).OnComplete(() =>
		{
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
	public LevelData levelData
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
public class UIButton : AUIElement
{
	private Button _button;
	private UnityAction _clickAction;
	private RectTransform _rt => _button.transform as RectTransform;
	private float _smallScale = .2f;
	private bool _canFade = false;
	private bool _canScale = false;
	private CanvasGroup _canvasGroup;

	public bool CanFade
	{
		get { return _canFade; }
		set
		{
			_canFade = value;
			if (_canFade) AddFadingCapability(); else RemoveFadingCapability();
		}
	}

	public bool CanScale
	{
		get { return _canScale; }
		set { _canScale = value; }
	}

	public UIButton(Button button, UnityAction clickAction = null, bool canFade = true, bool canScale = false)
	{
		_button = button;
		_clickAction = clickAction;
		_canFade = canFade;
		_canScale = canScale;

		if (_canScale) _rt.localScale = new Vector3(_smallScale, _smallScale, 1f);
		if (_canFade) AddFadingCapability();
		_button.onClick.AddListener(() =>
		{
			if (!IsShowing || clickAction == null) return;
			clickAction.Invoke();
		});
		_button.gameObject.SetActive(false);
	}

	override public void Show()
	{
		base.Show();

		float duration = .3f;
		float delay = .1f;
		Ease ease = Ease.OutExpo;
		_button.gameObject.SetActive(true);
		if (_canScale)
			_rt.DOScaleX(1f, duration)
				.SetEase(ease)
				.SetAutoKill();
		_rt.DOScaleY(1, duration)
			.SetEase(ease)
			.SetDelay(delay)
			.SetAutoKill();
		if (_canFade)
			_canvasGroup.DOFade(1f, duration)
				.SetEase(ease)
				.SetDelay(delay)
				.SetAutoKill();
	}

	override public void Hide()
	{
		base.Hide();

		float duration = .25f;
		Ease ease = Ease.InBack;

		bool hasComplete = false;
		if (_canScale)
		{
			Tweener t = _rt.DOScale(_smallScale, duration)
				.SetEase(ease)
				.SetAutoKill();
			if (!hasComplete)
			{
				hasComplete = true;
				t.OnComplete(() => { _button.gameObject.SetActive(false); });
			}
		}
		if (_canFade)
		{
			Tweener t = _canvasGroup.DOFade(0f, duration)
				.SetEase(ease)
				.SetAutoKill();
			if (!hasComplete)
			{
				hasComplete = true;
				t.OnComplete(() => { _button.gameObject.SetActive(false); });
			}
		}
	}

	public void SetClickAction(UnityAction clickAction)
	{
		_clickAction = clickAction;
	}

	private void AddFadingCapability()
	{
		if (!_button) return;
		if (!_button.gameObject.GetComponent<CanvasGroup>() && !_canvasGroup)
		{
			_canvasGroup = _button.gameObject.AddComponent<CanvasGroup>();
			_canvasGroup.alpha = 0f;
		}
	}

	private void RemoveFadingCapability()
	{
		if (!_button) return;
		if (_canvasGroup || _button.GetComponent<CanvasGroup>() == null)
		{
			MonoBehaviour.Destroy(_button.gameObject.GetComponent<CanvasGroup>());
			_canvasGroup = null;
		}
	}
}