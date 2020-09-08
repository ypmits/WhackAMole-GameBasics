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

	public bool IsVisible { get; private set; }
	
	private float _aliveTime = 10f;
	private float _targetYPos = 0f;
	private Vector3 _originalPos;
	private Renderer _renderer;
	private Material _material;
	private Tweener _tweenCoreColor;
	private Tweener _tweenCoreLight;
	private Tween _delayedTween;

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
		if (StateManager.isPaused) return;
		transform.position = Vector3.Lerp(transform.position, new Vector3(_originalPos.x, _targetYPos, _originalPos.z), Time.deltaTime * (IsVisible ? speed * 3f : speed));
		float diff = Mathf.Abs((IsVisible ? _showYPos : _hideYPos) - transform.position.y);
		if (!IsVisible && diff < .001f) _renderer.enabled = false;

		_aliveTime -= Time.deltaTime;
		if (_aliveTime <= 0f)
		{
			if(_delayedTween != null) _delayedTween.Kill();
			_delayedTween = DOVirtual.DelayedCall(.2f, Hide);
			_aliveTime = _moleData.aliveTime;
		}
	}

	private void OnMouseDown()
	{
		if (!IsVisible || StateManager.state != GameState.InGameScreen) return;

		AudioManager.PlaySound(_moleData.GetWhackSound());
		StateManager.gameEvent.Invoke(GameEvent.Whack);

		if (_clickParticles) _clickParticles.Play();

		_tweenCoreColor.Play();
		_tweenCoreLight.Play();

		if(_delayedTween != null) _delayedTween.Kill();
		_delayedTween = DOVirtual.DelayedCall(.2f, Hide);
	}

	/**
	<summary>
	</summary>
	*/
	public void Show()
	{
		if (IsVisible) return;
		_renderer.enabled = true;
		IsVisible = true;
		_targetYPos = _showYPos;
		_aliveTime = _moleData.aliveTime;
	}

	public void HideImmediately()
	{
		Hide();
		transform.position =new Vector3(_originalPos.x, _hideYPos, _originalPos.z);
	}

	/**
	<summary>
	</summary>
	*/
	public void Hide()
	{
		if (!IsVisible) return;
		IsVisible = false;
		_targetYPos = _hideYPos;
	}
}