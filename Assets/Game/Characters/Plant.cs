using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Plant : Character {

    /* --- Variables --- */
    #region Variables

    // Settings.
    [SerializeField] private int m_VisisonDistance;
    [SerializeField, ReadOnly] private bool m_PlayerInVision;

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_VisionIndicators;
    private List<SpriteRenderer> m_TargetIndicators;
    private List<SpriteRenderer> m_HeartIndicators;
    private List<SpriteRenderer> m_DamageIndicators;

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
        Action action = Action.Pass;

        bool canActiveCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (canActiveCard) {
            action = AttemptCardAction();
            if (action != Action.Pass) {
                return action;
            }
        }

        return action;
    }

    protected override Vector2Int? GetTarget(Card card) {
        List<Vector2Int> targettablePositions = card.GetTargetablePositions(m_Board, m_Position);
        for (int i = 0; i < targettablePositions.Count; i++) {
            if (m_Board.GetAt<Player>(targettablePositions[i]) != null) {
                return targettablePositions[i];
            }
        }
        return null;
    }

    private Action AttemptCardAction() {
        List<Vector2Int> targettablePositions = m_Cards[0].GetTargetablePositions(m_Board, m_Position);
        for (int i = 0; i < targettablePositions.Count; i++) {
            if (m_Board.GetAt<Player>(targettablePositions[i]) != null) {
                return Action.CardSlot0;
            }
        }
        return Action.Pass;
    }

    protected override bool WaitForEndOfAction() {
        return false;
    }

    #endregion

    /* --- UI --- */
    #region UI

    protected override void SetUI() {
        bool playerHasActiveCard = PlayerHasActiveCard();

        BoardUI.DrawVisionUI(m_VisisonDistance, m_Board, m_Position, ref m_VisionIndicators, m_MouseOver && !playerHasActiveCard);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, m_MouseOver);

        bool hasCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (hasCard) {
            BoardUI.DrawDamageUI(m_Cards[0].Value, m_Position, ref m_DamageIndicators, m_MouseOver);
            BoardUI.DrawTargetUI(m_Cards[0], m_Board, m_Position, ref m_TargetIndicators, m_MouseOver && !playerHasActiveCard);
        }
    }

    protected override void ClearUI() {
        BoardUI.DrawVisionUI(m_VisisonDistance, m_Board, m_Position, ref m_VisionIndicators, false);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, false);

        bool hasCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (hasCard) {
            BoardUI.DrawDamageUI(m_Cards[0].Value, m_Position, ref m_DamageIndicators, false);
            BoardUI.DrawTargetUI(m_Cards[0], m_Board, m_Position, ref m_TargetIndicators, false);
        }
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
