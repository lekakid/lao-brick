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

    public void Show() {
        ModeAnimator.SetBool("Toggle", true);
        EventSystem.current.SetSelectedGameObject(PracticeButton);
    }

    public void OnClickPractice() {
        boardController.StartGame(true);
        ModeAnimator.SetBool("Toggle", false);
    }

    public void OnClickStandard() {
        boardController.StartGame(false);
        ModeAnimator.SetBool("Toggle", false);
    }
}
