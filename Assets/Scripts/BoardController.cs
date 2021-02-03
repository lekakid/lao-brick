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

    struct MappingItem {
        public SpriteRenderer spriteRenderer;
        public Transform transform;
        public bool exists;
    }
    MappingItem[,] _MappingTable;

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

    private void Awake() {
        _MappingTable = new MappingItem[10, 20];
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

    [Button]
    void GenerateBrick() {
        BrickScriptableObject data = _generator.GetRandomBrick();
        _currentBrick.pivot = new Vector2(4, 16);
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

    public void OnMove(InputAction.CallbackContext context) {
        if(context.performed) {
            Debug.Log("십자키 입력 시작");
            Vector2 input = context.ReadValue<Vector2>();

            if(input.x > 0) {
                Debug.Log("움직임: RIGHT");
            }
            else if(input.x < 0) {
                Debug.Log("움직임: LEFT");
            }

            if(input.y < 0) {
                Debug.Log("움직임: DOWN");
            }
        }
        if(context.canceled) {
            Debug.Log("십자키 입력 끝");
        }
    }
    
    public void OnDrop(InputAction.CallbackContext context) {
        if(context.performed) {
            Debug.Log("드랍");
        }
    }

    public void OnRotate(InputAction.CallbackContext context) {
        if(context.performed) {
            Debug.Log("회전");
        }
    }
}
