using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

public class Zombie : Character {

    [SerializeField] private int m_VisisonDistance;
    [SerializeField] private bool m_PlayerInVision;

    [SerializeField] private int m_MovementActions;
    public bool m_TookCardAction = false;

    protected override Action GetAction() {
        Action action = Action.Pass;

        Player player = m_Board.Get<Player>();

        if (!m_TookCardAction && m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null) {
            print("trying to use a card.");
            m_Cards[0].GetTargetablePositions(m_Board, m_Position);
            List<Vector2Int> targettablePositions = m_Cards[0].TargetablePositions;
            for (int i = 0; i < targettablePositions.Count; i++) {
                if (m_Board.GetAt<Player>(targettablePositions[i]) != null) {
                    m_TookCardAction = true; ;
                    return Action.CardSlot0;
                }
            }
        }

        if (m_TookCardAction) {
            m_TookCardAction = false;
            return Action.Pass;
        }

        if (m_ActionsTaken < m_MovementActions) {
            m_PlayerInVision = m_Board.WithinRadius(this, player, m_VisisonDistance);
            if (m_PlayerInVision) {
                action = MoveTowards(player);
            }
            if (action != Action.None) {
                return action;
            }
        }
        return Action.Pass;
    }

    protected override void Draw() {
        base.Draw();

        Vector3 origin = transform.position;
        Gizmos.DrawWireCube(origin, new Vector3(2f * m_VisisonDistance + 1,2f * m_VisisonDistance + 1, 1f));
        
        float halfHearts = m_MaxHearts % 2 == 0 ? (float)(m_MaxHearts / 2f) - 0.5f : (float)(m_MaxHearts / 2f);
        for (int i = 0; i < m_MaxHearts; i++) {
            if (i < m_Hearts) {
                Gizmos.DrawWireSphere(transform.position - Vector3.up * 0.5f + Vector3.right * 0.4f * (-halfHearts + i), 0.2f);
            }
        }

    }

    //
    protected override Vector2Int? GetTarget(Card card) {
        List<Vector2Int> targettablePositions = m_Cards[0].TargetablePositions;
        for (int i = 0; i < targettablePositions.Count; i++) {
            if (m_Board.GetAt<Player>(targettablePositions[i]) != null) {
                return targettablePositions[i];
            }
        }
        return null;
    }

    /* --- Indicators --- */

    private bool m_MouseOver = false;
    public Sprite m_VisionIndicator;
    List<SpriteRenderer> m_VisionIndicators = new List<SpriteRenderer>();
    
    public Sprite m_HeartIcon;
    List<SpriteRenderer> m_HeartIndicators = new List<SpriteRenderer>();

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    void LateUpdate() {

        bool playerHasActiveCard = false;
        Player player = m_Board.Get<Player>();
        if (player != null && player.Cards != null) {
            for (int i = 0; i < player.Cards.Length; i++) {
                if (player.Cards[i] != null && player.Cards[i].Active) {
                    playerHasActiveCard = true;
                    break;
                }
            }
        }

        DrawVision(m_MouseOver && !playerHasActiveCard);
        DrawHearts(m_MouseOver);

        if (m_Cards != null && m_Cards.Length > 0 && m_Cards[0] != null) {
            m_Cards[0].DrawTargetablePositions(m_Board, m_Position, m_MouseOver && !playerHasActiveCard);
        }
    }

    private void DrawVision(bool redraw = false) {

        if (m_VisionIndicators != null) {
            for (int i = 0; i < m_VisionIndicators.Count; i++) {
                SpriteRenderer oldIndicator = m_VisionIndicators[i];
                Destroy(oldIndicator.gameObject);
            }
        }
        m_VisionIndicators = new List<SpriteRenderer>();

        if (redraw) {
            for (int i = -m_VisisonDistance; i < m_VisisonDistance + 1; i++) {
                for (int j = -m_VisisonDistance; j < m_VisisonDistance + 1; j++) {

                    SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                    indicator.sortingOrder = 1;
                    indicator.transform.position = (Vector3)(Vector2)(m_Position + new Vector2Int(j, i));
                    indicator.color = new Color(1f, 1f, 1f, 0.25f);
                    indicator.transform.SetParent(transform);
                    indicator.sprite = m_VisionIndicator;
                    m_VisionIndicators.Add(indicator);

                }
            }
        }
        

    }

    private void DrawHearts(bool redraw = false) {

        if (m_HeartIndicators != null) {
            for (int i = 0; i < m_HeartIndicators.Count; i++) {
                SpriteRenderer oldIndicator = m_HeartIndicators[i];
                Destroy(oldIndicator.gameObject);
            }
        }
        m_HeartIndicators = new List<SpriteRenderer>();

        if (redraw) {
            for (int i = 0; i < m_Hearts; i++) {
                SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                indicator.sortingOrder = 1;
                indicator.transform.position = ((Vector3)(Vector2)m_Position)+ new Vector3(i, 1.25f, 0f);
                indicator.transform.SetParent(transform);
                indicator.color = new Color(1f, 1f, 1f, 0.5f);
                indicator.sprite = m_HeartIcon;
                m_HeartIndicators.Add(indicator);
            }
        }

    }

}
