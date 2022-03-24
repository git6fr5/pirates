using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Crocodile : Character {

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

    public int m_ChargeCount;

    public bool init;

    #endregion

    List<Vector2Int> targets;

    void Awake() {
        CrocDeaths = 0;
    }

    void LateUpdate() {
        if (!init) {
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

    /* --- Movement --- */
    float speedFactor = 1.5f;

    protected override bool UseCard() {
        for (int i = 0; i < m_Cards.Length; i++) {
            if (m_Cards[i] != null && m_Cards[i].Active) {
                Vector2Int? target = GetTarget(m_Cards[i]);
                if (target != null) {
                    Vector2Int intTarget = new Vector2Int((int)((Vector2)target).x, (int)((Vector2)target).y);
                    if (m_Cards[i].Effect(m_Board, m_Position, intTarget)) {
                        m_Cards[i].UseCharge();
                        if (m_Cards[i].Charges <= 0) {
                            RemoveCard(i);
                        }
                        m_ActionsTaken += 1;
                        float duration = m_Board.TurnDelay;
                        PerformAction(4 + i, duration / speedFactor);
                    }
                    else {
                        m_Cards[i].Deactivate();
                    }
                }
                return true;
            }
        }
        return false;
    }

    public override void Move(float deltaTime) {
        if (m_Board == null) {
            return;
        }

        Vector2 displacement = (Vector2)m_Position - (Vector2)transform.localPosition;
        if (displacement.sqrMagnitude > 0.1f * 0.1f) {
            transform.position += (Vector3)(displacement.normalized) / (m_Board.TurnDelay / speedFactor / 1.15f) * deltaTime;
        }
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
        targets.Add(m_Position);

        for (int i = 0; i < m_ChargeCount; i++) {

            int y = Random.Range(0, m_Board.Height);
            int x = Random.Range(0, m_Board.Width);

            //if (i % 2 == 0) {
            //    y = Random.Range(0f, 1f) < 0.5f ? 0 : m_Board.Height - 1;
            //}
            //else {
            //    x = Random.Range(0f, 1f) < 0.5f ? 0 : m_Board.Width - 1;
            //}

            Vector2Int position = new Vector2Int(x, y);
            List<Vector2Int> path = m_Board.DirectManhattanPath(targets[targets.Count - 1], position);

            for (int n = 0; n < path.Count; n++) {
                targets.Add(path[n]);
                if (targets.Count >= m_ActionsPerTurn) {
                    return;
                }
            }

        }

 

    }

    public static int CrocDeaths;

    protected override void Die() {
        CrocDeaths += 1;
        if (CrocDeaths >= 3) {
            Spawn(m_Board, m_Position);
        }
        base.Die();
    }

    public Piece m_SpawnPiece;

    public void Spawn(Board board, Vector2Int target) {
        Piece piece = board.GetAt<Piece>(target);
        // Effect newEffect = m_SpawnEffect.Create(target);
        board.AddPiece(m_SpawnPiece, target);

        Debug.Log("Nothing at targetted location.");

    }

    #region UI

    protected override void SetUI() {
        bool playerHasActiveCard = PlayerHasActiveCard();

        BoardUI.DrawVisionCharactersUI(targets.ToArray(), m_Board, m_Position, ref m_VisionIndicators, m_MouseOver && !playerHasActiveCard);
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
