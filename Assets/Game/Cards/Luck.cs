using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luck : Card {

    public Rarity m_Rarity;
    public int m_NewCardCharges;

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Character character = board.GetAt<Character>(target);
        if (character == null) {
            return false; // Should cards use charges if they don't do anything?
        }

        if (m_NewCardCharges > 0) {
            Card newCard = TreasurePool.GetRandomCardWithCharges(m_Rarity, 1);
            character.ReplaceCard(this, newCard);
        }
        else {
            Card newCard = TreasurePool.GetRandomCard(m_Rarity);
            character.ReplaceCard(this, newCard);
        }

        return true;
    }

}
