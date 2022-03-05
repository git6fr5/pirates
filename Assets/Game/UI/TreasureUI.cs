using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureUI : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    [SerializeField] private CardUI m_TreasureCard;
    private CardUI[] m_TreasureCards;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        transform.position = Camera.main.transform.position;
    }

    #endregion

    /* --- Refreshing --- */
    #region Refreshing

    public void Refresh(Treasure treasure) {
        if (treasure == null || !treasure.Active) { return; }
        transform.SetParent(null);

        RefreshCardUI(treasure);
    }

    private void RefreshCardUI(Treasure treasure) {

        if (m_TreasureCards == null) {

            m_TreasureCards = new CardUI[treasure.Cards.Length];
            for (int i = 0; i < treasure.Cards.Length; i++) {
                CardUI newCardUI = m_TreasureCard.Create(treasure.Cards[i], i);
                m_TreasureCards[i] = newCardUI;
            }
        }

    }

    public void Clear() {

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
