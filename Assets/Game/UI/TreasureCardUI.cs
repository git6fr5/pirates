using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCardUI : CardUI {

    protected override void Activate() {
        Board board = Board.FindInstance();
        Player player = board.Get<Player>();
        bool success = player.AddCard(m_Card);
        if (success) {
            Destroy(gameObject);
        }
    }

    protected override void Deactivate() {

    }

    protected override void DrawTarget() {


    }

}
