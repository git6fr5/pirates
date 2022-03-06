using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : Character {

    /* --- Variables --- */
    #region Variables

    // Settings.
    [SerializeField] private int m_MovementActions;
    [SerializeField] private int m_VisisonDistance;
    [SerializeField, ReadOnly] private bool m_PlayerInVision;
    [SerializeField, ReadOnly] private bool m_TookCardAction = false;

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_VisionIndicators;
    private List<SpriteRenderer> m_TargetIndicators;
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
        Action action = Action.Pass;

        bool canActiveCard = !m_TookCardAction && m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (canActiveCard) {
            action = AttemptCardAction();
            if (action != Action.Pass) {
                m_TookCardAction = true;
                return action;
            }
        }
        
        if (m_TookCardAction) {
            m_TookCardAction = false;
            return Action.Pass;
        }

        if (m_ActionsTaken < m_MovementActions) {
            action = AttemptMoveAction();
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

    private Action AttemptMoveAction() {
        Player player = m_Board.Get<Player>();
        if (player != null) {
            m_PlayerInVision = m_Board.WithinRadius(this, player, m_VisisonDistance);
            if (m_PlayerInVision) {
                return MoveTowards(player);
            }
            else {
                List<Action> moves = new List<Action>();
                for (int i = 0; i < 4; i++) {
                    if (CheckMove((Action)i)) {
                        moves.Add((Action)i);
                    }
                }
                if (moves.Count > 0) {
                    return moves[Random.Range(0, moves.Count - 1)];
                }
            }
        }
        return Action.Pass;
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
            BoardUI.DrawTargetUI(m_Cards[0], m_Board, m_Position, ref m_TargetIndicators, m_MouseOver && !playerHasActiveCard);
        }
    }

    protected override void ClearUI() {
        BoardUI.DrawVisionUI(m_VisisonDistance, m_Board, m_Position, ref m_VisionIndicators, false);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, false);

        bool hasCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (hasCard) {
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
