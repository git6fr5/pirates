using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

public class Zombie : Character {

    [SerializeField] private int m_VisisonDistance;
    [SerializeField] private bool m_PlayerInVision;

    protected override Action GetAction() {
        Player player = (Player)Game.Get<Player>();
        m_PlayerInVision = m_Section.WithinRadius(this, player, m_VisisonDistance);
        if (m_PlayerInVision) {
            Action action = MoveTowards(player);
            if (action != Action.None) {
                return action;
            }
            print(action);
        }
        return Action.Pass;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Vector3 origin = transform.position;

        Vector3 displacementA = new Vector3(-m_VisisonDistance, -m_VisisonDistance, 0);
        Vector3 displacementB = new Vector3(m_VisisonDistance, -m_VisisonDistance, 0);
        Gizmos.DrawLine(origin + displacementA, origin + displacementB);

        displacementB = new Vector3(-m_VisisonDistance, m_VisisonDistance, 0);
        Gizmos.DrawLine(origin + displacementA, origin + displacementB);

        displacementA = new Vector3(m_VisisonDistance, m_VisisonDistance, 0);
        displacementB = new Vector3(-m_VisisonDistance, m_VisisonDistance, 0);
        Gizmos.DrawLine(origin + displacementA, origin + displacementB);

        displacementB = new Vector3(m_VisisonDistance, -m_VisisonDistance, 0);
        Gizmos.DrawLine(origin + displacementA, origin + displacementB);
    }

}
