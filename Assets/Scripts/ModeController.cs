using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModeController : MonoBehaviour
{
    [SerializeField]
    BoardController boardController;
    [SerializeField]
    Animator ModeAnimator;
    [SerializeField]
    GameObject PracticeButton;
    [SerializeField]
    bool isFocus;

    public void Show() {
        ModeAnimator.SetBool("Toggle", true);
        isFocus = true;
    }

    public void Hide() {
        ModeAnimator.SetBool("Toggle", false);
        isFocus = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnKeyDown() {
        if(!isFocus) return;
        if(EventSystem.current.currentSelectedGameObject) return;

        EventSystem.current.SetSelectedGameObject(PracticeButton);
    }
}
