using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Wall : Character {

    /* --- Variables --- */
    #region Variables

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_HeartIndicators;

    #endregion

    /* --- Unity --- */
    #region Unity

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    #endregion

    /* --- Decision --- */
    #region Decision

    protected override Action GetAction() {
        return Action.Pass;
    }

    #endregion

    /* --- UI --- */
    #region UI

    protected override void SetUI() {
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, m_MouseOver);
    }

    protected override void ClearUI() {
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, false);
    }

    #endregion

}
