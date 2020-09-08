using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Engine : MonoBehaviour
{
	private float _spawnSpeed = 1.5f; // How long it takes for a new 'mole' to popup
	private float _spawnDecrement = 0.1f; // The time that's taken from the spawnDuration with every mole popping up
	public float gameTimer = 15f; // Total time of game
	private float spawnTimer = 0f;

	private List<Mole> _moles;


	private void Start()
	{
		ReplacePlaceholderMoles();
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.StartCountDown:
					gameTimer = Model.levelData.totalTime;
					Reset();
					break;

				case GameEvent.StartGame:
				case GameEvent.RestartGame:
					// First set the _spawnSpeed (how long it takes for the moles to spawn) to the level's startSpeed
					_spawnSpeed = Model.levelData.spawnSpeed;
					_spawnDecrement = Model.levelData.moleSpawnDecrement;
					StateManager.isPaused = false;
					break;

				case GameEvent.Pause:
				case GameEvent.GameOver:
					StateManager.isPaused = true;
					break;

				case GameEvent.Continue:
					StateManager.isPaused = false;
					break;
				
			}
		});
	}

	private void Update()
	{
		if (StateManager.isPaused) return;
		if (Model.levelData.timeBased) gameTimer -= Time.deltaTime;
		if (gameTimer <= 0f) return;

		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0f)
		{
			// Get a random active mole:
			List<Mole> activeMoles = _moles.Where(mole => mole.IsVisible == false).ToList();
			activeMoles[Random.Range(0, activeMoles.Count)].Show();

			_spawnSpeed -= _spawnDecrement;
			if (_spawnSpeed < Model.levelData.minimumSpawnDecrement) _spawnSpeed = Model.levelData.minimumSpawnDecrement;
			spawnTimer = Model.levelData.spawnSpeed;
		}
	}

	private void ReplacePlaceholderMoles()
	{
		if (_moles == null) _moles = new List<Mole>();
		GameObject[] _placeholders = GameObject.FindGameObjectsWithTag("MolePlaceholder");
		for (int i = 0; i < _placeholders.Length; i++)
		{
			List<GameObject> moles = GameController.refs.prefabs.levelEasy.moles;

			int index = Random.Range(0, moles.Count);
			_moles.Add(Instantiate<GameObject>(moles[index], _placeholders[i].transform.position, Quaternion.identity, _placeholders[i].transform.parent).GetComponent<Mole>());
			Destroy(_placeholders[i].gameObject);
		}
	}

	private void Reset()
	{
		_moles.Where(mole => mole.IsVisible == false).ToList().ForEach(m=>m.HideImmediately());
	}
}