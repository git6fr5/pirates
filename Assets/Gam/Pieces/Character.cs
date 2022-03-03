using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Piece {

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

    // private int m_ActionsPerTurn;

    private bool m_CompletedTurn;
    public bool CompletedTurn => m_CompletedTurn;

    void Start() {
    }

    void Update() {
        if (!CompletedTurn) {
            TakeTurn();
        }
    }

    void TakeTurn() {
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
            Card(actionIndex - 4);
        }

    }

    protected bool CheckMove(Action action) {
        int index = (int)action;
        if (index >= 4) {
            return false;
        }

        Vector2 _direction = (Vector2)(Quaternion.Euler(0f, 0f, 90f * index) * Vector2.right).normalized;
        Vector2Int direction = new Vector2Int((int)_direction.x, (int)_direction.y);
        return m_Section.CheckMove(m_Position, direction);
    }

    void Move(int index) {
        Vector2 floatDirection = (Vector2)(Quaternion.Euler(0f, 0f, 90f * index) * Vector2.right).normalized;
        Vector2Int direction = new Vector2Int((int)floatDirection.x, (int)floatDirection.y);
        m_CompletedTurn = m_Section.Move(this, direction);
    }

    public Action MoveTowards(Piece piece) {
        List<Vector2Int> path = m_Section.ManhattanPath(m_Position, piece.Position);

        if (path.Count < 1) { return Action.None; }
        Vector2Int direction = path[1] - Position;
        if (direction.x > 0) {
            return Action.MoveLeft;
        }
        if (direction.y > 0) {
            return Action.MoveUp;
        }
        if (direction.x < 0) {
            return Action.MoveRight;
        }
        if (direction.y < 0) {
            return Action.MoveDown;
        }
        return Action.None;
    }

    void Card(int index) {
        m_CompletedTurn = true;
    }

    protected virtual Action GetAction() {
        return Action.None;
    }

    public void NewTurn() {
        m_CompletedTurn = false;
    }

}
