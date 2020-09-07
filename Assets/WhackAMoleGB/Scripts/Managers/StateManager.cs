using UnityEngine.Events;

public enum GameEvent { GoHome, ChooseLevel, StartCountDown , StartGame, RestartGame, GameOver, Whack }
public enum GameState { None, HomeScreen, LevelChoiceScreen, InGameScreen, GameOverScreen, CreditScreen }
public class ChangeGameEvent : UnityEvent<GameEvent> { }
public class ChangeGameState : UnityEvent<GameState> { }

public class StateManager
{
	public static ChangeGameEvent gameEvent = new ChangeGameEvent();
	public static ChangeGameState gameState = new ChangeGameState();
	public static GameState state { get; private set; }
	
	public static void SetState(GameState newState)
	{
		if(state == newState) return;
		state = newState;
		StateManager.gameState.Invoke(state);
	}
}