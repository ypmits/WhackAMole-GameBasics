using System;
using System.Collections;
using UnityEngine;

/**
<summary>
The document-class
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
	private LevelData _levelData;

	/**
	<summary>
	</summary>
	*/
	private void Awake()
	{
		// Setup the references:
		GameController.refs = new References(this, _ui, _gameEngine, _prefabs);

		// Initialize the AudioManager. Important! set the 'sound' to 'true' since we're not using any preferences in this app:
		AudioManager.sound = true;
		AudioManager.AttachAudioSources(gameObject);

		// Setup the GameEvent-listener:
		StateManager.gameEvent.AddListener((GameEvent e) =>
		{
			switch (e)
			{
				case GameEvent.GoHome:
					StateManager.SetState(GameState.HomeScreen);
					break;

				case GameEvent.StartGame:
					StateManager.SetState( GameState.InGameScreen );
					ScoreManager.Reset();
					break;

				case GameEvent.RestartGame:
					StateManager.SetState( GameState.InGameScreen );
					ScoreManager.Reset();
					break;
				
				case GameEvent.Whack:
					Debug.Log("Whack!");
					ScoreManager.AddScore(1);
					_ui.scoresScreen.UpdateScore();
					break;
				
				case GameEvent.GameOver:
					StateManager.SetState( GameState.GameOverScreen );
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
