using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

public class Dummy : Character {

    [SerializeField] private Action m_Action;

    protected override Action GetAction() {
        if (CheckMove(m_Action)) {
            return m_Action;
        }
        return Action.Pass;
    }

}
