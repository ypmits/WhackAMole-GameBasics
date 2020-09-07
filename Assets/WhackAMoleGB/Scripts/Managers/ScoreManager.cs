
using UnityEngine;
/**
<summary>
</summary>
*/
public class ScoreManager
{
	public static int score;
	public static int hiscore;

	public static void Reset()
	{
		score = 0;
		hiscore = Model.GetHiscore();
	}

	public static void AddScore(int num)
	{
		score += num;
	}

	public static bool CheckHIScores()
	{
		if(score > hiscore) {
			hiscore = score;
			Model.SaveHiscore();
			return true;
		}
		return false;
	}
}