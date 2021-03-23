using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private void FixedUpdate() {
        transform.Translate(Vector2.left * Time.fixedDeltaTime);
        if(transform.position.x < -15f) {
            transform.Translate(Vector3.right * 30f);
        }
    }
}
