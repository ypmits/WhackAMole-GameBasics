using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;


/**
<summary>
Can count down and then call a function.
You can add seperate sounds for the counting and the finish.
You can set the delay
</summary>
*/
public class CountDown : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private AudioClip _beepCountDown;
	[SerializeField] private AudioClip _beepFinished;
	[SerializeField] private float _delay = .5f;
	[SerializeField] List<Image> _images;
#pragma warning restore 649
	public bool isCountingDown { get; private set; }

	private UnityAction _action;
	private int _current = 3;
	private int _currentImageIndex = 0;
	private List<Transform> _currentTransforms;


	private void Start()
	{
		Color baseColor = Color.white;
		baseColor.a = 0;
		_images.ForEach(img=>{
			img.color = baseColor;
			img.transform.localScale = new Vector3(.1f, .1f, .1f);
			img.gameObject.SetActive(false);
			img.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-20f, 20f)));
		});
		_current = _images.Count-1;
	}

	/**
	<summary>
	</summary>
	*/
	public void StartCountDown(UnityAction action = null, float countDelay = .5f)
	{
		_currentImageIndex = 0;
		_current = 3;
		_action = action;
		_delay = countDelay;
		isCountingDown = true;

		if(_currentTransforms != null && _currentTransforms.Count > 0) _currentTransforms.ForEach(transform=>transform.DOKill());
		StartCoroutine(Count());
	}

	private IEnumerator Count()
	{
		_currentTransforms = Fade(_images[_currentImageIndex], _delay, _currentImageIndex == 0 ? null : _images[_currentImageIndex-1]);
		_currentImageIndex++;
		AudioManager.PlaySound(_current == 0 ? _beepFinished : _beepCountDown);
		if (_current == 0)
		{
			_current = 3;
			isCountingDown = false;
			yield return new WaitForSeconds(_delay);
			_currentTransforms = Fade(null, _delay, _images[_currentImageIndex-1]);
			_action?.Invoke();
		}
		else
		{
			_current--;
			yield return new WaitForSeconds(_delay);
			StartCoroutine(Count());
		}
		yield return null;
	}

	/**
	<summary>
	Fades in the 'fadeInImage' (scale to 1f, rotation to 0f and opacity to 1f)
	Fades out the 'fadeOutImage' (scale to 0f, rotation to 0f and opacity to 0f)
	</summary>
	*/
	private List<Transform> Fade(Image fadeInImage, float delay, Image fadeOutImage = null)
	{
		List<Transform> transforms = new List<Transform>();
		if (fadeInImage != null)
		{
			fadeInImage.gameObject.SetActive(true);
			fadeInImage.transform.DOScale(1f, delay * .6f).SetEase(Ease.OutBack);
			fadeInImage.DOFade(1f, delay * .4f).SetEase(Ease.InOutCubic);
			fadeInImage.transform.DORotate(new Vector3(0f, 0f, 0f), delay * .6f).SetEase(Ease.OutBack);

			transforms.Add(fadeInImage.transform);
		}
		if (fadeOutImage != null)
		{
			fadeOutImage.DOFade(0f, delay * .15f).SetEase(Ease.InOutCubic).OnComplete(() => { fadeOutImage.gameObject.SetActive(false); });
			fadeOutImage.transform.DOScale(.1f, delay * .15f).SetEase(Ease.InOutCubic);
			fadeOutImage.transform.DORotate(new Vector3(0f, 0f, 0f), delay * .6f).SetEase(Ease.InOutCubic);
			transforms.Add(fadeOutImage.transform);
		}
		return transforms;
	}
}