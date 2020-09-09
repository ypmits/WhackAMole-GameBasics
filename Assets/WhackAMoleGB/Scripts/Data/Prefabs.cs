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
}