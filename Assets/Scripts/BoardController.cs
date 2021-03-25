using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class BoardController : MonoBehaviour
{
    [Header("UI")]
    public PauseController PauseController;
    public SpriteRenderer Preview;

    [Header("Item")]
    public Transform ItemContainer;
    [AssetsOnly]
    public GameObject ItemPrefab;

    [Header("Input")]
    public PlayerInput PlayerInput;
    public CanvasGroup Controller;
    public float RepeatKeyDelay = 0.25f;

    [Header("GameData")]
    public GameDataSO GameData;

    [Header("Events")]
    public GameEvent StartEvent;
    public GameEvent GameOverEvent;
    public GameEvent PlaceBrickEvent;
    public GameEvent ClearLineEvent;

    BrickGenerator BrickGenerator;

    bool _isPlaying = false;

    Coroutine _movementHandler;
    Coroutine _landingHandler;

    InputActionMap _inputMapGame;

    private void Awake() {
        GameData.BoardItems = new BoardItem[10, 20];
        GameData.LineCounts = new int[20];
        for(int y = 0; y < 20; y++) {
            for(int x = 0; x < 10; x++) {
                GameObject item = Instantiate(ItemPrefab, ItemContainer.localPosition + new Vector3(x, y, 0), Quaternion.identity);
                item.transform.SetParent(ItemContainer);
                GameData.BoardItems[x, y] = item.GetComponent<BoardItem>();
            }
        }

        BrickGenerator = GetComponent<BrickGenerator>();
        _inputMapGame = PlayerInput.actions.FindActionMap("GAME");
    }

    private void Update() {
        if(!_isPlaying) return;

        if(!GameData.hasBrick) {
            GenerateBrick();
            ShowPreview();
        }

        if(GameData.CountFallDelay(Time.deltaTime)) {
            FallBrick();
        }
    }

    public void StartGame() {
        ResetGame();
        _inputMapGame.Enable();
        Controller.interactable = true;
        
        Time.timeScale = 1f;
        ItemContainer.gameObject.SetActive(true);

        StartEvent.Invoke();

        _isPlaying = true;
    }

    public void StartGame(bool isPractice) {
        GameData.SetMode(isPractice);
        StartGame();
    }

    public void PauseGame() {
        Time.timeScale = 0;
        ItemContainer.gameObject.SetActive(false);
        PauseController.Show();
        _inputMapGame.Disable();
        Controller.interactable = false;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
        ItemContainer.gameObject.SetActive(true);
        _inputMapGame.Enable();
        Controller.interactable = true;
    }
    
    public void ResetGame() {
        ClearBoard();
        GenerateBrick();
        ShowPreview();

        GameData.Initilaize();
    }

    public void GameOver() {
        _inputMapGame.Disable();
        Controller.interactable = false;

        GameOverEvent.Invoke();

        _isPlaying = false;
    }

    void GenerateBrick() {
        GameData.BrickData = BrickGenerator.GetRandomBrick();
        GameData.BrickPivot = new Vector2(4, 19);
        GameData.BrickRotation = 0;
        GameData.BrickDropPos = FindDropPos();

        if(CastBrick(0)) {
            GameOver();
            return;
        }

        RenderShadow();
        RenderBrick();
    }

    void ShowPreview() {
        BrickScriptableObject data = BrickGenerator.GetTopBrickOnBag();
        Preview.sprite = data.Preview;
    }

    void PlaceBrick() {
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(GameData.BrickPivot.x + offsets[i].x);
            int y = (int)(GameData.BrickPivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].exists = true;
                GameData.BoardItems[x, y].Place();
                GameData.LineCounts[y] += 1;
            }
        }

        ClearFulledLine();
        GameData.BrickData = null;

        GameData.AddScore(GameDataSO.ScoreType.PLACE);

        PlaceBrickEvent.Invoke();
    }

    void RenderBrick() {
        Vector2 pivot = GameData.BrickPivot;
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].Render(GameData.BrickData.Blocks[i], rotation);
            }
        }
    }

    void EraseBrick() {
        Vector2 pivot = GameData.BrickPivot;
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].Erase();
            }
        }
    }

    void RenderShadow() {
        Vector2 pivot = GameData.BrickDropPos;
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].RenderShadow(GameData.BrickData.Blocks[i], rotation);
            }
        }
    }

    void EraseShadow() {
        Vector2 pivot = GameData.BrickDropPos;
        int rotation = GameData.BrickRotation;
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(pivot.x + offsets[i].x);
            int y = (int)(pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                GameData.BoardItems[x, y].Erase();
            }
        }
    }

    bool CastBrick(Vector2 targetPivot, int rotation) {
        Vector2[] offsets = GameData.BrickData.Offsets[rotation];
        
        for(int i = 0; i < 4; i++) {
            float ox = offsets[i].x;
            float oy = offsets[i].y;
            int x = (int)(targetPivot.x + ox);
            int y = (int)(targetPivot.y + oy);

            // 벽충돌 검사
            if(x < 0 || x >= 10 || y < 0) {
                return true;
            }
            // 블럭충돌 검사
            else if(y < 20) {
                if(GameData.BoardItems[x, y].exists) {
                    return true;
                }
            }
        }

        return false;
    }

    bool CastBrick(Vector2 targetPivot) {
        int rotation = GameData.BrickRotation;
        return CastBrick(targetPivot, rotation);
    }

    bool CastBrick(int rotation) {
        Vector2 targetPivot = GameData.BrickPivot;
        return CastBrick(targetPivot, rotation);
    }

    Vector2 FindDropPos() {
        Vector2 dropPos = GameData.BrickPivot;
        while(!CastBrick(dropPos)) {
            dropPos += Vector2.down;
        }
        return dropPos + Vector2.up;
    }

    void FallBrick() {
        if(CastBrick(GameData.BrickPivot + Vector2.down)) {
            PlaceBrick();
            return;
        }

        EraseBrick();

        GameData.BrickPivot += Vector2.down;

        RenderBrick();
    }

    void ClearFulledLine() {
        int removedCount = 0;

        for(int l = 19; l >= 0; l--) {
            if(GameData.LineCounts[l] == 10) {
                for(int x = 0; x < 10; x++) {
                    GameData.BoardItems[x, l].Boom();
                }

                for(int y = l; y < 19; y++) {
                    for(int x = 0; x < 10; x++) {
                        GameData.BoardItems[x, y].exists = GameData.BoardItems[x, y + 1].exists;
                        GameData.BoardItems[x, y].Render(GameData.BoardItems[x, y + 1]);
                    }
                    GameData.LineCounts[y] = GameData.LineCounts[y + 1];
                }
                for(int x = 0; x < 10; x++) {
                    GameData.BoardItems[x, 19].exists = false;
                    GameData.BoardItems[x, 19].Erase();
                }
                GameData.LineCounts[19] = 0;
                
                GameData.AddScore(GameDataSO.ScoreType.CLEAR);
                GameData.AddClearLine();

                removedCount += 1;
            }
        }

        if(removedCount >= 4) {
            GameData.AddScore(GameDataSO.ScoreType.FULLCLEAR);
        }

        if(removedCount > 0) ClearLineEvent.Invoke();
    }

    void ClearBoard() {
        for(int y = 0; y < 20; y++) {
            for(int x = 0; x < 10; x++) {
                GameData.BoardItems[x, y].Erase();
                GameData.BoardItems[x, y].exists = false;
            }
            GameData.LineCounts[y] = 0;
        }
    }

    IEnumerator HandleMove(float input) {
        while(true) {
            Vector2 direction = new Vector2(input, 0);

            if(!CastBrick(GameData.BrickPivot + direction)) {
                EraseBrick();
                EraseShadow();
                GameData.BrickPivot += direction;
                GameData.BrickDropPos = FindDropPos();
                RenderShadow();
                RenderBrick();
            }
            
            yield return new WaitForSeconds(RepeatKeyDelay);
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        if(context.performed) {
            float input = context.ReadValue<float>();
            OnMoveDown(input);
        }
        if(context.canceled) {
            OnMoveUp();
        }
    }

    public void OnMoveDown(float input) {
        if(_movementHandler == null) {
            _movementHandler = StartCoroutine(HandleMove(input));
        }
    }

    public void OnMoveUp() {
        if(_movementHandler != null) {
            StopCoroutine(_movementHandler);
            _movementHandler = null;
        }
    }

    IEnumerator HandleLand() {
        while(true) {
            if(!CastBrick(GameData.BrickPivot + Vector2.down)) {
                EraseBrick();
                GameData.BrickPivot += Vector2.down;
                RenderBrick();
                GameData.ResetDelay();
            }
            
            yield return new WaitForSeconds(RepeatKeyDelay);
        }
    }

    public void OnLand(InputAction.CallbackContext context) {
        if(context.performed) {
            OnLandDown();
        }
        if(context.canceled) {
            OnLandUp();
        }
    }

    public void OnLandDown() {
        if(_landingHandler == null) {
            _landingHandler = StartCoroutine(HandleLand());
        }
    }

    public void OnLandUp() {
        if(_landingHandler != null) {
            StopCoroutine(_landingHandler);
            _landingHandler = null;
        }
    }
    
    public void OnDrop(InputAction.CallbackContext context) {
        if(context.performed) {
            OnDropDown();
        }
    }

    public void OnDropDown() {
        Vector2 pivot = GameData.BrickPivot;
        int rotation = GameData.BrickRotation;

        EraseBrick();

        GameData.BrickPivot = GameData.BrickDropPos;

        RenderBrick();
        PlaceBrick();
        GameData.ResetDelay();
    }

    public void OnRotate(InputAction.CallbackContext context) {
        if(context.performed) {
            OnRotateDown();
        }
    }

    public void OnRotateDown() {
        int rotation = (GameData.BrickRotation + 1) % 4;
        if(CastBrick(rotation)) return;

        EraseBrick();
        EraseShadow();
        GameData.BrickRotation = rotation;
        GameData.BrickDropPos = FindDropPos();
        RenderShadow();
        RenderBrick();
    }

    public void OnPause(InputAction.CallbackContext context) {
        if(context.performed) {
            PauseGame();
        }
    }
}
