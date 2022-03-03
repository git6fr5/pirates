using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

public class Player : Character {

    protected override Action GetAction() {
        if (Input.GetKeyDown(KeyCode.W)) {
            return Action.MoveUp;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            return Action.MoveLeft;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            return Action.MoveDown;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            return Action.MoveRight;
        }
        return Action.None;
    }
}
