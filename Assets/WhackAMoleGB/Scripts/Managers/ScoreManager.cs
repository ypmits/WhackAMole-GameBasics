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
		CheckScores();
	}

	public static void AddScore(int num)
	{
		score += num;
	}

	public static void CheckScores()
	{
		if(score > hiscore) hiscore = score;
	}
}