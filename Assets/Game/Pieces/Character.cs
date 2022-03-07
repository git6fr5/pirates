using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Piece {

    [System.Serializable]
    public class StatusModifier {
        public Status Modifier;
        public int Duration;
        public StatusModifier(Status modifier, int duration) {
            Modifier = modifier;
            Duration = duration;
        }
    }

    // Turn.
    [SerializeField, ReadOnly] private bool m_CompletedTurn;
    public bool CompletedTurn => m_CompletedTurn;

    // Actions.
    [SerializeField, ReadOnly] private bool m_PerformingAction;
    [SerializeField, ReadOnly] private Action m_CurrAction;
    public Action CurrAction => m_CurrAction;
    [SerializeField, ReadOnly] protected int m_ActionsTaken;
    public int ActionsTaken => m_ActionsTaken;
    [SerializeField] private int m_ActionsPerTurn;
    public int ActionsPerTurn => m_ActionsPerTurn;

    // Cards.
    [SerializeField] protected int m_CardSlots;
    public int CardSlots => m_CardSlots;
    [SerializeField] protected Card[] m_Cards;
    public Card[] Cards => m_Cards;

    // Status.
    [SerializeField, ReadOnly] private List<StatusModifier> m_StatusModifiers;
    [SerializeField, ReadOnly] private bool m_Hexed;
    [SerializeField, ReadOnly] private bool m_Burning;
    [SerializeField, ReadOnly] private bool m_Paralyzed;
    public bool Hexed => m_Hexed;
    public bool Burning => m_Burning;
    public bool Paralyzed => m_Paralyzed;

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        m_CurrAction = Action.None;

        Card[] cards = new Card[m_CardSlots];
        if (cards.Length > m_Cards.Length) {
            m_Cards.CopyTo(cards, 0);
            m_Cards = cards;
        }
        else {
            cards = m_Cards;
            m_Cards = new Card[m_CardSlots];
            for (int i = 0; i < m_Cards.Length; i++) {
                m_Cards[i] = cards[i];
            }
        }
    }

    #endregion

    /* --- Thinking --- */
    #region Thinking

    protected override void Think() {
        if (m_Hearts <= 0) {
            m_CompletedTurn = true;
            return;
        }
        CheckStatus();

        SetUI();
        if (!m_CompletedTurn) {
            TakeTurn();
        }
    }

    #endregion

    /* --- Turn --- */
    #region Turn

    public void NewTurn() {
        m_ActionsTaken = 0;
        m_CompletedTurn = false;
        m_PerformingAction = false;
        if (m_Hearts > 0 && m_Burning) {
            TakeDamage(1);
        }
    }

    private void TakeTurn() {
        if (m_ActionsTaken >= m_ActionsPerTurn) {
            CompleteTurn();
            return;
        }

        if (m_PerformingAction) {
            return;
        }

        bool usedCard = UseCard();
        if (usedCard) {
            return;
        }

        Action action = GetAction();
        if (action == Action.None) {
            return;
        }
        if (action == Action.Pass) {
            m_ActionsTaken += 1;
            return;
        }

        int actionIndex = (int)action;
        if (actionIndex < 4) {
            if (m_Paralyzed) {
                m_ActionsTaken += 1;
            }
            else {
                Move(actionIndex);
            }
        }
        else if (actionIndex >= 4 && actionIndex < 7) {
            if (m_Hexed) {
                m_ActionsTaken += 1;
            }
            else {
                ActivateCard(actionIndex - 4);
            }
        }

    }

    private void CompleteTurn() {
        IncrementStatusModifiers();
        m_CompletedTurn = true;
    }

    /* --- Action --- */
    #region Action

    protected virtual Action GetAction() {
        return Action.None;
    }

    private void PerformAction(int index, float duration) {
        m_CurrAction = (Action)index;
        m_PerformingAction = true;
        StartCoroutine(IEPerformingAction(duration));
    }

    private IEnumerator IEPerformingAction(float duration) {
        m_PerformingAction = WaitForEndOfAction();
        yield return new WaitForSeconds(duration);
        Snap();
        m_CurrAction = Action.None;
        m_PerformingAction = false;
    }

    protected virtual bool WaitForEndOfAction() {
        return true;
    }

    #endregion

    /* --- Movement --- */
    private void Move(int index) {
        Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, 90f * index) * Vector2.right).normalized;
        bool tookAction = m_Board.Move(this, new Vector2Int((int)direction.x, (int)direction.y));
        if (tookAction) {
            m_ActionsTaken += 1;
            float duration = m_Board.TurnDelay;
            PerformAction(index, duration);
        }
    }


    protected bool CheckMove(Action action) {
        if ((int)action >= 4) { return false; }

        Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, 90f * (int)action) * Vector2.right).normalized;
        return m_Board.CheckMove(m_Position, new Vector2Int((int)direction.x, (int)direction.y));
    }

    #endregion

    /* --- Card --- */
    #region Card

    protected virtual Vector2Int? GetTarget(Card card) {
        return null;
    }

    private void ActivateCard(int index) {
        if (index >= m_Cards.Length) {
            return;
        }
        m_Cards[index].Activate(m_Board, m_Position);
    }

    private bool UseCard() {
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
                        PerformAction(4 + i, duration);
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

    public virtual void ReplaceCard(Card oldCard, Card newCard) {
        for (int i = 0; i < m_Cards.Length; i++) {
            if (m_Cards[i] == oldCard) {
                print("replacing card");
                m_Cards[i] = newCard;
            }
        }
        Destroy(oldCard.gameObject);
    }

    public virtual void RemoveCard(int index) {
        Card card = m_Cards[index];
        for (int i = index + 1; i < m_Cards.Length; i++) {
            m_Cards[i - 1] = m_Cards[i];
        }
        m_Cards[m_Cards.Length - 1] = null;
        Destroy(card.gameObject);
    }

    public virtual bool AddCard(Card card) {
        for (int i = 0; i < m_Cards.Length; i++) {
            if (m_Cards[i] == null) {
                Card newCard = Instantiate(card.gameObject).GetComponent<Card>();
                m_Cards[i] = newCard;
                newCard.transform.SetParent(transform);
                Destroy(card.gameObject);
                return true;
            }
        }
        return false;
    }

    #endregion

    /* --- Status --- */
    #region Status

    public bool ApplyStatus(Status status, int duration) {
        if (m_StatusModifiers.Find(statusModifier => statusModifier.Modifier == status) == null) {
            m_StatusModifiers.Add(new StatusModifier(status, duration));
            return true;
        }
        return false;
    }

    public void CheckStatus() {
        m_Hexed = m_StatusModifiers.Find(statusModifier => statusModifier.Modifier == Status.Hexed) != null;
        m_Burning = m_StatusModifiers.Find(statusModifier => statusModifier.Modifier == Status.Burning) != null;
        m_Paralyzed = m_StatusModifiers.Find(statusModifier => statusModifier.Modifier == Status.Paralyzed) != null;
    }

    private void IncrementStatusModifiers() {
        List<StatusModifier> temp = new List<StatusModifier>();
        for (int i = 0; i < m_StatusModifiers.Count; i++) {
            m_StatusModifiers[i].Duration -= 1;
            if (m_StatusModifiers[i].Duration <= 0) {
                temp.Add(m_StatusModifiers[i]);
            }
        }
        for (int i = 0; i < temp.Count; i++) {
            m_StatusModifiers.Remove(temp[i]);
        }
    }

    #endregion

    /* --- UI --- */
    void OnDestroy() {
        ClearUI();
    }

    protected virtual void SetUI() {
        //
    }

    protected virtual void ClearUI() {

    }

}
