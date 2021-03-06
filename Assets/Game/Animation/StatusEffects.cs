using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour {

    // Hex.
    public static Effect HexEffect;
    public Effect m_HexEffect;

    // Burning.
    public static Effect BurningEffect;
    public Effect m_BurningEffect;

    // Paralyze.
    public static Effect ParalyzeEffect;
    public Effect m_ParalyzeEffect;

    void Start() {
        HexEffect = m_HexEffect;
        BurningEffect = m_BurningEffect;
        ParalyzeEffect = m_ParalyzeEffect;
    }

}
