using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

public class Player : Character {

    private Vector2 m_MousePosition;
    private bool m_ActivateInput;
    private bool m_DeactivateInput;

    public float m_NodeRadius = 0.25f;

    protected override Action GetAction() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            return Action.CardSlot0;
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            return Action.CardSlot1;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            return Action.MoveUp;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            return Action.MoveLeft;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            return Action.MoveDown;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            return Action.MoveRight;
        }
        return Action.None;
    }

    protected override Vector2Int? GetTarget(Card card) {
        List<Vector2Int> targettablePositions = card.TargetablePositions;

        m_MousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_ActivateInput = Input.GetMouseButtonUp(0);
        m_DeactivateInput = Input.GetMouseButtonDown(1);

        if (m_DeactivateInput || targettablePositions == null || targettablePositions.Count == 0) {
            card.Deactivate();
            return null;
        }

        if (m_ActivateInput) {
            for (int i = 0; i < targettablePositions.Count; i++) {
                if (IsMouseOver(targettablePositions[i])) {
                    return (Vector2Int?)targettablePositions[i];
                }
            }
        }
        return null;
    }

    private bool IsMouseOver(Vector2Int position) {
        return (m_MousePosition - (Vector2)position).magnitude < m_NodeRadius;
    }

    protected override void Draw() {
        base.Draw();

        float halfHearts = m_MaxHearts % 2 == 0 ? (float)(m_MaxHearts / 2f) - 0.5f : (float)(m_MaxHearts / 2f);
        for (int i = 0; i < m_MaxHearts; i++) {
            if (i < m_Hearts) {
                Gizmos.DrawWireSphere(transform.position - Vector3.up * 0.5f + Vector3.right * 0.4f * (-halfHearts + i), 0.2f);
            }
        }

    }

    /* --- Interacting --- */


}
