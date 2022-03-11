using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Piece {

    public SpriteRenderer spriteRenderer;

    public Sprite activeSprite;
    public Sprite inactiveSprite;

    public bool active;

    public void Swap() {

        active = !active;
        spriteRenderer.sprite = active ? activeSprite : inactiveSprite;

    }

    void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public override void TakeDamage(int damage, float delay = 0f) {
    }

}
