using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Goldbeard : Character {

    /* --- Variables --- */
    #region Variables

    // Difficulty.
    [SerializeField] private Difficulty m_Difficulty;
    public Difficulty EnemyDifficulty => m_Difficulty;

    // Settings.
    [SerializeField] private int m_VisisonDistance;

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer>[] m_VisionIndicators;
    private List<SpriteRenderer>[] m_TargetIndicators;
    private List<SpriteRenderer> m_HeartIndicators;
    private List<SpriteRenderer> m_DamageIndicators;

    public int m_ChargeCount;

    public bool init;

    #endregion

    List<Vector2Int> targets;

    void Awake() {
    }

    void LateUpdate() {
        if (!init) {
            m_VisionIndicators = new List<SpriteRenderer>[m_ActionsPerTurn];
            m_TargetIndicators = new List<SpriteRenderer>[m_ActionsPerTurn];
            GetRandomTargets();
            init = true;
        }
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
        return targets[targets.Count - 1];
    }

    private void GetRandomTargets() {
        targets = new List<Vector2Int>();

        for (int i = 0; i < m_ActionsPerTurn; i++) {

            int x = Random.Range(0, m_Board.Width);
            int y = Random.Range(0, m_Board.Height);

            targets.Add(new Vector2Int(x, y));

        }

    }

    protected override void Die() {
        Background background = (Background)GameObject.FindObjectOfType(typeof(Background));
        if (background != null) {
            background.WinGame();
        }
        base.Die();
    }

    #region UI

    protected override void SetUI() {
        bool playerHasActiveCard = PlayerHasActiveCard();

        for (int i = 0; i < targets.Count; i++) {
            BoardUI.DrawVisionRowUI(0, Board.FindInstance(), targets[i], ref m_VisionIndicators[i], true, false);
        }
        BoardUI.DrawHealthUI(m_Hearts, m_Position, ref m_HeartIndicators, m_MouseOver);


        bool hasCard = m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null;
        if (hasCard) {
            BoardUI.DrawDamageUI(m_Cards[0].Value, m_Position, ref m_DamageIndicators, m_MouseOver);
            //for (int i = 0; i < targets.Count; i++) {
            //    BoardUI.DrawTargetUI(m_Cards[0], m_Board, targets[i], ref m_TargetIndicators[i], m_MouseOver && !playerHasActiveCard);
            //}
        }
    }

    protected override void ClearUI() {
        for (int i = 0; i < targets.Count; i++) {
            BoardUI.DrawVisionRowUI(0, Board.FindInstance(), targets[i], ref m_VisionIndicators[i], false, false);
        }
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
