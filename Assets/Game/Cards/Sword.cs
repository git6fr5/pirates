using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Card {

    [SerializeField] private Effect m_SlashEffect;

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        // base.Effect(board, origin, target);

        Piece piece = board.GetAt<Piece>(target);
        if (piece == null || piece.GetComponent<Treasure>() != null) {
            Debug.Log("Nothing at targetted location.");
            return false; // Should cards use charges if they don't do anything?
        }

        if (m_ActivationEffect != null) {
            m_ActivationEffect.Create(origin);
        }
        if (m_ActivationSound != null) {
            SoundController.PlaySound(m_ActivationSound, 1);
        }

        Effect newEffect = m_SlashEffect.Create(target);

        Debug.Log("Doing damage");
        piece.TakeDamage(m_Value);

        Character character = piece.GetComponent<Character>();
        if (character != null && m_StatusEffect != Status.None && m_Duration > 0) {
            character.ApplyStatus(m_StatusEffect, m_Duration);
        }

        float duration = board.TurnDelay;
        piece.StartShake(0f, duration);

        return true;
    }

}
