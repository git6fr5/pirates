using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToHide : MonoBehaviour
{

    public GameObject hideObject;

    public Sprite scrollOn;
    public Sprite scrollOff;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer scroll;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown() {
        hideObject.SetActive(!hideObject.activeSelf);
    }

    void OnMouseOver() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
    }


    void Update() {
        if (hideObject.activeSelf) {
            spriteRenderer.sprite = scrollOn;
        }
        else {
            spriteRenderer.sprite = scrollOff;
        }
    }


}
