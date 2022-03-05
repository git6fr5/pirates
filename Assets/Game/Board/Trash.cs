/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Trash : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // UI.
    private bool m_MouseOver = false;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Update() {
        CheckActivate();
    }

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    #endregion

    #region Activation

    private void CheckActivate() {
        bool input0 = Input.GetMouseButtonUp(0);

        if (m_MouseOver && input0) {
            Activate();
        }
        
    }

    private void Activate() {
        Board board = Board.FindInstance();
        Player player = board.Get<Player>();
        for (int i = 0; i < player.Cards.Length; i++) {
            if (player.Cards[i] != null && player.Cards[i].Active) {
                player.RemoveCard(i);
            }
        }
    }

    #endregion

    /* --- UI --- */
    #region UI


    #endregion


}
