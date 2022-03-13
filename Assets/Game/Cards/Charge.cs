using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : Card {

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Crocodile crocodile = board.GetAt<Crocodile>(origin);
        if (crocodile != null) {

            crocodile.SetPosition(target);
            // piece.Snap();

            Player player = board.GetAt<Player>(target);
            if (player != null) {
                player.TakeDamage(m_Value);
            }

            Debug.Log("Nothing at targetted location.");
            return true; // Should cards use charges if they don't do anything?
        }

        return false;

    }

}
