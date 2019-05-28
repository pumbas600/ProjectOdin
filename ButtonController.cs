using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    public const int numMainButtons = 4;
    public const int numSecondaryButtons = 4;

    public int[] selectedButtons = new int[3] { -1, -1, -1 };

    public GameObject controller;
    private GameController gameController;

    public Button[] mainButtons = new Button[numMainButtons];
    public Button[] secondaryButtons = new Button[numSecondaryButtons];

    public Color selectedColour = new Color(255, 255, 0);
    public Color unselectedColour = new Color(255, 255, 255);

    // Start is called before the first frame update
    void Start() {
        //set ids for all mainButtons:
        for (int i = 0; i < numMainButtons; i++) {
            ButtonFunctions tempScript = mainButtons[i].GetComponent<ButtonFunctions>();
            tempScript.id = i;
        }
        //set ids for all secondaryButtons: (This is continuing on from the mainButtons ids)
        for (int i = 0; i < numSecondaryButtons; i++) {
            ButtonFunctions tempScript = secondaryButtons[i].GetComponent<ButtonFunctions>();
            tempScript.id = numMainButtons + i;
        }
        gameController = controller.GetComponent<GameController>();
        HideButtons(0); //Will hide all secondary buttons

    }

    public void DisplayTertiaryInformation(int id) {

    }

    public void DisplaySecondaryButtons(int id) {
        //Get the current players "Player" script and find the relevant abilities list
        Player playersScript = gameController.friendlyParty[gameController.currentPlayer].GetComponent<Player>();
        //TEMPORARILY CHANGED FROM "Abilities[]" TO "string[]"
        string[] abilities = playersScript.GetAbilities(id);
        //Loop through all the secondary buttons
        for (int i = 0; i < secondaryButtons.Length; i++) {

            //Check that the player has an ability "saved?"

            if (abilities[i] != null) {
                //If so, then set that button active with the name of the ability
                secondaryButtons[i].gameObject.SetActive(true);
                Text text = secondaryButtons[i].GetComponentInChildren<Text>();
                //TEMPORARILY CHANGED FROM "abilities[i].name" TO "abilities[i]"
                text.text = abilities[i];
            }
            else { //For when you hover over a button that uses less of the buttons (just clears the extras)
                secondaryButtons[i].gameObject.SetActive(false);
            }
        }
    }
    public void OnHoverEntry(int id) {
        if (id < numMainButtons) {
            if (id != selectedButtons[0] && selectedButtons[0] != -1) {
                mainButtons[selectedButtons[0]].image.color = unselectedColour;
                selectedButtons[0] = -1;
                HideButtons(0);
            }
        }
        else {// Must be a secondary button otherwise:
            if(id - numMainButtons != selectedButtons[1] && selectedButtons[1] != -1) {
                secondaryButtons[selectedButtons[1]].image.color = unselectedColour;
                selectedButtons[1] = -1;
                HideButtons(1);
            }
        }
    }

    public void OnHoverExit(int id) {
        if (id < numMainButtons) {
            if(id != selectedButtons[0]) {
                mainButtons[id].image.color = unselectedColour;
            }
   
        }
        else { //Must be a secondary button otherwise:
            if (id - numMainButtons != selectedButtons[1]) {
                secondaryButtons[id - numMainButtons].image.color = unselectedColour;
            }
        }

    }

    public void HideButtons(int id) {

        if (id == 0) { //id for secondary buttons
            foreach (Button button in secondaryButtons) {
                button.image.color = unselectedColour;
                button.gameObject.SetActive(false);
            }

        }
        else { //otherwise its for the tertiary buttons
            //Hide Tertiary buttons / info
        }
    }

    public void ButtonSelected(int id) {
        
        if (id < numMainButtons) {
            if (selectedButtons[0] == id) { //If you toggle selected primary button:
                selectedButtons[0] = -1;
                //Hides all secondary buttons
                HideButtons(0);
            }
            else { //Otherwise
                selectedButtons[0] = id;
                mainButtons[id].image.color = selectedColour;
                if (mainButtons[id].name != "Flee") {
                    DisplaySecondaryButtons(id);
                }
            }
        }

        else if (id - numMainButtons < numSecondaryButtons) {
            if (selectedButtons[1] == id - numMainButtons) { //If you toggle selected primary button:
                selectedButtons[1] = -1;
                secondaryButtons[id - numMainButtons].image.color = unselectedColour;
                //Hide tertiary infomation
                HideButtons(1);
            }
            else { //Otherwise
                selectedButtons[1] = id - numMainButtons;
                secondaryButtons[id - numMainButtons].image.color = selectedColour;
                //Display the tertiary information:
                DisplayTertiaryInformation(id - numMainButtons);
            }
        }
    }

    // Update is called once per frame
    void Update() {

     
    }
}
