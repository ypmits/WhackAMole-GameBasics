using System.Collections.Generic;
using UnityEngine;

public enum Difficulty { Easy, Medium, Hard }
[CreateAssetMenu(fileName = "LevelData", menuName = "WhackAMoleGamebasics/LevelData", order = 0)]
public class LevelData : ScriptableObject {
	[Tooltip("The difficulty of the level. Every level has it's own parameters")] public Difficulty difficulty = Difficulty.Easy;
	[Tooltip("This is the amount of seconds this level starts with")] public float startSpeed = 3f;
	[Tooltip("Set to true if you want the level to be time-based, false if you want to have an endless game")] public bool timeBased = false;
	[Range(.1f, .99f), Tooltip("This is the time that is multiplied to the startSpeed")] public float speedMultiplier = .95f;
	[Tooltip("The total time of this level")] public float totalTime = 10f;
	[TextArea, Tooltip("Explanation Text that will show when the game starts")] public string explanation = "";
	[Tooltip("The different moles that can be picked in this level. A random mole will be picked by the game")] public List<GameObject> moles;
}