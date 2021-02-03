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
