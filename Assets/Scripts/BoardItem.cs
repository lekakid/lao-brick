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

    Color _originColor = new Color(1f, 1f, 1f, 1f);
    Color _shadowColor = new Color(1f, 1f, 1f, 0.5f);
    
    public void Render(Sprite sprite, int rotation) {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = _originColor;
        transform.rotation = Quaternion.Euler(0, 0, -90f * rotation);
    }

    public void Render(BoardItem item) {
        spriteRenderer.sprite = item.spriteRenderer.sprite;
        spriteRenderer.color = _originColor;
        transform.rotation = item.transform.rotation;
    }

    public void RenderShadow(Sprite sprite, int rotation) {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = _shadowColor;
        transform.rotation = Quaternion.Euler(0, 0, -90f * rotation);
    }

    public void Erase() {
        spriteRenderer.sprite = null;
        spriteRenderer.color = _originColor;
        transform.rotation = Quaternion.identity;
    }

    public void Boom() {
        animator.SetTrigger("Boom");
    }
}
