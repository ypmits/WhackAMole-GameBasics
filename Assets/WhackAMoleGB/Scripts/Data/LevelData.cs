using System.Collections.Generic;
using UnityEngine;

public enum Difficulty { Easy, Medium, Hard }
[CreateAssetMenu(fileName = "LevelData", menuName = "WhackAMoleGamebasics/LevelData", order = 0)]
public class LevelData : ScriptableObject {
	[Tooltip("The difficulty of the level")] public Difficulty difficulty = Difficulty.Easy;
	[Tooltip("This is the amount of seconds this level starts with")] public float startSpeed = 3f;
	[Tooltip("")] public float totalTime = 10f;
	[Tooltip("")] public bool timeBased = false;
	public List<GameObject> moles;
}