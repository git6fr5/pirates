using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTreasure : Treasure
{
    protected override void Init() {

        // print("Initializing treasure chest");
        

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        // m_BaseOffset = m_SpriteRenderer.material.GetVector("_Offset").y;
        StartCoroutine(IEJump());

    }

}
