using System;
using System.Collections;
using UnityEngine;

/**
<summary>
Could be seen as the document-class. Taking the center of the complete app.
</summary>
*/
public class GameController : MonoBehaviour
{
#pragma warning disable 649
	[SerializeField] private UI _ui;
	[SerializeField] private Engine _gameEngine;
	[SerializeField] private Prefabs _prefabs;
#pragma warning restore 649
	public static References refs;

	/**
	<summary>
	</summary>
	*/
	private void Awake()
	{
		// Setup the references
		GameController.refs = new References(this, _ui, _gameEngine, _prefabs);

		// Reset the scores
		ScoreManager.Reset();

		// Initialize the AudioManager
		// Important! note that the 'sound' is set to 'true' since this is a simple app where we're not using any preferences
		AudioManager.sound = AudioManager.music = true;
		AudioManager.AttachAudioSources(gameObject);

		// Setup the GameEvent-listener:
		// From here all the states are set:
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.GoHome:
					StateManager.state =GameState.HomeScreen;
					break;

				case GameEvent.StartGame:
					StateManager.isAlive = true;
					StateManager.state = GameState.InGameScreen;
					ScoreManager.Reset();
					break;

				case GameEvent.RestartGame:
					StateManager.state = GameState.InGameScreen;
					ScoreManager.Reset();
					break;
				
				case GameEvent.GameOver:
					StateManager.isAlive = false;
					StateManager.state = GameState.GameOverScreen;
					break;

			}
		});
	}

	IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		StateManager.gameEvent.Invoke(GameEvent.GoHome);
		yield return null;
	}
}
