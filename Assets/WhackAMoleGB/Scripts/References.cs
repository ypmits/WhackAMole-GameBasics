[System.Serializable]
public class References
{
	public GameController gameController;
	public UI ui;
	public Engine engine;
	public Prefabs prefabs;

	public References(GameController controller, UI ui, Engine engine, Prefabs prefabs)
	{
		this.gameController = controller;
		this.ui = ui;
		this.engine = engine;
		this.prefabs = prefabs;
	}
}