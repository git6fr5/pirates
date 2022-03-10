/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Treasure : Piece {

    /* --- Variables --- */
    #region Variables

    // References.
    [HideInInspector] private SpriteRenderer m_SpriteRenderer;

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

    // Jumping.
    [SerializeField, ReadOnly] private float m_BaseOffset;
    [SerializeField, ReadOnly] private bool m_Jump;
    [SerializeField] private float m_JumpDuration;
    [SerializeField] private float m_JumpHeight;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    protected override void Think() {
        float deltaTime = Time.deltaTime;
        SetPosition(deltaTime);
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

        print("Initializing treasure chest");

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

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        // m_BaseOffset = m_SpriteRenderer.material.GetVector("_Offset").y;
        StartCoroutine(IEJump());

    }

    #endregion

    #region Polish

    private void SetPosition(float deltaTime) {
        float targetY = m_BaseOffset;

        if (m_Jump) {
            targetY = m_JumpHeight + m_BaseOffset;
        }

        float currY = m_SpriteRenderer.material.GetVector("_Offset").y;
        if (currY > targetY + 0.05f || currY < targetY - 0.05f) {
            currY += Mathf.Sign(targetY - currY) * (m_JumpHeight / m_JumpDuration) * deltaTime;
        }
        else {
            currY = targetY;
        }

        m_SpriteRenderer.material.SetVector("_Offset", new Vector4(0f, currY, 0f, 0f));

        //if (m_Jump) {
        //    m_SpriteRenderer.material.SetVector("_Offset", new Vector4(Random.Range(-0.1f, 0.1f), currY, 0f, 0f));
        //}

    }

    private IEnumerator IEJump() {
        yield return new WaitForSeconds(Random.Range(7.5f, 10f));
        while (true) {
            m_Shake = true;
            yield return new WaitForSeconds(m_ShakeDuration);
            m_Jump = true;
            yield return new WaitForSeconds(m_JumpDuration);
            m_Jump = false;
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    #endregion

    #region Activation

    private void CheckActivate() {
        bool input0 = Input.GetMouseButtonDown(0);
        bool releaseInput0 = Input.GetMouseButtonUp(0);
        bool input1 = m_TreasureUI.Exit.Active || Input.GetMouseButtonDown(1);

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
