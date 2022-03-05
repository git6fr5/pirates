/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Card : Piece {

    /* --- Variables --- */
    #region Variables

    [Header("Settings")]
    // Name.
    [SerializeField] private string m_CardName;
    public string CardName => m_CardName;
    // Target Type.
    [SerializeField] private TargetType m_TargetType;
    public TargetType CardTargetType => m_TargetType;
    // Range.
    [SerializeField] private int m_Range;
    public int Range => m_Range;
    // Value.
    [SerializeField] protected int m_Value;
    public int Value => m_Value;
    // Charges.
    [SerializeField] private int m_Charges;
    public int Charges => m_Charges;
    // Face.
    [SerializeField] private Sprite m_Face;
    public Sprite Face => m_Face;

    [Header("Activation")]
    // Activation.
    [SerializeField, ReadOnly] private bool m_Active;
    public bool Active => m_Active;
    // Activation Effect
    [SerializeField] private Sprite m_ActivationEffect;

    [Header("Targetting")]
    // Indicator.
    [SerializeField] private Sprite m_SquareIndicator;
    private List<SpriteRenderer> m_TargetIndicators;

    #endregion

    /* --- Activation --- */
    #region Activation

    public void Activate(Board board, Vector2Int origin) {
        m_Active = true;
        GetTargetablePositions(board, origin);
    }

    public void Deactivate() {
        m_Active = false;
    }

    #endregion

    /* --- Use --- */
    #region Use

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
        effect.sprite = m_ActivationEffect;
        Destroy(effect.gameObject, board.TurnDelay);
        return false;
    }

    #endregion

    /* --- Targetting --- */
    #region Targetting

    public List<Vector2Int> GetTargetablePositions(Board board, Vector2Int origin) {
        List<Vector2Int> targetablePositions = new List<Vector2Int>();
        switch (m_TargetType) {
            case TargetType.Melee:
                targetablePositions = board.AdjacentPositions(origin, m_Range, ref targetablePositions);
                break;
            case TargetType.Self:
                targetablePositions.Add(origin);
                break;
            default:
                break;
        }
        return targetablePositions;
    }

    #endregion

}
