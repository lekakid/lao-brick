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
    Animator PauseGroupAnimator;

    private void Start() {
        AudioMixerController.PlayBGM("Title");
    }

    public void OnClickStart() {
        BoardController.StartGame();
        TitleGroupAnimator.SetBool("Toggle", true);
    }

    public void OnClickOption() {
        PauseGroupAnimator.SetBool("Toggle", true);
    }
}
