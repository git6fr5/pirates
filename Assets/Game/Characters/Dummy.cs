using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Dummy : Character {

    [SerializeField] private int m_VisisonDistance;

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_VisionIndicators;
    private List<SpriteRenderer> m_TargetIndicators;
    private List<SpriteRenderer> m_HeartIndicators;

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    /* --- UI --- */
    #region UI

    protected override void SetUI() {
        bool playerHasActiveCard = PlayerHasActiveCard();

        BoardUI.DrawVisionUI(m_VisisonDistance, m_Board, m_Position, ref m_VisionIndicators, m_MouseOver && !playerHasActiveCard);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, m_MouseOver);

    }

    protected override void ClearUI() {
        BoardUI.DrawVisionUI(m_VisisonDistance, m_Board, m_Position, ref m_VisionIndicators, false);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, false);

    }

    private bool PlayerHasActiveCard() {
        Player player = m_Board.Get<Player>();
        if (player != null && player.Cards != null) {
            for (int i = 0; i < player.Cards.Length; i++) {
                if (player.Cards[i] != null && player.Cards[i].Active) {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

}
