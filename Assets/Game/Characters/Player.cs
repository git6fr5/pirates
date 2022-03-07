using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = Character.Action;

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
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - (Vector2)position).magnitude < 0.5f; // This is a bit weird, it'll miss the edges.
    }

    #endregion

    /* --- Cards --- */
    #region Card

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
