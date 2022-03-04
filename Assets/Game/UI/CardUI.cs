using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUI : MonoBehaviour {

    public PlayerUI m_PlayerUI;

    public Card m_Card;
    public Sprite m_EmptyCardSprite;
    public Sprite m_CardSprite;

    public bool m_MouseOver = false;
    public bool m_Active = false;

    public Vector2 m_Origin;

    public float m_Scale;
    public Transform m_TargettingUI;

    public Label m_CardName;
    public Label m_Charges;
    public Label m_Value;
    public SpriteRenderer m_Icon;

    public CardUI Create(PlayerUI playerUI, Card card, int i) {
        CardUI newCardUI = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<CardUI>();
        newCardUI.SetOrigin(i);
        newCardUI.SetCard(playerUI, card);
        newCardUI.gameObject.SetActive(true);
        return newCardUI;
    }

    public void SetOrigin(int i) {
        transform.position += i * Vector3.right * 4f;
        m_Origin = transform.position;
    }

    public void SetCard(PlayerUI playerUI, Card card) {
        m_PlayerUI = playerUI;
        m_Card = card;
        if (card == null) {
            GetComponent<SpriteRenderer>().sprite = m_EmptyCardSprite;
        }
        else {
            m_TargettingUI.GetComponent<SpriteRenderer>().sprite = card.CardIcon;
            GetComponent<SpriteRenderer>().sprite = m_CardSprite;
        }
    }

    void Update() {
        bool m_Input0 = Input.GetMouseButtonDown(0);
        bool m_ReleaseInput0 = Input.GetMouseButtonUp(0);

        DrawCard();

        m_Card.DrawTargetablePositions(m_PlayerUI.GetBoard(), m_PlayerUI.GetPlayerPosition(), m_MouseOver || m_Active);

        if (m_MouseOver && m_Input0) {
            m_Active = true;
        }

        if (m_Active && m_ReleaseInput0) {
            m_Active = false;
        }

        if (m_Active) {
            FollowMouse();
            m_Card.Activate(m_PlayerUI.GetBoard(), m_PlayerUI.GetPlayerPosition());
        }
        else {
            transform.position = (Vector3)m_Origin;
        }

        float distance = ((Vector2)transform.position - m_Origin).magnitude;
        distance = distance > 0.5f ? distance : 0f;
        float targetScale = Mathf.Max(0f, 1f - distance);
        if (m_Scale != targetScale) {
            m_Scale += Mathf.Sign(targetScale - m_Scale) * 5f * Time.deltaTime;
        }
        if (Mathf.Abs(targetScale - m_Scale) < 0.05f) {
            m_Scale = targetScale;
        }

        transform.localScale = m_Scale * new Vector3(1f, 1f, 1f);

        if (m_Scale <= 0f) {
            m_TargettingUI.SetParent(null);
            m_TargettingUI.position = transform.position;
            m_TargettingUI.gameObject.SetActive(true);
            m_TargettingUI.localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            m_TargettingUI.SetParent(transform);
            m_TargettingUI.gameObject.SetActive(false);
        }
    }

    void LateUpdate() {
        if (!m_Active && m_Card.Active) {
            m_Card.Deactivate();
        }
    }

    void DrawCard() {
        if (m_Card != null) {
            m_CardName.SetText(m_Card.CardName);
            m_Value.SetText(m_Card.Value.ToString());
            m_Charges.SetText(m_Card.Charges.ToString());
            m_Icon.sprite = m_Card.CardIcon;
        }
    }

    void FollowMouse() {
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

}
