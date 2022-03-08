using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IconsWriter : MonoBehaviour {

    /* --- Components --- */
    public Sprite[] icons;
    public int secondRow;

    public int value; // The text in for this label.
    public SpriteRenderer characterRenderer; // The default character renderer object.

    /* --- Variables --- */
    [SerializeField] [ReadOnly] private SpriteRenderer[] characterRenderers; // Holds the currently rendered characters.
    [SerializeField] [Range(0.05f, 0.5f)] private float spacing = 0.4f; // The spacing between the characters.

    /* --- Unity --- */
    void Start() {
        // SetIcons(value, m_IconIndex);
    }

    public void SetIcons(int value, int index) {
        if (index >= icons.Length) {
            return;
        }

        // Delete the previous text
        if (characterRenderers != null) {
            for (int i = 0; i < characterRenderers.Length; i++) {
                if (characterRenderers[i] != null) {
                    Destroy(characterRenderers[i].gameObject);
                }
            }
        }

        // Create the new characters
        characterRenderers = new SpriteRenderer[value];
        for (int i = 0; i < value; i++) {
            SpriteRenderer newCharacterRenderer = Instantiate(characterRenderer.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            // newCharacterRenderer.transform.localPosition = new Vector3(spacing * i, Mathf.Pow(-1f, i) * spacing / 2f, 0f);
            float y = i > secondRow ? -2f * spacing : 0f;
            float x = 2f * spacing * i + (i >= secondRow ? -2f * spacing * i : 0f);
            newCharacterRenderer.transform.localPosition = new Vector3(x, y, 0f);

            newCharacterRenderer.sprite = icons[index];
            characterRenderers[i] = newCharacterRenderer;
        }
    }

}
