using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePool : MonoBehaviour
{

    // Treasure Pool.
    [SerializeField] private Card[] m_AllCards;
    [SerializeField] private static List<Card> AllCards;
    [SerializeField] private static List<Card> CommonCards;
    [SerializeField] private static List<Card> RareCards;
    [SerializeField] private static List<Card> LegendaryCards;

    void Start() {
        AllCards = new List<Card>();
        CommonCards = new List<Card>();
        RareCards = new List<Card>();
        LegendaryCards = new List<Card>();

        for (int i = 0; i < m_AllCards.Length; i++) {
            AllCards.Add(m_AllCards[i]);
            switch (m_AllCards[i].CardRarity) {
                case Rarity.Common:
                    CommonCards.Add(m_AllCards[i]);
                    break;
                case Rarity.Rare:
                    RareCards.Add(m_AllCards[i]);
                    break;
                case Rarity.Legendary:
                    LegendaryCards.Add(m_AllCards[i]);
                    break;
            }
        }

    }

    public static Card GetCompletelyRandomCard(int charges = 1) {
        Card card = GetCompletelyRandomCard();
        card.SetCharges(charges + 1);
        return card;
    }

    public static Card GetCompletelyRandomCard() {
        if (AllCards == null || AllCards.Count == 0) {
            return null;
        }
        Card card = Instantiate(AllCards[Random.Range(0, AllCards.Count)].gameObject).GetComponent<Card>();
        return card;
    }

    public static Card GetRandomCardWithCharges(Rarity rarity, int charges = 1) {
        Card card = GetRandomCard(rarity);
        card.SetCharges(charges + 1);
        return card;
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
