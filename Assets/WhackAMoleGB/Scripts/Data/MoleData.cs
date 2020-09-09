using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoleData", menuName = "WhackAMoleGamebasics/MoleData", order = 0)]
public class MoleData : ScriptableObject {
	[Tooltip("The tap/click-sounds. A random sound will be picked by the game")] public List<AudioClip> whackSounds;
	[Tooltip("When a mole is popping up from the ground")] public AudioClip popupSound;
	[Tooltip("What a mole says when he gets hit")] public List<AudioClip> hitSounds;
	[Tooltip("What a mole is returning without getting hit")] public AudioClip goBackSound;
	[Tooltip("The total time every mole stays visible")] public float aliveTime = 10f;
	[Tooltip("The speed in which the mole moves up and down")] public float speed = 1f;
	[Tooltip("")] public float maxLightIntensity = .7f;

	public AudioClip GetWhackSound()
	{
		return AGetRandomSound(whackSounds);
	}

	public AudioClip GetHitSound()
	{
		return AGetRandomSound(hitSounds);
	}

	private AudioClip AGetRandomSound(List<AudioClip> sounds)
	{
		if(sounds != null && sounds.Count > 0) return sounds[Random.Range(0, sounds.Count)];
		return null;
	}
}