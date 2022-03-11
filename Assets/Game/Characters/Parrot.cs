using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Parrot : Character {

    /* --- Variables --- */
    #region Variables

    // Difficulty.
    [SerializeField] private Difficulty m_Difficulty;
    public Difficulty EnemyDifficulty => m_Difficulty;

    // Settings.
    [SerializeField] private int m_VisisonDistance;

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_VisionIndicators;
    private List<SpriteRenderer>[] m_TargetIndicators;
    private List<SpriteRenderer> m_HeartIndicators;
    private List<SpriteRenderer> m_DamageIndicators;

    #endregion

    List<Vector2Int> targets;

    void Awake() {
        m_TargetIndicators = new List<SpriteRenderer>[m_ActionsPerTurn];
        GetRandomTargets();
    }

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    protected override void CompleteTurn() {
        GetRandomTargets();
        base.CompleteTurn();
    }


    protected override Action GetAction() {
        Action action = Action.Pass;

        bool canActiveCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (canActiveCard) {
            return Action.CardSlot0;
        }

        return action;
    }

    protected override Vector2Int? GetTarget(Card card) {
        if (m_ActionsTaken < targets.Count) {
            return targets[m_ActionsTaken];
        }
        return targets[0];
    }

    private void GetRandomTargets() {
        targets = new List<Vector2Int>();
        for (int i = 0; i < m_ActionsPerTurn; i++) {
            int y = Random.Range(0, m_Board.Height);
            int x = Random.Range(0, m_Board.Width);
            if (x <= m_Position.x + 1 && x >= m_Position.x - 1) {
                x = m_Position.x - 2;
            }
            if (y <= m_Position.y + 1 && y >= m_Position.y - 1) {
                y = m_Position.y - 2;
            }
            Vector2Int position = new Vector2Int(x, y);
            targets.Add(position);
        }
    }

    #region UI

    protected override void SetUI() {
        bool playerHasActiveCard = PlayerHasActiveCard();

        BoardUI.DrawVisionCharactersUI(targets.ToArray(), m_Board, m_Position, ref m_VisionIndicators, m_MouseOver && !playerHasActiveCard);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, m_MouseOver);


        bool hasCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (hasCard) {
            BoardUI.DrawDamageUI(m_Cards[0].Value, m_Position, ref m_DamageIndicators, m_MouseOver);
            for (int i = 0; i < targets.Count; i++) {
                BoardUI.DrawTargetUI(m_Cards[0], m_Board, targets[i], ref m_TargetIndicators[i], m_MouseOver && !playerHasActiveCard);
            }
        }
    }

    protected override void ClearUI() {
        BoardUI.DrawVisionUI(m_VisisonDistance, m_Board, m_Position, ref m_VisionIndicators, false);
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, false);

        bool hasCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (hasCard) {
            BoardUI.DrawDamageUI(m_Cards[0].Value, m_Position, ref m_DamageIndicators, false);
            // BoardUI.DrawTargetUI(m_Cards[0], m_Board, m_Position, ref m_TargetIndicators, false);
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
