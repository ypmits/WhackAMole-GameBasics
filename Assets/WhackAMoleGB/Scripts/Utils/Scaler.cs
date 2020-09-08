using UnityEngine;

/**
<summary>
A simple 2D-movement-component that will update the position through a SineObject2D
</summary>
 */
[System.Serializable]
public class Scaler : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private SineObject2D _scaleX;
	[SerializeField] private SineObject2D _scaleY;
#pragma warning restore 649

	private Vector3 _originalScale;

	private void Awake() {
		_originalScale = transform.localScale;
	}

	/**
	<summary>
	Initializes the object
	</summary>
	*/
	public Scaler Init(float minX = .04f, float maxX = .09f, float minY = .3f, float maxY = .9f)
	{
		_scaleX = new SineObject2D(Random.Range(minX, maxX), Random.Range(minY, maxY));
		_scaleY = new SineObject2D(Random.Range(minX, maxX), Random.Range(minY, maxY), Axis.Vertical);
		return this;
	}

	void Update()
	{
		Vector3 s = transform.localScale;
		if (_scaleX != null) s.x = _originalScale.x + _scaleX.Calculate(transform).x;
		if (_scaleY != null) s.y = _originalScale.y + _scaleY.Calculate(transform).y;
		transform.localScale = s;
	}

	/**
	<summary>
	Set a custom SineObject at runtime
	</summary>
	*/
	public void SetSine(SineObject2D scaleX = null, SineObject2D scaleY = null)
	{
		_scaleX = scaleX;
		_scaleY = scaleY;
	}
}