using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Piece {

    // Action States.
    public enum Action {
        MoveRight,
        MoveUp,
        MoveLeft,
        MoveDown,
        CardSlot0,
        CardSlot1,
        CardSlot2,
        Pass,
        None
    }

    public int m_MaxHearts;
    [SerializeField] protected int m_Hearts;
    public int Hearts => m_Hearts;

    public int m_ActionsTaken;
    public int ActionsTaken => m_ActionsTaken;

    public int m_ActionsPerTurn;
    public int ActionsPerTurn => m_ActionsPerTurn;

    public bool m_PerformingAction;

    private bool m_CompletedTurn;
    public bool CompletedTurn => m_CompletedTurn;

    [SerializeField] private int m_CardSlots;
    public int CardSlots => m_CardSlots;

    public Card[] m_Cards;
    public Card[] Cards => m_Cards;

    [SerializeField] private bool m_IsStatic;
    public bool IsStatic => m_IsStatic;

    void Start() {
        m_Hearts = m_MaxHearts;

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

    protected override void Think() {
        CheckDeath();
        SetUI();

        if (!m_CompletedTurn) {
            TakeTurn();
        }
    }

    /* --- Turn --- */
    public void NewTurn() {
        m_ActionsTaken = 0;
        m_CompletedTurn = false;
        m_PerformingAction = false;
    }

    private void TakeTurn() {
        if (m_ActionsTaken >= m_ActionsPerTurn) {
            m_CompletedTurn = true;
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
            m_CompletedTurn = true;
            return;
        }

        int actionIndex = (int)action;
        if (actionIndex < 4) {
            Move(actionIndex);
        }
        else if (actionIndex >= 4 && actionIndex < 7) {
            ActivateCard(actionIndex - 4);
        }

    }

    /* --- Decision --- */
    protected virtual Action GetAction() {
        return Action.None;
    }

    protected virtual Vector2Int? GetTarget(Card card) {
        return null;
    }

    protected bool CheckMove(Action action) {
        if ((int)action >= 4) { return false; }

        Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, 90f * (int)action) * Vector2.right).normalized;
        return m_Board.CheckMove(m_Position, new Vector2Int((int)direction.x, (int)direction.y));
    }

    protected Action MoveTowards(Piece target) {
        List<Vector2Int> path = m_Board.ManhattanPath(m_Position, target.Position);
        if (path.Count < 1) { return Action.None; }

        Vector2Int direction = path[1] - m_Position;
        if (direction.x > 0) {
            return Action.MoveRight;
        }
        if (direction.y > 0) {
            return Action.MoveUp;
        }
        if (direction.x < 0) {
            return Action.MoveLeft;
        }
        if (direction.y < 0) {
            return Action.MoveDown;
        }
        return Action.Pass;
    }

    /* --- Execution --- */
    private void Move(int index) {
        Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, 90f * index) * Vector2.right).normalized;
        bool tookAction = m_Board.Move(this, new Vector2Int((int)direction.x, (int)direction.y));
        if (tookAction) {
            m_ActionsTaken += 1;
            float duration = m_Board.TurnDelay;
            PerformAction(duration);
        }
    }

    private void PerformAction(float duration) {
        m_PerformingAction = true;
        StartCoroutine(IEPerformingAction(duration));
    }

    private IEnumerator IEPerformingAction(float duration) {
        yield return new WaitForSeconds(duration);
        Snap();
        m_PerformingAction = false;
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
                    if (m_Cards[i].Effect(m_Board, intTarget)) {
                        m_Cards[i].UseCharge();
                        if (m_Cards[i].Charges <= 0) {
                            RemoveCard(i);
                        }
                        m_ActionsTaken += 1;
                        float duration = m_Board.TurnDelay;
                        PerformAction(duration);
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

    /* --- Damage --- */

    public void TakeDamage(int damage) {
        m_Hearts -= damage;
        CameraShake.ActivateShake(m_Board.TurnDelay);
    }

    public void Heal(int health) {
        m_Hearts += health;
        if (m_Hearts > m_MaxHearts) {
            m_Hearts = m_MaxHearts;
        }
    }

    private void CheckDeath() {
        if (m_Hearts <= 0) {
            ClearUI();
            Destroy(gameObject);
        }
    }

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
