using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardItem : MonoBehaviour
{
    public bool exists;

    [SerializeField]
    Animator animator;

    [SerializeField]
    SpriteRenderer spriteRenderer;
    
    public void Render(Sprite sprite, int rotation) {
        spriteRenderer.sprite = sprite;
        transform.rotation = Quaternion.Euler(0, 0, -90f * rotation);
    }

    public void Render(BoardItem item) {
        spriteRenderer.sprite = item.spriteRenderer.sprite;
        transform.rotation = item.transform.rotation;
    }

    public void Erase() {
        spriteRenderer.sprite = null;
        transform.rotation = Quaternion.identity;
    }

    public void Boom() {
        animator.SetTrigger("Boom");
    }
}
