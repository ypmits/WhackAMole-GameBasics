using System;
using UnityEngine;

public class Model
{
	public static LevelData levelData;
	public static int score = 0;
	public static int hiscore = 0;

	public static int GetHiscore()
	{
		return PlayerPrefs.GetInt("hiscore", 0);
	}

	public static void SaveHiscore()
	{
		PlayerPrefs.SetInt("hiscore", hiscore);
	}
}