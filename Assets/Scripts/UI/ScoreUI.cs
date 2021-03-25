using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public GameDataSO GameData;

    public Text ScoreText;
    public Text HighScoreText;

    public void UpdateScore() {
        ScoreText.text = string.Format("{0,8:D8}", GameData.Score);
        HighScoreText.text = string.Format("{0,8:D8}", GameData.HighScore);
    }
}
