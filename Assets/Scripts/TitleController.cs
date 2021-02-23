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
    Animator TitleGroupAnimator;
    [SerializeField]
    ModeController ModeController;
    [SerializeField]
    Animator PauseGroupAnimator;

    private void Start() {
        AudioMixerController.PlayBGM("Title");
    }

    public void OnClickStart() {
        TitleGroupAnimator.SetBool("Toggle", true);
        ModeController.Show();
    }

    public void OnClickOption() {
        PauseGroupAnimator.SetBool("Toggle", true);
    }
}
