using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public int id;
    Button button;

    ButtonController buttonController;

    public void OnHoverEnter() {
        buttonController.OnHoverEntry(id);
        button.image.color = buttonController.selectedColour;
    }
    public void OnHoverExit() {

        buttonController.OnHoverExit(id);

    }
    public void ToggleSelected() {
        buttonController.ButtonSelected(id);
    }

    void Start() {

        //Creates the onclick component:
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleSelected);

        buttonController = button.GetComponentInParent<ButtonController>();

        //Creates the hovering components when added to button:
        gameObject.AddComponent<EventTrigger>();
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        exit.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener(delegate{ OnHoverEnter(); });
        exit.callback.AddListener(delegate { OnHoverExit(); });
        trigger.triggers.Add(entry);
        trigger.triggers.Add(exit);

    }

    void Update() {
        
    }
}
