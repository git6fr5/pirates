using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Card {

    [SerializeField] private Effect m_ProjectileEffect;

    public override bool Effect(Board board, Vector2Int origin, Vector2Int target) {
        base.Effect(board, origin, target);

        Character character = board.GetAt<Character>(target);
        if (character == null) {
            Debug.Log("Nothing at targetted location.");
            return false; // Should cards use charges if they don't do anything?
        }

        float distance = (origin - target).magnitude;
        float maxDistance = Mathf.Sqrt(2) * m_Range;
        float actualSpeed = maxDistance / board.TurnDelay;
        float timeInterval = distance / actualSpeed;

        Effect newEffect = m_ProjectileEffect.Create(origin, timeInterval);
        newEffect.MoveTo(target, timeInterval);
        character.TakeDamage(m_Value, timeInterval);

        return true;
    }

}
