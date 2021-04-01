using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataSO : ScriptableObject
{
    public enum ScoreType { PLACE, CLEAR, FULLCLEAR }

    [Header("Difficulty")]
    public float FallDelay = 1.5f;
    public float SpeedUpRate = 0.85f;
    public int LevelCut = 10;

    [Header("Events")]
    [SerializeField]
    GameEvent ModifiedScoreEvent;

    string _mode = "";
    int _highScore;
    int _score;
    int _level;
    int _clearLines;
    float _currentFallDelay;
    float _currentAccumulateDelay;

    public Vector2 BrickPivot;
    public Vector2 BrickDropPos;
    public int BrickRotation;
    public BrickScriptableObject BrickData;

    public BoardItem[,] BoardItems;
    public int[] LineCounts;

    public int HighScore {
        get { 
            return _highScore;
        }
    }
    public int Score {
        get {
            return _score;
        }
    }

    public string Mode {
        get { return _mode; }
    }

    public bool hasBrick {
        get {
            return BrickData != null;
        }
    }

    private void OnDisable() {
        _mode = "";
        _highScore = 0;
        _score = 0;
        _level = 1;
        _clearLines = 0;
        _currentFallDelay = 0;
        _currentAccumulateDelay = 0;
        
        BrickPivot = Vector2.zero;
        BrickDropPos = Vector2.zero;
        BrickRotation = 0;
        BrickData = null;
    }

    public void SetMode(string mode) {
        _mode = mode;
    }

    public void Initilaize() {
        _score = 0;
        _level = 1;
        _clearLines = 0;
        _currentFallDelay = FallDelay;
        _currentAccumulateDelay = 0;

        BrickPivot = Vector2.zero;
        BrickDropPos = Vector2.zero;
        BrickRotation = 0;
        BrickData = null;

        ModifiedScoreEvent.Invoke();
    }

    public void LoadHighScore() {
        _highScore = PlayerPrefs.GetInt($"HighScore_{_mode}", 0);
        ModifiedScoreEvent.Invoke();
    }

    public void SaveHighScore() {
        PlayerPrefs.SetInt($"HighScore_{_mode}", _highScore);
    }

    public void AddScore(ScoreType type) {
        int addedScore = 0;

        switch(type) {
        case ScoreType.PLACE:
            addedScore = 10;
            break;
        case ScoreType.CLEAR:
            addedScore = _mode == "Practice" ? 100 : 100 * _level;
            break;
        case ScoreType.FULLCLEAR:
            addedScore = _mode == "Practice" ? 500 : 500 * _level;
            break;
        }

        _score += addedScore;
        if(_score > _highScore) {
            _highScore = _score;
        }
        ModifiedScoreEvent.Invoke();
    }

    public void RaiseLevel() {
        if(_mode == "Practice") return;

        _level++;
        _currentFallDelay *= SpeedUpRate;
    }

    public void AddClearLine() {
        _clearLines++;

        if(_clearLines >= 20) {
            _clearLines -= 20;
            RaiseLevel();
        }
    }

    public bool CountFallDelay(float deltaTime) {
        _currentAccumulateDelay += deltaTime;

        if(_currentAccumulateDelay > _currentFallDelay) {
            _currentAccumulateDelay -= _currentFallDelay;
            return true;
        }
        return false;
    }

    public void ResetDelay() {
        _currentAccumulateDelay = 0;
    }
}
