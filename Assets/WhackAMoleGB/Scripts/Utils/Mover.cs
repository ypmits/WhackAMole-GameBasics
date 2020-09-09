using UnityEngine;

/**
<summary>
A simple 2D-movement-component that will update the position through a SineObject
</summary>
 */
[System.Serializable]
public class Mover : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private SineObject2D _x;
	[SerializeField] private SineObject2D _y;
#pragma warning restore 649

	/**
	<summary>
	Initializes the object
	</summary>
	*/
	public Mover Init(float minX = .04f, float maxX = .09f, float minY = .3f, float maxY = .9f)
	{
		_x = new SineObject2D(Random.Range(minX, maxX), Random.Range(minY, maxY));
		_y = new SineObject2D(Random.Range(minX, maxX), Random.Range(minY, maxY), Axis.Vertical);
		return this;
	}

	void Update()
	{
		if (_x != null) transform.position += _x.Calculate(transform);
		if (_y != null) transform.position += _y.Calculate(transform);
	}

	/**
	<summary>
	Set a custom SineObject at runtime
	</summary>
	*/
	public void SetSine(SineObject2D x = null, SineObject2D y = null)
	{
		_x = x;
		_y = y;
	}
}