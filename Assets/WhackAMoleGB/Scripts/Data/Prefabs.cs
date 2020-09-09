using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Prefabs", menuName = "WhackAMoleGamebasics/Prefabs", order = 0)]
public class Prefabs : ScriptableObject {
	[Header("Levels")]
	public LevelData levelEasy;
	public LevelData levelMedium;
	public LevelData levelHard;
	[Header("Sound")]
	public AudioClip musicMenu;
	public AudioClip musicGame;
	public AudioClip failureTune;
	public AudioClip hiScoreSound;
	public AudioClip buttonClick;
	public AudioClip buttonClickBack;
	public AudioClip buttonClickConfirm;
	[Header("Effects")]
	public GameObject groundParticles;

	public LevelData GetLevelData(Difficulty difficulty)
	{
		switch (difficulty)
		{
			case Difficulty.Easy: return GameController.refs.prefabs.levelEasy;
			case Difficulty.Normal: return GameController.refs.prefabs.levelMedium;
			case Difficulty.Hard: return GameController.refs.prefabs.levelHard;
		}
		return null;
	}
}