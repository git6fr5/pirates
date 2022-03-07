using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake Instance;

    [Header("Shake")]
    [SerializeField, ReadOnly] public float m_ShakeStrength = 1f;
    [SerializeField, ReadOnly] public float m_Duration = 0.5f;
    [SerializeField, ReadOnly] float m_Ticks = 0f;
    [SerializeField, ReadOnly] public bool m_Shake;
    [SerializeField] public bool Shake => m_Shake;

    public AnimationCurve m_Curve;

    public Vector3 m_Origin;

    void Start() {
        m_Origin = transform.position;
        Instance = this;
    }

    void Update() {
        // Shake the camera
        if (m_Shake) {
            m_Shake = WhileShake();
        }
    }

    public bool WhileShake() {
        transform.position = m_Origin;

        m_Ticks += Time.deltaTime;
        if (m_Ticks >= m_Duration) {
            m_Ticks = 0f;
            return false;
        }
        float strength = m_ShakeStrength * m_Curve.Evaluate(m_Ticks / m_Duration);
        transform.position += (Vector3)Random.insideUnitCircle * m_ShakeStrength;
        return true;
    }

    public void StartShake(float duration) {
        m_Shake = true;
        m_Duration = duration;
    }

    /* --- Events --- */
    public static void DelayedShake(float delay, float duration) {
        Instance.StartCoroutine(Instance.IEDelayedShake(delay, duration));
    }

    IEnumerator IEDelayedShake(float delay, float duration) {
        yield return new WaitForSeconds(delay);
        ActivateShake(delay);
    }

    public static void ActivateShake(float duration) {
        if (!Instance.Shake) {
            Instance.StartShake(duration);
        }
    }

}
