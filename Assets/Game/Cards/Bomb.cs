using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : AOE {

    public override bool AOEEffect(Board board, Vector2Int target, float delay) {
        base.AOEEffect(board, target, delay);

        Piece piece = board.GetAt<Piece>(target);
        Debug.Log("Doing damage");
        if (piece != null) {
            piece.TakeDamage(m_Value);

            Character character = piece.GetComponent<Character>();
            if (character != null && m_StatusEffect != Status.None && m_Duration > 0) {
                character.ApplyStatus(m_StatusEffect, m_Duration);
            }

            float duration = board.TurnDelay;
            piece.StartShake(delay, duration);

        }

        return true;
    }

}
