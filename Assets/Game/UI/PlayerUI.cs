using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    [SerializeField] private CardUI m_PlayerCard;
    private CardUI[] m_PlayerCards;

    [SerializeField] private SpriteRenderer m_PlayerHeart;
    [SerializeField] private SpriteRenderer m_EmptyHeart;
    private List<SpriteRenderer> m_PlayerHearts;

    [SerializeField] private SpriteRenderer m_PlayerAction;
    [SerializeField] private SpriteRenderer m_EmptyAction;
    private List<SpriteRenderer> m_PlayerActions;

    [SerializeField] private SpriteRenderer m_TargetSquare;

    public Material uiMat;

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
                CardUI newCardUI = m_PlayerCard.Create(player.Cards[i], i, player.Cards.Length);
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
        for (int i = 0; i < player.MaxHearts; i++) {
            if (i >= player.MaxHearts - player.Hearts) {
                SpriteRenderer newHeart = Instantiate(m_PlayerHeart.gameObject, m_PlayerHeart.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                newHeart.transform.position += i * 1.125f * Vector3.down +bobAmp * Mathf.Sin(Mathf.PI * (Board.Ticks * bobSpeed + (float)i / 6)) * Vector3.up;
                newHeart.material = uiMat;
                newHeart.gameObject.SetActive(true);
                m_PlayerHearts.Add(newHeart);
            }
            else {
                SpriteRenderer newHeart = Instantiate(m_EmptyHeart.gameObject, m_PlayerHeart.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                newHeart.transform.position += i * 1.125f * Vector3.down + bobAmp * Mathf.Sin(Mathf.PI * (Board.Ticks * bobSpeed + (float)i / 6)) * Vector3.up;
                newHeart.material = uiMat;
                newHeart.gameObject.SetActive(true);
                m_PlayerHearts.Add(newHeart);
            }
            
        }

    }

    private float bobSpeed = 1.25f;
    private float bobAmp = 1.5f / 16f;

    private void RefreshEnergyUI(Player player) {

        if (m_PlayerActions != null) {
            for (int i = 0; i < m_PlayerActions.Count; i++) {
                SpriteRenderer renderer = m_PlayerActions[i];
                Destroy(renderer.gameObject);
            }
        }

        m_PlayerActions = new List<SpriteRenderer>();
        int actions = player.ActionsPerTurn + (player.Angry ? 1 : 0);
        int energy = actions - player.ActionsTaken;
        for (int i = 0; i < actions; i++) {
            if (i >= actions - energy) {
                SpriteRenderer newEnergy = Instantiate(m_PlayerAction.gameObject, m_PlayerAction.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                newEnergy.transform.position += i * 1.125f * Vector3.down + bobAmp * Mathf.Sin(Mathf.PI * (Board.Ticks * bobSpeed + (float)i / 6)) * Vector3.up;
                if (player.Angry && i == 0) {
                    newEnergy.color = Color.red;
                }
                newEnergy.gameObject.SetActive(true);
                m_PlayerActions.Add(newEnergy);
            }
            else {
                SpriteRenderer newEnergy = Instantiate(m_EmptyAction.gameObject, m_PlayerAction.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
                newEnergy.transform.position += i * 1.125f * Vector3.down + bobAmp * Mathf.Sin(Mathf.PI * (Board.Ticks * bobSpeed + (float)i / 6)) * Vector3.up;
                newEnergy.gameObject.SetActive(true);
                m_PlayerActions.Add(newEnergy);
            }
        }

    }

    #endregion

}
