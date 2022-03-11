using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    /* --- Variables --- */
    #region Variables

    // UI.
    [SerializeField] private PlayerUI m_PlayerUIBase;
    [HideInInspector] private PlayerUI m_PlayerUI;
    public PlayerUI UI => m_PlayerUI;

    #endregion

    /* --- Decision --- */
    #region Decision

    protected override Action GetAction() {
        if (Input.GetKeyDown(KeyCode.W)) {
            return Action.MoveUp;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            return Action.MoveLeft;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            return Action.MoveDown;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            return Action.MoveRight;
        }
        return Action.None;
    }

    protected override Vector2Int? GetTarget(Card card) {
        List<Vector2Int> targettablePositions = card.GetTargetablePositions(m_Board, m_Position);

        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool releaseInput0 = Input.GetMouseButtonUp(0);
        if (releaseInput0) {
            return CheckTargets(targettablePositions);
        }
        return null;
    }

    private Vector2Int? CheckTargets(List<Vector2Int> targettablePositions) {
        for (int i = 0; i < targettablePositions.Count; i++) {
            if (IsMouseOver(targettablePositions[i])) {
                return (Vector2Int?)targettablePositions[i];
            }
        }
        return null;
    }

    private bool IsMouseOver(Vector2Int position) {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0f);
        Vector3 snappedPosition = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y), 0f);
        if (mousePosition.x >= 0 && mousePosition.x < m_Board.Width && mousePosition.y >= 0 && mousePosition.y < m_Board.Height) {
            if (snappedPosition == (Vector3)(Vector2)position) {
                return true;
            }
        }
        return false;
    }

    #endregion

    /* --- Cards --- */
    #region Card
    public override void ReplaceCard(Card oldCard, Card newCard) {
        base.ReplaceCard(oldCard, newCard);
        m_PlayerUI.ResetCards(this);
    }

    public override void RemoveCard(int index) {
        base.RemoveCard(index);
        m_PlayerUI.ResetCards(this);
    }

    public override bool AddCard(Card card) {
        bool success = base.AddCard(card);
        m_PlayerUI.ResetCards(this);
        return success;
    }

    #endregion

    /* --- UI --- */
    #region UI

    void Awake() {
        PlayerUI[] allplayerUIs = (PlayerUI[])GameObject.FindObjectsOfType(typeof(PlayerUI));
        for (int i = 0; i < allplayerUIs.Length; i++) {
            Destroy(allplayerUIs[i].gameObject);
        }

        GameObject ui = Instantiate(m_PlayerUIBase.gameObject, Vector3.zero, Quaternion.identity, null);
        ui.transform.SetParent(null);
        ui.SetActive(true);
        m_PlayerUI = ui.GetComponent<PlayerUI>();
    }

    protected override void SetUI() {
        m_PlayerUI.Refresh(this, m_Board);
       
    }

    protected override void ClearUI() {
        m_PlayerUI.ResetCards(this, false);
        Destroy(m_PlayerUI);
    }

    #endregion

}
