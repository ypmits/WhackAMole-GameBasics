using System.Collections;
using UnityEngine;
using DG.Tweening;

/**
<summary>
This is the base-object
</summary>
*/
public class Mole : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private ParticleSystem _clickParticles;
	[SerializeField] private float _showYPos = 0f;
	[SerializeField] private float _hideYPos = 0f;
	[SerializeField] private MoleData _moleData;
	[SerializeField] private float speed = 1f;
	[SerializeField] private Light _light;
	[SerializeField, Range(.1f, 2f)] private float _maxLightIntensity = .7f;
	[SerializeField, ColorUsage(true, true)] private Color32 _fromColor;
#pragma warning restore 649

	public bool isShowing { get; private set; }
	private float _targetYPos = 0f;
	private Vector3 _originalPos;
	private Renderer _renderer;
	private Material _material;
	private Tweener _tweenCoreColor;
	private Tweener _tweenCoreLight;

	private void Start()
	{
		_originalPos = transform.position;
		_renderer = GetComponent<Renderer>();
		_material = _renderer.material;
		if (_light) _light.intensity = 0f;
		_targetYPos = _hideYPos;
		transform.position = new Vector3(_originalPos.x, _targetYPos, _originalPos.z);

		float duration = .4f;
		Ease ease = Ease.InOutCubic;
		float delay = .15f;
		_tweenCoreColor = _material.DOColor(Color.black, "_EmissionColor", duration).From(_fromColor, false).SetEase(ease).SetDelay(delay).Pause();
		_tweenCoreLight = _light.DOIntensity(0f, duration).From(_maxLightIntensity, false).SetEase(ease).SetDelay(delay).Pause();
	}

	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(_originalPos.x, _targetYPos, _originalPos.z), Time.deltaTime * speed);
		float diff = Mathf.Abs((isShowing ? _showYPos : _hideYPos) - transform.position.y);
		if (!isShowing && diff < .001f) _renderer.enabled = false;
	}

	private void OnMouseDown()
	{
		if (!isShowing || StateManager.state != GameState.InGameScreen) return;

		AudioManager.PlaySound(_moleData.GetWhackSound());
		// AudioManager.PlaySoundAtPosition(_moleData.GetWhackSound(), transform.position);
		StateManager.gameEvent.Invoke(GameEvent.Whack);

		if (_clickParticles) _clickParticles.Play();

		_tweenCoreColor.Play();
		_tweenCoreLight.Play();
		
		StartCoroutine(WaitAndHide(.2f));
	}

	internal void Show()
	{
		if (isShowing) return;
		_renderer.enabled = true;
		isShowing = true;
		_targetYPos = _showYPos;
	}

	private IEnumerator WaitAndHide(float delay)
	{
		yield return new WaitForSeconds(delay);
		Hide();
		yield return null;
	}

	internal void Hide()
	{
		if (!isShowing) return;
		isShowing = false;
		_targetYPos = _hideYPos;
	}
}