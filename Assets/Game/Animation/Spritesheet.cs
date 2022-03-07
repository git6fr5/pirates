/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */
using Action = Character.Action;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Character))]
public class Spritesheet : MonoBehaviour {

    /* --- Components --- */
    [Space(2), Header("Components")]
    [HideInInspector] public SpriteRenderer m_SpriteRenderer;
    [HideInInspector] public Character m_Character;
    [SerializeField] private Sprite[] m_Sprites;

    /* --- Parameters --- */
    [Space(2), Header("Parameters")]
    [SerializeField] private int m_IndexOffset;
    [SerializeField] private int m_IdleFrames;
    [SerializeField] private int m_ActionFrames;
    [SerializeField] private int m_BuffFrames;
    [SerializeField] private int m_MovementFrames;

    /* --- Properties --- */
    [Space(2), Header("Properties")]
    [HideInInspector] private Sprite[] m_IdleAnimation;
    [HideInInspector] private Sprite[] m_ActionAnimation;
    [HideInInspector] private Sprite[] m_BuffAnimation;
    [HideInInspector] private Sprite[][] m_MoveAnimations;

    [Space(2), Header("Properties")]
    [SerializeField, ReadOnly] private float m_IdleFrameRate;
    [SerializeField, ReadOnly] private float m_ActionFrameRate;
    [SerializeField, ReadOnly] private float m_BuffFrameRate;
    [SerializeField, ReadOnly] private float m_MoveFrameRate;

    [Space(2), Header("Animation Data")]
    [HideInInspector] private Sprite[] m_CurrentAnimation;
    [HideInInspector] private Sprite[] m_PreviousAnimation;
    [SerializeField, ReadOnly] private float m_FrameRate;
    [SerializeField, ReadOnly] private int m_DirectionIndex;
    [SerializeField, ReadOnly] private int m_CurrentFrame;

    [Space(2), Header("Ticks")]
    [SerializeField, ReadOnly] private float m_Ticks;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        float deltaTime = Time.deltaTime;
        Animate(deltaTime);
    }

    /* --- Methods --- */
    public void Init() {
        // Caching components.
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Character = GetComponent<Character>();
        Organize();
    }

    private void Animate(float deltaTime) {

        m_CurrentAnimation = GetAnimation(m_Character);

        m_Ticks = m_PreviousAnimation == m_CurrentAnimation ? m_Ticks + deltaTime : 0f;
        m_CurrentFrame = (int)Mathf.Floor(m_Ticks * m_FrameRate) % m_CurrentAnimation.Length;

        // Set the current frame.
        m_SpriteRenderer.sprite = m_CurrentAnimation[m_CurrentFrame];
        m_PreviousAnimation = m_CurrentAnimation;
    }

    // Gets the current animation info.
    public Sprite[] GetAnimation(Character character) {
        if ((int)character.CurrAction < 4 && (int)character.CurrAction <= m_MoveAnimations.Length) {
            m_FrameRate = m_MoveFrameRate;
            return m_MoveAnimations[(int)character.CurrAction];
        }
        else if ((int)character.CurrAction < 7) {
            m_FrameRate = m_ActionFrameRate;
            print("ACtion Animation");
            return m_ActionAnimation;
        }
        else {
            m_FrameRate = m_IdleFrameRate;
            return m_IdleAnimation;
        }
    }

    // Organizes the sprite sheet into its animations.
    public void Organize() {
        int startIndex = m_IndexOffset;

        startIndex = SliceSheet(startIndex, m_IdleFrames, ref m_IdleAnimation);
        m_IdleFrameRate = 3f *  m_IdleFrames / 2f;

        startIndex = SliceSheet(startIndex, m_ActionFrames, ref m_ActionAnimation);
        m_ActionFrameRate = m_ActionFrames / (Board.FindInstance().TurnDelay);

        startIndex = SliceSheet(startIndex, m_BuffFrames, ref m_BuffAnimation);
        m_BuffFrameRate = m_BuffFrames / (Board.FindInstance().TurnDelay);

        m_MoveAnimations = new Sprite[4][];
        for (int i = 0; i < 4; i++) {
            startIndex = SliceSheet(startIndex, m_MovementFrames, ref m_MoveAnimations[i]);
        }
        m_MoveFrameRate = 3f * m_MovementFrames;

    }

    // Slices an animation out of the the sheet.
    private int SliceSheet(int startIndex, int length, ref Sprite[] array) {
        List<Sprite> splicedSprites = new List<Sprite>();
        for (int i = startIndex; i < startIndex + length; i++) {
            splicedSprites.Add(m_Sprites[i]);
        }
        array = splicedSprites.ToArray();
        return startIndex + length;
    }

}
