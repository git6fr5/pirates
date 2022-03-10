using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Wall : Piece {

    /* --- Variables --- */
    #region Variables

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_HeartIndicators;

    //bool m_InitDelay = false;
    //float m_InitDelayTicks = 0f;

    #endregion

    /* --- Unity --- */
    #region Unity

    //protected override void Think() {
    //    if (!m_InitDelay) {
    //        m_InitDelayTicks += Time.deltaTime;
    //        if (m_InitDelayTicks > 5f) {
    //            m_InitDelay = true;
    //        }
    //    }

    //    if (m_InitDelay) {
    //        Snap();
    //    }
    //}

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    #endregion

    /* --- UI --- */
    #region UI
    void OnDestroy() {
        ClearUI();
    }

    private void SetUI() {
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, m_MouseOver);
    }

    private void ClearUI() {
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, false);
    }

    #endregion

}
