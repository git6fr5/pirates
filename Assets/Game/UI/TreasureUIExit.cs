using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureUIExit : MonoBehaviour
{

    private bool m_Active = false;
    public bool Active => m_Active;

    private SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseOver() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
    }

    void OnMouseDown() {
        m_Active = true;
    }

}
