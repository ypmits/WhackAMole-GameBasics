using UnityEngine.Events;

public enum GameEvent { GoHome, ChooseLevel, ShowExplanation, StartCountDown, StartGame, RestartGame, GameOver, Whack, Continue, Pause }
public enum GameState { None, HomeScreen, LevelChoiceScreen, InGameScreen, GameOverScreen, CreditScreen, PauseScreen }
public class ChangeGameEvent : UnityEvent<GameEvent> { }
public class ChangeGameState : UnityEvent<GameState> { }

public class StateManager
{
	public static ChangeGameEvent gameEvent = new ChangeGameEvent();
	public static ChangeGameState gameState = new ChangeGameState();
	public static GameState state;
	public static bool isAlive = false;
	public static bool isPaused = true;
}