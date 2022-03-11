using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureUI : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    [SerializeField] private CardUI m_TreasureCard;
    private CardUI[] m_TreasureCards;

    [SerializeField] private TreasureUIExit m_Exit;
    public TreasureUIExit Exit => m_Exit;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
    }

    #endregion

    /* --- Refreshing --- */
    #region Refreshing

    public void Refresh(Treasure treasure) {
        transform.position = (Vector2)Camera.main.transform.position;

        if (treasure == null || !treasure.Active) { return; }
        transform.SetParent(null);

        RefreshCardUI(treasure);
        m_Exit.gameObject.SetActive(true);
    }

    private void RefreshCardUI(Treasure treasure) {

        if (m_TreasureCards == null) {

            m_TreasureCards = new CardUI[treasure.Cards.Length];
            for (int i = 0; i < treasure.Cards.Length; i++) {
                CardUI newCardUI = m_TreasureCard.Create(treasure.Cards[i], i);
                if (newCardUI.GetComponent<TreasureCardUI>() != null) {
                    newCardUI.GetComponent<TreasureCardUI>().SetIndex(i);
                }
                m_TreasureCards[i] = newCardUI;
            }
        }

    }

    public void Clear() {

        m_Exit.gameObject.SetActive(false);

        if (m_TreasureCards != null) {
            for (int i = 0; i < m_TreasureCards.Length; i++) {
                CardUI cardUI = m_TreasureCards[i];
                if (cardUI != null) {
                    Destroy(cardUI.CardTargetType.gameObject);
                    Destroy(cardUI.gameObject);
                }
            }
        }

    }

    #endregion

}
