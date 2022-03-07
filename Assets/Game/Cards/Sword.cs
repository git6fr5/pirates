using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Card {

    [SerializeField] private Effect m_SlashEffect;

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Character character = board.GetAt<Character>(target);
        if (character == null) {
            Debug.Log("Nothing at targetted location.");
            return false; // Should cards use charges if they don't do anything?
        }

        Effect newEffect = m_SlashEffect.Create(target, board.TurnDelay);

        Debug.Log("Doing damage");
        character.TakeDamage(m_Value);
        return true;
    }

}
