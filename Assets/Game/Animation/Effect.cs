/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a particle animation.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Effect : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    private SpriteRenderer m_SpriteRenderer;

    // Sprites.
    [SerializeField] private Sprite[] m_Sprites;

    // Settings.
    [SerializeField] private bool m_Loop;
    [SerializeField] private Vector3 m_Target;
    [SerializeField, ReadOnly] private float m_Speed;

    // Animation.
    [SerializeField, ReadOnly] private int m_FrameRate = 12;
    [SerializeField, ReadOnly] private int m_Frame;
    [SerializeField, ReadOnly] private float m_Ticks;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Runs once every frame.
    private void Update() {
        float deltaTime = Time.deltaTime;
        Animate(deltaTime);
        Move(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public Effect Create(Vector2Int position, float lifeTime = -1f) {
        Effect newEffect = Instantiate(gameObject, (Vector3)(Vector2)position, Quaternion.identity, null).GetComponent<Effect>();
        newEffect.Init();
        if (lifeTime > 0f) {
            Destroy(newEffect.gameObject, lifeTime);
        }
        return newEffect;
    }

    public void Init() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Target = transform.position;
        m_Speed = 0f;
        gameObject.SetActive(true);
    }

    public void MoveTo(Vector2Int position, float duration) {
        m_Target = (Vector3)(Vector2)position;
        m_Speed = (m_Target - transform.position).magnitude / duration;
    }

    #endregion

    /* --- Animation --- */
    #region Animation

    private void Animate(float deltaTime) {
        // Set the current frame.
        m_Frame = (int)Mathf.Floor(m_Ticks * m_FrameRate);
        if (!m_Loop && m_Frame >= m_Sprites.Length) {
            Destroy(gameObject);
        }
        m_Frame = m_Frame % m_Sprites.Length;
        m_SpriteRenderer.sprite = m_Sprites[m_Frame];

        m_Ticks += deltaTime;

    }

    private void Move(float deltaTime) {
        Vector3 displacement = m_Target - transform.position;
        if (displacement.sqrMagnitude >= 0.05f * 0.05f) {
            transform.position += displacement.normalized * m_Speed * deltaTime;
        }
    }

    #endregion

}
