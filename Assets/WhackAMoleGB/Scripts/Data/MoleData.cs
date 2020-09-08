using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoleData", menuName = "WhackAMoleGamebasics/MoleData", order = 0)]
public class MoleData : ScriptableObject {
	[Tooltip("The tap/click-sounds. A random sound will be picked by the game")] public List<AudioClip> whackSounds;
	[Tooltip("The total time every mole stays visible")] public float aliveTime = 10f;
	[Tooltip("The speed in which the mole moves up and down")] public float speed = 1f;
	[Tooltip("")] public float maxLightIntensity = .7f;

	public AudioClip GetWhackSound()
	{
		if(whackSounds != null && whackSounds.Count > 0) return whackSounds[Random.Range(0, whackSounds.Count)];
		return null;
	}
}