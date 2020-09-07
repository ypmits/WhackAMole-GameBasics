using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoleData", menuName = "WhackAMoleGamebasics/MoleData", order = 0)]
public class MoleData : ScriptableObject {
	public List<AudioClip> whackSounds;

	public AudioClip GetWhackSound()
	{
		if(whackSounds != null && whackSounds.Count > 0) return whackSounds[Random.Range(0, whackSounds.Count)];
		return null;
	}
}