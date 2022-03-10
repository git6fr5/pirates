using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : Card {

    [SerializeField] private Effect m_SpawnEffect;

    public Piece m_SpawnPiece;

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Piece piece = board.GetAt<Piece>(target);
        if (piece == null) {

            // Effect newEffect = m_SpawnEffect.Create(target);

            board.AddPiece(m_SpawnPiece, target);

            Debug.Log("Nothing at targetted location.");
            return false; // Should cards use charges if they don't do anything?
        }

        return true;
    }

}
