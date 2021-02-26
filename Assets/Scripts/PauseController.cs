using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    BoardController BoardController;
    [SerializeField]
    ModeController ModeController;
    [SerializeField]
    Animator PauseAnimator;
    [SerializeField]
    GameObject ResumeButton;
    [SerializeField]
    bool isFocus;

    public void Show() {
        PauseAnimator.SetBool("Toggle", true);
        isFocus = true;
    }

    public void Hide() {
        PauseAnimator.SetBool("Toggle", false);
        isFocus = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnKeyDown() {
        if(!isFocus) return;
        if(EventSystem.current.currentSelectedGameObject) return;

        EventSystem.current.SetSelectedGameObject(ResumeButton);
    }

    public void OnClickModeSelect() {
        BoardController.ResetGame();
        ModeController.Show();
        Hide();
    }

    public void OnClickResume() {
        if(!isFocus) return;
        BoardController.ResumeGame();
        Hide();
    }

    public void OnResume(InputAction.CallbackContext context) {
        if(context.performed) {
            OnClickResume();
        }
    }
}
