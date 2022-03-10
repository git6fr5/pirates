using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureUIExit : MonoBehaviour
{

    private bool m_Active = false;
    public bool Active => m_Active;

    void OnMouseDown() {
        m_Active = true;
    }

}
