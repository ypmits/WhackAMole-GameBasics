using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Engine : MonoBehaviour
{
	private GameObject[] _placeholders;
	private List<Mole> _moles;
	private float _startSpeed;

	private bool isPaused = true;
	private IEnumerator _coroutine;


	private void Awake()
	{
		_coroutine = PickAMole();
	}

	private void Start()
	{
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.StartCountDown:
					InitMoles();
					break;

				case GameEvent.StartGame:
					isPaused = false;
					_startSpeed = Model.levelData.startSpeed;
					PickAMole();
					if (_coroutine != null) StopCoroutine(_coroutine);
					StartCoroutine(_coroutine);
					break;
				
				case GameEvent.Whack:
					if (_coroutine != null) StopCoroutine(_coroutine);
					StartCoroutine(_coroutine);
					break;

				case GameEvent.GameOver:
					isPaused = true;
					break;
			}
		});
	}

	private IEnumerator PickAMole()
	{
		Debug.Log("Choose a mole to 'Show'");
		// 1. Get a list of the moles that are not 'visible':
		List<Mole> activeMoles = _moles.Where(mole => mole.IsVisible == false).ToList();

		// 2. Take a random mole from that list:
		Mole m = activeMoles[Random.Range(0, activeMoles.Count)];

		// 3. Show it
		m.Show();

		_startSpeed = Mathf.Clamp(_startSpeed - .15f, .25f, float.MaxValue);
		Debug.Log(_startSpeed);
		yield return new WaitForSeconds(_startSpeed);
		StartCoroutine(_coroutine);
		yield return null;
	}

	private void InitMoles()
	{
		if (_moles == null) _moles = new List<Mole>();
		_placeholders = GameObject.FindGameObjectsWithTag("MolePlaceholder");
		for (int i = 0; i < _placeholders.Length; i++)
		{
			List<GameObject> moles = GameController.refs.prefabs.levelEasy.moles;

			int index = Random.Range(0, moles.Count);
			_moles.Add(Instantiate<GameObject>(moles[index], _placeholders[i].transform.position, Quaternion.identity, _placeholders[i].transform.parent).GetComponent<Mole>());
			Destroy(_placeholders[i].gameObject);
		}
	}
}