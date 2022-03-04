using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public Player m_Player;
    public CardUI m_BaseCardUI;
    public List<CardUI> m_CardUI;

    public SpriteRenderer m_BaseHeart;
    public List<SpriteRenderer> m_Hearts;

    public Board m_Board;

    public SpriteRenderer m_MouseIndicator;

    public void Set(Board board, Player player) {
        m_Player = player;
        m_Board = board;
        SetCardUI();
    }

    void Update() {
        if (m_Player.CompletedTurn) {
            for (int i = 0; i < m_CardUI.Count; i++) {
                m_CardUI[i].gameObject.SetActive(false);
            }
        }
        else {
            for (int i = 0; i < m_CardUI.Count; i++) {
                m_CardUI[i].gameObject.SetActive(true);
            }
        }
        SetHealthUI();

        SetMouseIndicator();
    }

    void SetMouseIndicator() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector3 snappedPosition = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y), 0f);
        if (mousePosition.x >= 0 && mousePosition.x < m_Board.Width && mousePosition.y >= 0 && mousePosition.y < m_Board.Height) {
            m_MouseIndicator.gameObject.SetActive(true);
            m_MouseIndicator.transform.position = snappedPosition;
        }
        else {
            m_MouseIndicator.gameObject.SetActive(false);
        }
    }

    public Board GetBoard() {
        return m_Board;
    }

    public Vector2Int GetPlayerPosition() {
        return m_Player.Position;
    }

    private void SetCardUI() {
        if (m_Player == null) {
            return;
        }

        if (m_CardUI != null) {
            for (int i = 0; i < m_CardUI.Count; i++) {
                CardUI card = m_CardUI[i];
                Destroy(card.gameObject);
            }
        }

        m_CardUI = new List<CardUI>();
        for (int i = 0; i < m_Player.Cards.Length; i++) {
            CardUI newCardUI = m_BaseCardUI.Create(this, m_Player.Cards[i], i);
            m_CardUI.Add(newCardUI);
        }

    }

    private void SetHealthUI() {
        if (m_Player == null) {
            return;
        }

        if (m_Hearts != null) {
            for (int i = 0; i < m_Hearts.Count; i++) {
                SpriteRenderer renderer = m_Hearts[i];
                Destroy(renderer.gameObject);
            }
        }

        m_Hearts = new List<SpriteRenderer>();
        for (int i = 0; i < m_Player.Hearts; i++) {
            SpriteRenderer newHeart = Instantiate(m_BaseHeart.gameObject, m_BaseHeart.transform.position, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            newHeart.transform.position += i * 1f * Vector3.right;
            newHeart.gameObject.SetActive(true);
            m_Hearts.Add(newHeart);
        }

    }
}
