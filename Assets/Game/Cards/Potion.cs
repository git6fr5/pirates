using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Card {


    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Character character = board.GetAt<Character>(target);
        if (character == null) {
            return false; // Should cards use charges if they don't do anything?
        }

        character.Heal(m_Value);
        return true;
    }


}
