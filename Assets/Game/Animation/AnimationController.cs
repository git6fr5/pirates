using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    public int m_FrameRate;
    public static int FrameRate;

    void Update() {
        FrameRate = m_FrameRate;
    }

}
