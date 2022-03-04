/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Card : Piece {

    public string m_CardName;
    public string CardName => m_CardName;

    public Sprite m_CardIcon;
    public Sprite CardIcon => m_CardIcon;

    public int m_Value;
    public int Value => m_Value;

    public int m_Charges;
    public int Charges => m_Charges;

    public bool m_Active;
    public bool Active => m_Active;

    public TargetType m_TargetType;
    public int m_Range;

    protected List<Vector2Int> m_TargetablePositions;
    public List<Vector2Int> TargetablePositions => m_TargetablePositions;

    public Sprite m_Effect;
    public Sprite m_Indicator;
    public List<SpriteRenderer> m_Indicators;

    public void Activate(Board board, Vector2Int origin) {
        m_Active = true;
        GetTargetablePositions(board, origin);
    }

    public void Deactivate() {
        m_Active = false;
    }

    public void UseCharge() {
        m_Active = false;
        m_Charges = m_Charges - 1;
        if (m_Charges <= 0) {
            Destroy(gameObject);
        }
    }

    public virtual bool Effect(Board board, Vector2Int target) {
        SpriteRenderer effect = new GameObject("Effect", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        effect.transform.position = (Vector3)(Vector2)target;
        effect.sprite = m_Effect;
        Destroy(effect.gameObject, board.TurnDelay);
        return false;
    }

    public void GetTargetablePositions(Board board, Vector2Int origin) {
        m_TargetablePositions = new List<Vector2Int>();
        switch (m_TargetType) {
            case TargetType.Melee:
                board.AdjacentPositions(origin, m_Range, ref m_TargetablePositions);
                return;
            case TargetType.Self:
                m_TargetablePositions.Add(origin);
                return;
            default:
                return;
        }
    }

    public void DrawTargetablePositions(Board board, Vector2Int origin, bool redraw = true) {
        GetTargetablePositions(board, origin);
        List<Vector2Int> targettablePositions = m_TargetablePositions;

        if (m_Indicators != null) {
            for (int i = 0; i < m_Indicators.Count; i++) {
                SpriteRenderer oldIndicator = m_Indicators[i];
                Destroy(oldIndicator.gameObject);
            }
        }
        m_Indicators = new List<SpriteRenderer>();

        if (redraw) {
            for (int i = 0; i < targettablePositions.Count; i++) {
                SpriteRenderer indicator = new GameObject("Indicator", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
                indicator.sortingOrder = 1;
                indicator.transform.position = (Vector3)(Vector2)targettablePositions[i];
                indicator.color = new Color(1f, 1f, 1f, 0.25f);
                indicator.sprite = m_Indicator;
                m_Indicators.Add(indicator);
            }
        }
        
    }

    protected override void Draw() {
        if (m_Active) {
            Gizmos.color = Color.yellow;
            base.Draw();

            for (int i = 0; i < m_TargetablePositions.Count; i++) {
                Gizmos.DrawWireSphere((Vector3)(Vector2)m_TargetablePositions[i], 0.35f);
            }
        }

    }


}
