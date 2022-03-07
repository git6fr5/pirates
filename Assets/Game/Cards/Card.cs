/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Card : MonoBehaviour { // Is there any reason for this to be derived from a piece? I guess.

    /* --- Variables --- */
    #region Variables

    [Header("Settings")]
    // Name.
    [SerializeField] private string m_CardName;
    public string CardName => m_CardName;
    // Rarity.
    [SerializeField] private Rarity m_Rariry;
    public Rarity CardRarity => m_Rariry;
    // Target Type.
    [SerializeField] private TargetType m_TargetType;
    public TargetType CardTargetType => m_TargetType;
    // Range.
    [SerializeField] protected int m_Range;
    public int Range => m_Range;
    // Values.
    [SerializeField] protected int m_Value;
    public int Value => m_Value;
    // Status.
    [SerializeField] protected Status m_StatusEffect;
    public Status StatusEffect => m_StatusEffect;
    [SerializeField] protected int m_Duration;
    public int DurationValue => m_Duration;
    // Charges.
    [SerializeField] private int m_Charges;
    public int Charges => m_Charges;
    // Face.
    [SerializeField] private Sprite m_Face;
    public Sprite Face => m_Face;

    [Header("Activation")]
    // Activation.
    [SerializeField, ReadOnly] protected bool m_Active;
    public bool Active => m_Active;
    // Activation Effect
    [SerializeField] private Effect m_ActivationEffect;

    [Header("Targetting")]
    // Indicator.
    // [SerializeField] private Sprite m_SquareIndicator;
    private List<SpriteRenderer> m_TargetIndicators;
    protected List<Vector2Int> m_TargetablePositions;

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

    public void AddCharges(int charges) {
        m_Charges = m_Charges + charges;
    }

    public void SetCharges(int charges) {
        m_Charges = charges;
    }

    public void UseCharge() {
        m_Active = false;
        m_Charges = m_Charges - 1;
    }

    public virtual bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        if (m_ActivationEffect != null) {
            m_ActivationEffect.Create(origin);
        }
        return false;
    }

    #endregion

    /* --- Targetting --- */
    #region Targetting

    public List<Vector2Int> GetTargetablePositions(Board board, Vector2Int origin) {
        List<Vector2Int> targetablePositions = new List<Vector2Int>();
        switch (m_TargetType) {
            case TargetType.AOE:
                targetablePositions = board.AOETargetting(origin, m_Range, ref targetablePositions);
                break;
            case TargetType.Directional:
                targetablePositions = board.DirectionalTargetting(origin, m_Range, ref targetablePositions);
                break;
            case TargetType.Melee:
                targetablePositions = board.MeleeTargetting(origin, m_Range, ref targetablePositions);
                break;
            case TargetType.Self:
                targetablePositions.Add(origin);
                break;
            default:
                break;
        }
        m_TargetablePositions = targetablePositions;
        return targetablePositions;
    }

    #endregion

}
