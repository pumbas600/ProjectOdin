using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour
{

    new public Animation animation;
    // Start is called before the first frame update
    void OnEnable()
    {
        Destroy(gameObject, animation.clip.length);
    }

    public void SetText(string text, Color colour)
    {
        Text textComponent = animation.GetComponentInChildren<Text>();
        textComponent.text = text;
        textComponent.color = colour;
    }
}
