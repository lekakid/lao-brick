using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Start() {
        AudioMixerController.PlayBGM("Title");
    }

    public void OnClickStart() {
        Hide();
        ModeController.Show();
    }

    public void OnClickOption() {
        PauseGroupAnimator.SetBool("Toggle", true);
    }
}
