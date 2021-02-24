using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    AudioMixerController AudioMixerController;
    [SerializeField]
    BoardController BoardController;
    [SerializeField]
    Animator TitleAnimator;
    [SerializeField]
    ModeController ModeController;
    [SerializeField]
    Animator PauseGroupAnimator;
    [SerializeField]
    GameObject StartButton;
    [SerializeField]
    bool isFocus;

    private void Start() {
        AudioMixerController.PlayBGM("Title");
        Show();
    }

    public void Show() {
        TitleAnimator.SetBool("Toggle", true);
        isFocus = true;
    }

    public void Hide() {
        TitleAnimator.SetBool("Toggle", false);
        isFocus= false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnKeyDown() {
        if(!isFocus) return;
        if(EventSystem.current.currentSelectedGameObject) return;

        EventSystem.current.SetSelectedGameObject(StartButton);
    }

    public void OnClickStart() {
        Hide();
        ModeController.Show();
    }

    public void OnClickOption() {
        PauseGroupAnimator.SetBool("Toggle", true);
    }
}
