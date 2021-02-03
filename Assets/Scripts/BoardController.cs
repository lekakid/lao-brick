using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class BoardController : MonoBehaviour
{
    [BoxGroup("Item")]
    public Transform ItemContainer;

    [BoxGroup("Item"), AssetsOnly]
    public GameObject ItemPrefab;

    [BoxGroup("Difficulty")]
    public float FallDelay = 1.5f;
    
    [BoxGroup("Difficulty")]
    public float SpeedUpRate = 0.95f;

    struct MappingItem {
        public SpriteRenderer spriteRenderer;
        public Transform transform;
        public bool exists;
    }
    MappingItem[,] _MappingTable;
    int[] _itemCount;

    BrickGenerator _generator;
    struct Brick {
        public bool hasValue;
        public Vector2 pivot;
        public int rotation;
        public BrickScriptableObject data;
        public SpriteRenderer[] lastRenderers;
        public Transform[] lastTransforms;
    }
    Brick _currentBrick;

    bool isPlaying = false;
    float _currentDelay = 0f;
    float _elapsedTime = 0f;

    private void Awake() {
        _MappingTable = new MappingItem[10, 20];
        _itemCount = new int[20];
        for(int y = 0; y < 20; y++) {
            for(int x = 0; x < 10; x++) {
                GameObject item = Instantiate(ItemPrefab, ItemContainer.localPosition + new Vector3(x, y, 0), Quaternion.identity);
                item.transform.SetParent(ItemContainer);
                _MappingTable[x, y].spriteRenderer = item.GetComponent<SpriteRenderer>();
                _MappingTable[x, y].transform = item.transform;
            }
        }

        _generator = GetComponent<BrickGenerator>();
    }

    private void Update() {
        if(!isPlaying) return;

        if(!_currentBrick.hasValue) {
            GenerateBrick();
        }

        if(_elapsedTime >= _currentDelay) {
            FallBrick();
        }

        _elapsedTime += Time.deltaTime;
    }

    [Button]
    void StartGame() {
        _currentDelay = FallDelay;
        isPlaying = true;
    }

    void GenerateBrick() {
        BrickScriptableObject data = _generator.GetRandomBrick();
        _currentBrick.pivot = new Vector2(4, 19);
        _currentBrick.rotation = 0;
        _currentBrick.data = data;
        if(_currentBrick.lastRenderers == null && _currentBrick.lastTransforms == null) {
            _currentBrick.lastRenderers = new SpriteRenderer[4];
            _currentBrick.lastTransforms = new Transform[4];
        }
        else {
            for(int i = 0; i < 4; i++) {
                _currentBrick.lastRenderers[i] = null;
                _currentBrick.lastTransforms[i] = null;
            }
        }
        _currentBrick.hasValue = true;

        RenderBrick();
    }

    void PlaceBrick() {
        int rotation = _currentBrick.rotation;
        Vector2[] offsets = _currentBrick.data.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            int x = (int)(_currentBrick.pivot.x + offsets[i].x);
            int y = (int)(_currentBrick.pivot.y + offsets[i].y);

            _MappingTable[x, y].exists = true;
            _itemCount[y] += 1;
        }
        
        _currentBrick.hasValue = false;
        _elapsedTime = 0f;
    }

    void RenderBrick() {
        int rotation = _currentBrick.rotation;
        Vector2[] offsets = _currentBrick.data.Offsets[rotation];

        for(int i = 0; i < 4; i++) {
            if(_currentBrick.lastRenderers[i] != null) {
                _currentBrick.lastRenderers[i].sprite = null;
                _currentBrick.lastTransforms[i].rotation = Quaternion.identity;
            }
        }

        for(int i = 0; i < 4; i++) {
            int x = (int)(_currentBrick.pivot.x + offsets[i].x);
            int y = (int)(_currentBrick.pivot.y + offsets[i].y);

            if(x < 10 && y < 20) {
                _currentBrick.lastRenderers[i] = _MappingTable[x, y].spriteRenderer;
                _currentBrick.lastTransforms[i] = _MappingTable[x, y].transform;
                _MappingTable[x, y].spriteRenderer.sprite = _currentBrick.data.Blocks[i];
                _MappingTable[x, y].transform.Rotate(0f, 0f, -90f * rotation);
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

    public void OnMove(InputAction.CallbackContext context) {
        if(context.performed) {
            Vector2 input = context.ReadValue<Vector2>();
            Vector2 direction = Vector2.zero;

            // TODO: 키를 누르고 있으면 계속 이동
            if(input.x != 0f) {
                direction = (input.x < 0) ? Vector2.left : Vector2.right;
            }
            if(input.y < 0) {
                direction = Vector2.down;
            }

            if(CastBrick(direction)) return;

            if(input.y < 0) _elapsedTime = 0f;
            _currentBrick.pivot += direction;
            RenderBrick();
        }
        if(context.canceled) {
            // TODO: 키를 누르고 있다 떼면 후처리
        }
    }
    
    public void OnDrop(InputAction.CallbackContext context) {
        if(context.performed) {
            int rotation = _currentBrick.rotation;
            Vector2 pivot = _currentBrick.pivot;

            while(!CastBrick(Vector2.down)) {
                _currentBrick.pivot += Vector2.down;
            }

            RenderBrick();
            PlaceBrick();
            _elapsedTime = 0f;
        }
    }

    public void OnRotate(InputAction.CallbackContext context) {
        if(context.performed) {
            int rotation = (_currentBrick.rotation + 1) % 4;
            if(CastBrick(rotation)) return;

            _currentBrick.rotation = rotation;
            RenderBrick();
        }
    }
}
