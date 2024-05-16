using System;

[Serializable]
public class PlayerScore
{
    public string username;
    public int score;
    
    public PlayerScore(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}