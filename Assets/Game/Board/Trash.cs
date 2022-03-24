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

    public Sprite closedSprite;
    public Sprite openSprite;

    public SpriteRenderer spriteRenderer;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Update() {
        CheckActivate();
    }

    void OnMouseOver() {
        if (!m_MouseOver) {
            SoundController.PlaySound(SoundController.OpenTrash);
        }
        m_MouseOver = true;
        spriteRenderer.sprite = openSprite;
        spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        if (m_MouseOver) {
            SoundController.PlaySound(SoundController.CloseTrash);
        }
        m_MouseOver = false;
        spriteRenderer.sprite = closedSprite;
        spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
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
                SoundController.PlaySound(SoundController.UseTrash, 1);
                player.RemoveCard(i);
            }
        }
    }

    #endregion

    /* --- UI --- */
    #region UI


    #endregion


}
