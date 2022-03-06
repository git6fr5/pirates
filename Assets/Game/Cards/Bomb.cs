using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : AOE {

    public override bool AOEEffect(Board board, Vector2Int target) {
        base.AOEEffect(board, target);

        Character character = board.GetAt<Character>(target);
        if (character == null) {
            Debug.Log("Nothing at targetted location.");
            return false; // Should cards use charges if they don't do anything?
        }

        Debug.Log("Doing damage");
        character.TakeDamage(m_Value);
        return true;
    }

}
