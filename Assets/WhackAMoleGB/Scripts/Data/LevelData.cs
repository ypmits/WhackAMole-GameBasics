using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Difficulty { Easy, Normal, Hard }
[CreateAssetMenu(fileName = "LevelData", menuName = "WhackAMoleGamebasics/LevelData", order = 0)]
public class LevelData : ScriptableObject {
	[Tooltip("The difficulty of the level. Every level has it's own parameters")] public Difficulty difficulty = Difficulty.Easy;
	[Tooltip("This is the amount of seconds this level starts with")] public float spawnSpeed = 3f;
	[Tooltip("Set to true if you want the level to be time-based, false if you want to have an endless game")] public bool timeBased = false;
	[Range(.1f, .99f), Tooltip("This is the time that is taken away from the startSpeed to the startSpeed")] public float moleSpawnDecrement = .95f;
	[Range(.01f, .99f), Tooltip("")] public float minimumSpawnDecrement = .95f;
	[Tooltip("The total time of this level")] public float totalTime = 10f;
	[Tooltip("The time that is used to respawn the next mole, when the user clicks on a mole")] public float timeUsedWhenClicked = 1f;
	[TextArea, Tooltip("Explanation Text that will show when the game starts")] public string explanation = "";
	[Tooltip("The different moles that can be picked in this level. A random mole will be picked by the game")] public List<GameObject> moles;
}