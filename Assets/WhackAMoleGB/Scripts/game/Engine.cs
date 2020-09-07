using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Engine : MonoBehaviour
{
	private GameObject[] _placeholders;
	private List<Mole> _moles;
	private float _startSpeed;

	private bool isPaused = true;


	private void Start()
	{

		// Setup the GameEvent-listener:
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.StartCountDown:
					InitGameObjects();
					break;

				case GameEvent.StartGame:
					isPaused = false;
					_startSpeed = Model.levelData.startSpeed;
					Floep();
					Invoke("Floep", _startSpeed);
					break;

				case GameEvent.GameOver:
					isPaused = true;
					break;
			}
		});
	}

	private void Floep()
	{
		Debug.Log("Choose a mole to 'Show'");
		// 1. Get a list of the moles that are not 'showing':
		List<Mole> activeMoles = _moles.Where(mole => mole.isShowing == false).ToList();

		// 2. Take a random mole from that list: https://dejanstojanovic.net/aspnet/2015/february/random-element-of-ienumerablearray/
		Mole m = activeMoles[ Random.Range(0, activeMoles.Count) ];
		// Mole m = activeMoles[Random.Range(0, activeMoles.ToArray().Length)];

		// 3. Show it
		m.Show();

		_startSpeed -= .15f;
		Invoke("Floep", _startSpeed);
	}

	private void Update()
	{
		if (isPaused) return;

	}

	internal void Play() { }
	internal void Pause() { }
	internal void Stop() { }
	internal void Reset()
	{
	}
	internal void LevelUp()
	{
	}

	private void InitGameObjects()
	{
		if (_moles == null) _moles = new List<Mole>();
		_placeholders = GameObject.FindGameObjectsWithTag("MolePlaceholder");
		for (int i = 0; i < _placeholders.Length; i++)
		{
			List<GameObject> moles = GameController.refs.prefabs.levelEasy.moles;

			int index = Random.Range(0, moles.Count);
			Debug.Log("index: " + index);

			_moles.Add(Instantiate<GameObject>(moles[index], _placeholders[i].transform.position, Quaternion.identity, _placeholders[i].transform.parent).GetComponent<Mole>());
			Destroy(_placeholders[i].gameObject);
		}
	}
}