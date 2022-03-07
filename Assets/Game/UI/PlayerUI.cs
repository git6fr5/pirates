using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    [SerializeField] private CardUI m_PlayerCard;
    private CardUI[] m_PlayerCards;

    [SerializeField] private SpriteRenderer m_PlayerHeart;
    private List<SpriteRenderer> m_PlayerHearts;

    [SerializeField] private SpriteRenderer m_PlayerAction;
    private List<SpriteRenderer> m_PlayerActions;

    [SerializeField] private SpriteRenderer m_TargetSquare;

    #endregion

    /* --- Refreshing --- */
    #region Refreshing

    public void Refresh(Player player, Board board) {
        if (player == null || board == null) { return; }
        transform.SetParent(null);

        RefreshTargetUI(player, board);
        RefreshCardUI(player);
        RefreshHealthUI(player);
        RefreshEnergyUI(player);
    }

    private void RefreshTargetUI(Player player, Board board) {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector3 snappedPosition = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y), 0f);
        if (!player.CompletedTurn && mousePosition.x >= 0 && mousePosition.x < board.Width && mousePosition.y >= 0 && mousePosition.y < board.Height) {
            m_TargetSquare.gameObject.SetActive(true);
            m_TargetSquare.transform.position = snappedPosition;
        }
        else {
            m_TargetSquare.gameObject.SetActive(false);
        }
    }

    private void RefreshCardUI(Player player) {

        if (m_PlayerCards == null) {

            m_PlayerCards = new CardUI[player.Cards.Length];
            for (int i = 0; i < player.Cards.Length; i++) {
                CardUI newCardUI = m_PlayerCard.Create(player.Cards[i], i);
                m_PlayerCards[i] = newCardUI;
            }
        }

        // While not the players turn, deactivate the card ui interactability.
        if (player.CompletedTurn) {
            for (int i = 0; i < m_PlayerCards.Length; i++) {
                m_PlayerCards[i].gameObject.SetActive(false);
            }
        }
        else {
            for (int i = 0; i < m_PlayerCards.Length; i++) {
                m_PlayerCards[i].gameObject.SetActive(true);
            }
        }

    }

    public void ResetCards(Player player, bool redraw = true) {

        if (m_PlayerCards != null) {
            for (int i = 0; i < m_PlayerCards.Length; i++) {
                CardUI cardUI = m_PlayerCards[i];
                if (cardUI != null) {
                    Destroy(cardUI.CardTargetType.gameObject);
                    Destroy(cardUI.gameObject);
                }
            }
        }

        if (redraw) {
            m_PlayerCards = new CardUI[player.Cards.Length];
            for (int i = 0; i < player.Cards.Length; i++) {
                CardUI newCardUI = m_PlayerCard.Create(player.Cards[i], i);
                m_PlayerCards[i] = newCardUI;
            }
        }

    }

    private void RefreshHealthUI(Player player) {

        if (m_PlayerHearts != null) {
            for (int i = 0; i < m_PlayerHearts.Count; i++) {
                SpriteRenderer renderer = m_PlayerHearts[i];
                Destroy(renderer.gameObject);
            }
        }

        m_PlayerHearts = new List<SpriteRenderer>();
        for (int i = 0; i < player.Hearts; i++) {
            SpriteRenderer newHeart = Instantiate(m_PlayerHeart.gameObject, m_PlayerHeart.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            newHeart.transform.position += i * 1f * Vector3.right;
            newHeart.gameObject.SetActive(true);
            m_PlayerHearts.Add(newHeart);
        }

    }

    private void RefreshEnergyUI(Player player) {

        if (m_PlayerActions != null) {
            for (int i = 0; i < m_PlayerActions.Count; i++) {
                SpriteRenderer renderer = m_PlayerActions[i];
                Destroy(renderer.gameObject);
            }
        }

        m_PlayerActions = new List<SpriteRenderer>();
        int energy = player.ActionsPerTurn - player.ActionsTaken;
        for (int i = 0; i < energy; i++) {
            SpriteRenderer newEnergy = Instantiate(m_PlayerAction.gameObject, m_PlayerAction.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            newEnergy.transform.position += i * 1f * Vector3.right;
            newEnergy.gameObject.SetActive(true);
            m_PlayerActions.Add(newEnergy);
        }

    }

    #endregion

}
