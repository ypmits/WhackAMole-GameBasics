using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


/**
<summary>
</summary>
*/
public class LifeCounter : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private Image _image;
	[SerializeField] private Image _errorImage;
	[SerializeField] private float _showYPos = 30f;
	[SerializeField] private float _hideYPos = -250f;
#pragma warning restore 649
	private bool isShowing;
	private RectTransform rt => transform as RectTransform;

	public bool isActive = false;

	void Start()
	{
		Reset();
	}

	/**
	<summary>
	</summary>
	*/
	public void Show(int n)
	{
		if(isShowing) return;
		isShowing = true;
		
		gameObject.SetActive(true);

		// DOTween-animations:
		_image.DOFade(1f, .35f).SetEase(Ease.OutExpo).SetDelay(n*.1f).SetAutoKill();
		rt.DOAnchorPosY(_showYPos, .35f).SetEase(Ease.OutExpo).SetDelay(n*.1f).SetAutoKill();
	}
	
	/**
	<summary>
	</summary>
	*/
	public void Hide()
	{
		if(!isShowing) return;
		isShowing = false;

		// DOTween-animations:
		_image.DOFade(1f, .15f).SetEase(Ease.InExpo).SetAutoKill();
		rt.DOAnchorPosY(_hideYPos, .35f).SetEase(Ease.InExpo).SetAutoKill();
	}

	public void Activate()
	{
		isActive = true;
		Debug.Log("LiefCounter Activate");

		// DOTween-animations:
		_errorImage.gameObject.SetActive(true);
		_errorImage.DOFade(1f, .35f).SetEase(Ease.OutExpo).SetAutoKill();
	}

	public void Reset()
	{
		isShowing = isActive = false;
		_image.color = new Color(1f,1f,1f,0f);
		rt.anchoredPosition = new Vector3(rt.anchoredPosition.x, _hideYPos);
		_errorImage.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
}