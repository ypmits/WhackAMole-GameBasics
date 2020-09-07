using UnityEngine;

public enum Axis { Vertical, Horizontal }

[System.Serializable]
public class SineObject2D
{
	[SerializeField] private Axis _axis = Axis.Horizontal;
	[SerializeField] private float _amplitude;
	[SerializeField] private float _frequency;

	public Axis axis { set { _axis = value; } }

	public SineObject2D(float amplitude, float frequency, Axis axis = Axis.Horizontal)
	{
		_amplitude = amplitude;
		_frequency = frequency;
		_axis = axis;
	}

	public Vector3 Calculate(Transform transform)
	{
		return _amplitude * (Mathf.Sin(2 * Mathf.PI * _frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * _frequency * (Time.time - Time.deltaTime))) * ((_axis == Axis.Horizontal) ? transform.up : transform.right);
	}
}