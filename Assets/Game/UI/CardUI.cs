using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // References.
    [HideInInspector] protected SpriteRenderer m_SpriteRenderer;
    [SerializeField] protected Card m_Card;

    [Space(2), Header("Rarity")]
    [SerializeField] private Sprite[] m_Frames;
    [SerializeField] private Sprite[] m_Circles;

    // Icons.
    [Space(2), Header("Icons")]
    [SerializeField] private Sprite m_EmptyCard;
    [SerializeField] private Sprite m_ThrowIcon;
    [SerializeField] private Sprite m_DirectionalIcon;
    [SerializeField] private Sprite m_MeleeIcon;
    [SerializeField] private Sprite m_BuffIcon;

    // Design.
    [Space(2), Header("Design")]
    [SerializeField] private Text m_CardName;
    [SerializeField] private Text m_CardDescription;
    [SerializeField] public Text m_Charges;
    [SerializeField] public IconsWriter m_DamageValue;
    [SerializeField] public IconsWriter m_StatusValue;
    [SerializeField] private SpriteRenderer m_CardFace;
    [SerializeField] private SpriteRenderer m_CardTargetType;
    [SerializeField] private SpriteRenderer m_CircleRenderer;
    public SpriteRenderer CardTargetType => m_CardTargetType;
    
    // Selection.
    [Space(2), Header("Interaction")]
    [SerializeField, ReadOnly] protected bool m_MouseOver = false;
    [SerializeField, ReadOnly] protected bool m_Active = false;
    private List<SpriteRenderer> m_TargetIndicators;

    // Transformation.
    [SerializeField, ReadOnly] protected Vector2 m_Origin;
    [SerializeField, ReadOnly] private Vector2 m_IconPosition;
    [SerializeField, ReadOnly] protected float m_Scale;
    [SerializeField] protected float m_ScaleSpeed = 5f;
    protected Dictionary<Transform, Vector3>  m_LocalOrigins;

    private int index;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Origin = transform.position;
        m_IconPosition = m_CardTargetType.transform.localPosition;
    }

    void Update() {
        if (m_Card == null) {
            DrawEmpty();
            return; 
        }

        float deltaTime = Time.deltaTime;
        Draw();
        Interact(deltaTime);
    }

    void LateUpdate() {
        CheckActive();
    }

    void OnMouseOver() {
        m_MouseOver = true;
    }

    void OnMouseExit() {
        m_MouseOver = false;
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public CardUI Create(Card card, int i) {
        CardUI newCardUI = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<CardUI>();
        newCardUI.SetCard(card);
        newCardUI.SetOrigin(i, 0);
        newCardUI.gameObject.SetActive(true);
        return newCardUI;
    }


    public CardUI Create(Card card, int i, int num) {
        CardUI newCardUI = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<CardUI>();
        newCardUI.SetCard(card);
        newCardUI.SetOrigin(i, 0);
        newCardUI.gameObject.SetActive(true);
        return newCardUI;
    }

    public void SetCard(Card card) {
        m_Card = card;
    }

    public void SetOrigin(int i, int num) {
        transform.position += (i - (float)num  / 2f) * Vector3.right * 6f;
        m_Origin = transform.position;
        m_IconPosition = m_CardTargetType.transform.localPosition;

        m_LocalOrigins = new Dictionary<Transform, Vector3>();
        foreach (Transform child in transform) {
            m_LocalOrigins.Add(child, child.localPosition);
        }
        m_LocalOrigins.Add(m_CircleRenderer.transform, m_CircleRenderer.transform.localPosition);

        index = i;
    }

    #endregion

    /* --- Activation --- */
    #region Activation

    private void CheckActive() {
        if (m_Card == null) {
            return;
        }

        if (m_Active && !m_Card.Active) {
            Activate();
        }
        else if (!m_Active && m_Card.Active) {
            Deactivate();
        }
    }

    protected virtual void Activate() {
        Board board = Board.FindInstance();
        if (board != null && board.Get<Player>() != null) {
            m_Card.Activate(board, board.Get<Player>().Position);
        }
    }

    protected virtual void Deactivate() {
        m_Card.Deactivate();
    }

    #endregion

    /* --- Interacting --- */
    #region Interacting 

    private void Interact(float deltaTime) {
        bool m_Input0 = Input.GetMouseButtonDown(0);
        bool m_ReleaseInput0 = Input.GetMouseButtonUp(0);

        if (m_MouseOver && m_Input0) {
            m_Active = true;
        }

        if (m_Active && m_ReleaseInput0) {
            m_Active = false;
        }

        SetPosition(deltaTime);
        SetScale(deltaTime);
    }


    protected virtual void SetPosition(float deltaTime) {

        if (m_Active) {
            Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }
        else {
            transform.position = (Vector3)m_Origin;
        }


        float targetY = 0f;
        if (m_MouseOver && !m_Active) {
            targetY = 3f;
        }

        float currY = m_SpriteRenderer.material.GetVector("_Offset").y;
        if (currY > targetY + 0.1f || currY < targetY - 0.25f) {
            currY += Mathf.Sign(targetY - currY) * 3f * m_ScaleSpeed * deltaTime;
        }
        else {
            currY = targetY;
        }

        foreach (KeyValuePair<Transform, Vector3> item in m_LocalOrigins) {
            item.Key.localPosition = item.Value + Vector3.up * currY;
        }

        m_SpriteRenderer.material.SetVector("_Offset", new Vector4(0f, currY, 0f, 0f));

        m_CircleRenderer.transform.localPosition = m_LocalOrigins[m_CircleRenderer.transform] + bobAmp * Mathf.Sin(Mathf.PI * (Board.Ticks * bobSpeed + (float)index / 6)) * Vector3.up;

        if (m_Active) {
            m_CardTargetType.transform.position = transform.position;
        }

    }

    protected virtual void SetScale(float deltaTime) {

        float targetScale = m_Active ? 0f : 1f; // Mathf.Max(0f, 1f - distance);

        m_Scale = m_Scale != targetScale ? m_Scale + Mathf.Sign(targetScale - m_Scale) * m_ScaleSpeed * deltaTime : m_Scale;
        if (Mathf.Abs(targetScale - m_Scale) < 0.25f) {
            m_Scale = targetScale;
        }

        transform.localScale = m_Scale * new Vector3(1f, 1f, 1f);

        if (m_Scale <= 0f) {
            m_CardTargetType.transform.SetParent(null);
            m_CardTargetType.gameObject.SetActive(true);
            m_CardTargetType.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            // m_CardTargetType.gameObject.SetActive(false);
            m_CardTargetType.transform.SetParent(transform);
            m_CardTargetType.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
    }

    #endregion

    /* --- Drawing --- */
    #region Drawing

    private float bobSpeed = 1.25f;
    private float bobAmp = 1.5f / 160f;

    void Draw() {
        // Draw the name.
        // m_CardName.SetText(m_Card.CardName);
        m_CardName.text = m_Card.CardName;
        // Draw the frame.
        GetComponent<SpriteRenderer>().sprite = m_Frames[(int)m_Card.CardRarity];
        m_CircleRenderer.sprite = m_Circles[(int)m_Card.CardRarity];

        // Draw the face.
        m_CardFace.sprite = m_Card.Face;
        // Draw the type.
        m_CardTargetType.sprite = m_Card.CardIcon; // GetCardTargetTypeIcon(m_Card.CardTargetType);
        // Draw the values.
        m_DamageValue.SetIcons(m_Card.Value, 0);
        if (m_Card.StatusEffect != Status.None) {
            m_StatusValue.SetIcons(m_Card.DurationValue, (int)m_Card.StatusEffect);
        }
        else {
            m_StatusValue.gameObject.SetActive(false);
        }
        // Write the charges.
        m_Charges.text = m_Card.Charges.ToString();
        // m_Charges.SetText(m_Card.Charges.ToString());
        // Draw the description.
        m_CardDescription.text = m_Card.CardDescription;

        // Draw the targets.
        DrawTarget();
    }

    protected virtual void DrawTarget() {
        Board board = Board.FindInstance();
        if (board != null && board.Get<Player>() != null) {
            BoardUI.DrawTargetUI(m_Card, board, board.Get<Player>().Position, ref m_TargetIndicators, m_MouseOver || m_Active);
        }
    }

    void DrawEmpty() {
        // Draw the name.
        // m_CardFace.sprite = m_EmptyCard;
        // m_CardName.gameObject.SetActive(false);
        // Draw the sprites.
        // m_CardTargetType.gameObject.SetActive(false);
        // Draw the values.
        // m_DamageValue.gameObject.SetActive(false);
        // m_Charges.gameObject.SetActive(false);
        // transform.localScale = new Vector3(1f, 1f, 1f);
        // transform.position = (Vector3)m_Origin;
    }

    private Sprite GetCardTargetTypeIcon(TargetType targetType) {
        switch (targetType) {
            case TargetType.Throw:
                return m_ThrowIcon;
            case TargetType.Directional:
                return m_DirectionalIcon;
            case TargetType.Melee:
                return m_MeleeIcon;
            case TargetType.Self:
                return m_BuffIcon;
            default:
                return null;
        }
    }

    #endregion

}
