using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardController : MonoBehaviour
{
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
