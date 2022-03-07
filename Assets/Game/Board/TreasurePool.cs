using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePool : MonoBehaviour
{

    // Treasure Pool.
    [SerializeField] private Card[] m_AllCards;
    [SerializeField] private static List<Card> CommonCards;
    [SerializeField] private static List<Card> RareCards;
    [SerializeField] private static List<Card> LegendaryCards;

    void Start() {
        CommonCards = new List<Card>();
        RareCards = new List<Card>();
        LegendaryCards = new List<Card>();

        for (int i = 0; i < m_AllCards.Length; i++) {
            switch (m_AllCards[i].CardRarity) {
                case Rarity.Common:
                    CommonCards.Add(m_AllCards[i]);
                    break;
                case Rarity.Rare:
                    CommonCards.Add(m_AllCards[i]);
                    break;
                case Rarity.Legendary:
                    CommonCards.Add(m_AllCards[i]);
                    break;
            }
        }

    }

    public static Card GetRandomCard(Rarity rarity) {
        switch (rarity) {
            case Rarity.Common:
                return GetRandomCard(CommonCards);
            case Rarity.Rare:
                return GetRandomCard(RareCards);
            case Rarity.Legendary:
                return GetRandomCard(LegendaryCards);
            default:
                return null;
        }
    }

    public static Card GetRandomCard(List<Card> cards) {
        if (cards == null || cards.Count < 1) {
            return null;
        }
        Card card = Instantiate(cards[Random.Range(0, cards.Count)].gameObject).GetComponent<Card>();
        return card;
    }

}
