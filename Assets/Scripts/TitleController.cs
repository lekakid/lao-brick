using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField]
    BoardController BoardController;
    [SerializeField]
    Animator TitleGroupAnimator;

    public void OnClickStart() {
        BoardController.StartGame();
        TitleGroupAnimator.SetTrigger("out");
    }
}
