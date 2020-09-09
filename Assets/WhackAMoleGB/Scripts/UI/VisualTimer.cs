using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class VisualTimer : MonoBehaviour {
#pragma warning disable 649
	[SerializeField] private Image _barGraphic;
#pragma warning restore 649

	public bool isPaused = true;
	public UnityAction completeAction;

	private float _timeLeft = 10f;
	private float _totalTimeInSeconds = 10f;
	private RectTransform rt => transform as RectTransform;


	private void Start() {
		Reset();
		gameObject.SetActive(false);
	}

	private void Update() {
		if(isPaused) return;
		_timeLeft -= Time.deltaTime;
		float fillAmount = _timeLeft * (1f / _totalTimeInSeconds);
		if(fillAmount <= 0)
		{
			isPaused = true;
			if(completeAction != null) completeAction.Invoke();
		}
		_barGraphic.fillAmount = fillAmount;
	}

	public void AddTime(float timeToAdd) => _timeLeft += timeToAdd;
	public void Setup(float totalTimeInSeconds, bool autoShow = false, bool autoStart = false)
	{
		_totalTimeInSeconds = _timeLeft = totalTimeInSeconds;
		if(autoShow) Show(autoStart);
	}
	public void StartTimer()
	{
		if(!isPaused) return;
		isPaused = false;
	}
	/**
	Pauses the timer.
	The bar will stay the where it ended
	*/
	public void Pause()
	{
		if(isPaused) return;
		isPaused = true;
	}
	/**
	Stops the timer
	The bar will be reset (filled to the fullest)
	*/
	public void Stop()
	{
		Reset();
	}
	public void Reset()
	{
		isPaused = true;
		_timeLeft = _totalTimeInSeconds;
		_barGraphic.fillAmount = 1f;
	}

	public void Show(bool autoStart = false)
	{
		gameObject.SetActive(true);
		rt.DOAnchorPosY(40f, .3f).SetEase(Ease.OutExpo);
		if(autoStart) StartTimer();
	}
	public void Hide()
	{
		rt.DOAnchorPosY(0f, .25f).SetEase(Ease.InExpo).OnComplete(()=>{
			gameObject.SetActive(false);
		});

	}
}