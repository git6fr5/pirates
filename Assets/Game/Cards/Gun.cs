using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Card {

    [SerializeField] private Effect m_ProjectileEffect;

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Piece piece = board.GetAt<Piece>(target);
        if (piece == null) {
            Debug.Log("Nothing at targetted location.");
            return false; // Should cards use charges if they don't do anything?
        }

        float distance = (origin - target).magnitude;
        float maxDistance = Mathf.Sqrt(2) * m_Range;
        float actualSpeed = maxDistance / board.TurnDelay;
        float delay = distance / actualSpeed;

        Effect newEffect = m_ProjectileEffect.Create(origin, delay);
        newEffect.MoveTo(target, delay);

        piece.TakeDamage(m_Value, delay);

        Character character = piece.GetComponent<Character>();
        if (character != null && m_StatusEffect != Status.None && m_Duration > 0) {
            character.ApplyStatus(m_StatusEffect, m_Duration);
        }

        return true;
    }

}
