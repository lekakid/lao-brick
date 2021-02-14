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

    private void Start() {
        AudioMixerController.PlayBGM("Title");
    }

    public void OnClickStart() {
        BoardController.StartGame();
        TitleGroupAnimator.SetBool("Toggle", true);
    }
}
