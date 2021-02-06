using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class BoardController : MonoBehaviour
{
    [BoxGroup("UI")]
    public SpriteRenderer Preview;
    
    [BoxGroup("UI")]
    public Animator GameOverAnimator;

    [BoxGroup("UI")]
    public Animator DracurinaAnimator;

    [BoxGroup("UI")]
    public Text HighScoreText;

    [BoxGroup("UI")]
    public Text ScoreText;

    [BoxGroup("Input")]
    public PlayerInput PlayerInput;

    [BoxGroup("Input")]
    public CanvasGroup Controller;

    [BoxGroup("Input")]
    public float RepeatKeyDelay = 0.25f;

    [BoxGroup("Item")]
    public Transform ItemContainer;

    [BoxGroup("Item"), AssetsOnly]
    public GameObject ItemPrefab;

    [BoxGroup("Difficulty")]
    public float FallDelay = 1.5f;
    
    [BoxGroup("Difficulty")]
    public float SpeedUpRate = 0.95f;

    [BoxGroup("Difficulty")]
    public int LevelCut = 20;

    [BoxGroup("Score")]
    public int PlaceBlockScore = 10;

    [BoxGroup("Score")]
    public int ClearLineScore = 100;
    
    BoardItem[,] _MappingTable;
    int[] _lineCount;

    BrickGenerator _generator;
    struct Brick {
        public bool hasValue;
        public Vector2 pivot;
        public int rotation;
        public BrickScriptableObject data;
        public BoardItem[] lastItem;
    }
    Brick _currentBrick;

    bool isPlaying = false;
    float _currentDelay = 0f;
    float _elapsedTime = 0f;

    int _score = 0;
    int Score {
        get {
            return _score;
        }
        set {
            _score = value;
            ScoreText.text = string.Format("{0,8:D8}", value);
        }
    }
    int _highscore = 0;
    int _level = 1;
    int _removedLine = 0;

    Coroutine _movementHandler;

    private void Awake() {
        _MappingTable = new BoardItem[10, 20];
        _lineCount = new int[20];
        for(int y = 0; y < 20; y++) {
            for(int x = 0; x < 10; x++) {
                GameObject item = Instantiate(ItemPrefab, ItemContainer.localPosition + new Vector3(x, y, 0), Quaternion.identity);
                item.transform.SetParent(ItemContainer);
                _MappingTable[x, y] = item.GetComponent<BoardItem>();
            }
        }

        _generator = GetComponent<BrickGenerator>();
    }

    private void Update() {
        if(!isPlaying) return;

        if(!_currentBrick.hasValue) {
            GenerateBrick();
            ShowPreview();
        }

        if(_elapsedTime >= _currentDelay) {
            FallBrick();
        }

        _elapsedTime += Time.deltaTime;
    }

    public void StartGame() {
        _currentDelay = FallDelay;
        _elapsedTime = 0f;
        ClearBoard();

        Score = 0;
        _level = 1;
        _removedLine = 0;

        PlayerInput.currentActionMap = PlayerInput.actions.FindActionMap("GAME");
        Controller.interactable = true;
        
        isPlaying = true;
        GameOverAnimator.SetBool("Toggle", false);
        DracurinaAnimator.SetBool("Toggle", false);
    }

    public void GameOver() {
        PlayerInput.currentActionMap = null;
        Controller.interactable = false;

        if(Score > _highscore) {
            _highscore = Score;
            HighScoreText.text = string.Format("{0,8:D8}", _highscore);
        }

        isPlaying = false;
        GameOverAnimator.SetBool("Toggle", true);
        DracurinaAnimator.SetBool("Toggle", true);
    }

    void GenerateBrick() {
        BrickScriptableObject data = _generator.GetRandomBrick();
        _currentBrick.pivot = new Vector2(4, 19);
        _currentBrick.rotation = 0;
        _currentBrick.data = data;
        if(_currentBrick.lastItem == null) {
            _currentBrick.lastItem = new BoardItem[4];
        }
        else {
            for(int i = 0; i < 4; i++) {
                _currentBrick.lastItem[i] = null;
            }
        }

        if(CastBrick(0)) {
            GameOver();
            return;
        }

        _currentBrick.hasValue = true;

        RenderBrick();
    }

    void ShowPreview() {
        BrickScriptableObject data = _generator.GetTopBrickOnBag();
        Preview.sprite = data.Preview;
    }

    void PlaceBrick() {
        int rotation = _currentBrick.rotation;
        Vector2[] offsets = _currentBrick.data.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(_currentBrick.pivot.x + offsets[i].x);
            int y = (int)(_currentBrick.pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                _MappingTable[x, y].exists = true;
                _lineCount[y] += 1;
            }
        }

        ClearFulledLine();
        _currentBrick.hasValue = false;
        _elapsedTime = 0f;

        Score += PlaceBlockScore;
    }

    void RenderBrick() {
        Vector2 pivot = _currentBrick.pivot;
        int rotation = _currentBrick.rotation;
        Vector2[] offsets = _currentBrick.data.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            if(_currentBrick.lastItem[i] != null) {
                _currentBrick.lastItem[i].Erase();
            }
        }

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                _currentBrick.lastItem[i] = _MappingTable[x, y];
                _MappingTable[x, y].Render(_currentBrick.data.Blocks[i], rotation);
            }
        }
    }

    bool CastBrick(Vector2 direction) {
        int rotation = _currentBrick.rotation;
        Vector2 pivot = _currentBrick.pivot + direction;
        Vector2[] offsets = _currentBrick.data.Offsets[rotation];
        
        for(int i = 0; i < 4; i++) {
            float ox = offsets[i].x;
            float oy = offsets[i].y;
            int x = (int)(pivot.x + ox);
            int y = (int)(pivot.y + oy);

            // 벽충돌 검사
            if(x < 0 || x >= 10 || y < 0) {
                return true;
            }
            // 블럭충돌 검사
            else if(y < 20) {
                if(_MappingTable[x, y].exists) {
                    return true;
                }
            }
        }

        return false;
    }

    bool CastBrick(int rotation) {
        Vector2 pivot = _currentBrick.pivot;
        Vector2[] offsets = _currentBrick.data.Offsets[rotation];
        
        for(int i = 0; i <4; i++) {
            float ox = offsets[i].x;
            float oy = offsets[i].y;
            int x = (int)(pivot.x + ox);
            int y = (int)(pivot.y + oy);

            if(x < 0 || x >= 10 || y < 0) {
                return true;
            }
            else if(y < 20) {
                if(_MappingTable[x, y].exists) {
                    return true;
                }
            }
        }

        return false;
    }

    void FallBrick() {
        int rotation = _currentBrick.rotation;
        if(CastBrick(Vector2.down)) {
            PlaceBrick();
            return;
        }
        _currentBrick.pivot += Vector2.down;
        _elapsedTime = 0f;

        RenderBrick();
    }

    void ClearFulledLine() {
        for(int l = 19; l >= 0; l--) {
            if(_lineCount[l] == 10) {
                for(int x = 0; x < 10; x++) {
                    _MappingTable[x, l].Boom();
                }
                if(l == 19) {
                    for(int x = 0; x < 10; x++) {
                        _MappingTable[x, l].exists = false;
                        _MappingTable[x, l].Erase();
                    }
                    _lineCount[l] = 0;
                }
                else {
                    for(int y = l; y < 19; y++) {
                        for(int x = 0; x < 10; x++) {
                            _MappingTable[x, y].exists = _MappingTable[x, y + 1].exists;
                            _MappingTable[x, y].Render(_MappingTable[x, y + 1]);
                        }
                        _lineCount[y] = _lineCount[y + 1];
                    }
                }
                Score += ClearLineScore * _level;
                _removedLine += 1;
                
                if(_removedLine >= 20) {
                    _level += 1;
                    _removedLine -= 20;
                    _currentDelay *= SpeedUpRate;
                }
            }
        }
    }

    void ClearBoard() {
        for(int y = 0; y < 20; y++) {
            for(int x = 0; x < 10; x++) {
                _MappingTable[x, y].Erase();
                _MappingTable[x, y].exists = false;
            }
            _lineCount[y] = 0;
        }
    }

    IEnumerator HandleMove(Vector2 direction) {
        while(true) {
            if(!CastBrick(direction)) {
                if(direction.y < 0) _elapsedTime = 0f;
                _currentBrick.pivot += direction;
                RenderBrick();
            }
            
            yield return new WaitForSeconds(RepeatKeyDelay);
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        if(context.performed) {
            Vector2 input = context.ReadValue<Vector2>();
            OnMoveDown(input);
        }
        if(context.canceled) {
            OnMoveUp();
        }
    }

    public void OnMoveDown(Vector2 input) {
            Vector2 direction = Vector2.zero;

            if(input.x != 0f) {
                direction = (input.x < 0) ? Vector2.left : Vector2.right;
            }
            else if(input.y < 0) {
                direction = Vector2.down;
            }

            if(_movementHandler != null) StopCoroutine(_movementHandler);
            _movementHandler = StartCoroutine(HandleMove(direction));
    }

    public void OnMoveUp() {
        StopCoroutine(_movementHandler);
        _movementHandler = null;
    }
    
    public void OnDrop(InputAction.CallbackContext context) {
        if(context.performed) {
            OnDropDown();
        }
    }

    public void OnDropDown() {
        int rotation = _currentBrick.rotation;
        Vector2 pivot = _currentBrick.pivot;

        while(!CastBrick(Vector2.down)) {
            _currentBrick.pivot += Vector2.down;
        }

        RenderBrick();
        PlaceBrick();
        _elapsedTime = 0f;
    }

    public void OnRotate(InputAction.CallbackContext context) {
        if(context.performed) {
            OnRotateDown();
        }
    }

    public void OnRotateDown() {
        int rotation = (_currentBrick.rotation + 1) % 4;
        if(CastBrick(rotation)) return;

        _currentBrick.rotation = rotation;
        RenderBrick();
    }
}
