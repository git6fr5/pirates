/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Treasure : Piece {

    /* --- Variables --- */
    #region Variables

    // Rarity.
    [SerializeField] private Rarity m_Rarity;

    // Treasure.
    [SerializeField] private Card[] m_Cards;
    public Card[] Cards => m_Cards;

    // Activation.
    [SerializeField] private bool m_Active;
    public bool Active => m_Active;

    // UI.
    private bool m_MouseOver = false;
    private List<SpriteRenderer> m_VisionIndicators;
    [SerializeField] private TreasureUI m_TreasureUI;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    void Update() {
        SetUI();
        CheckActivate();
    }

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    #endregion

    #region Initialization

    private void Init() {

        m_Cards = new Card[3];
        if (m_Rarity == Rarity.Common) {
            m_Cards[0] = TreasurePool.GetRandomCard(Rarity.Common);
            m_Cards[1] = TreasurePool.GetRandomCard(Rarity.Common);
            m_Cards[2] = TreasurePool.GetRandomCard(Rarity.Common);
        }
        else if (m_Rarity == Rarity.Rare) {
            m_Cards[0] = TreasurePool.GetRandomCard(Rarity.Common);
            m_Cards[1] = TreasurePool.GetRandomCard(Rarity.Rare);
            m_Cards[2] = Random.Range(0f, 1f) < 0.5f ? TreasurePool.GetRandomCard(Rarity.Common) : TreasurePool.GetRandomCard(Rarity.Rare);
        }
        else {
            m_Cards[0] = TreasurePool.GetRandomCard(Rarity.Rare);
            m_Cards[1] = TreasurePool.GetRandomCard(Rarity.Legendary);
            m_Cards[2] = Random.Range(0f, 1f) < 0.5f ? TreasurePool.GetRandomCard(Rarity.Common) : TreasurePool.GetRandomCard(Rarity.Rare);
        }

    }

    #endregion

    #region Activation

    private void CheckActivate() {
        bool input0 = Input.GetMouseButtonDown(0);
        bool releaseInput0 = Input.GetMouseButtonUp(0);
        bool input1 = Input.GetMouseButtonDown(1);

        bool empty = true;
        for (int i = 0; i < m_Cards.Length; i++) {
            if (m_Cards[i] != null) {
                empty = false;
                break;
            }
        }

        if (m_MouseOver && input0) {
            Activate();
        }
        if (empty || (m_Active && input1)) {
            ClearUI();
            Destroy(gameObject);
        }
    }

    private void Activate() {
        Board board = Board.FindInstance();
        Player player = board.Get<Player>();
        float distance = Mathf.Max(Mathf.Abs(player.Position.x - m_Position.x), Mathf.Abs(player.Position.y - m_Position.y));
        if (distance <= 1) {
            m_Active = true;
        }
    }

    #endregion

    /* --- UI --- */
    #region UI

    private void SetUI() {
        m_TreasureUI.Refresh(this);

        bool playerHasActiveCard = PlayerHasActiveCard();
        BoardUI.DrawVisionUI(1, m_Board, m_Position, ref m_VisionIndicators, m_MouseOver && !playerHasActiveCard);
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

    private void ClearUI() {
        m_TreasureUI.Clear();
        Destroy(m_TreasureUI.gameObject);
    }

    #endregion


}
