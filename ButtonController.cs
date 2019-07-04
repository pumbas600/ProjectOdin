using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    public const int numMainButtons = 4;
    public const int numSecondaryButtons = 4;

    public const int maxPartyLength = 4;

    public int[] selectedButtons = new int[3] { -1, -1, -1 };

    public GameObject controller;
    private GameController gameController;

    public Button[] mainButtons = new Button[numMainButtons];
    public Button[] secondaryButtons = new Button[numSecondaryButtons];
    public Button[] tertiaryButtons = new Button[2 * maxPartyLength];

    public GameObject tertiaryInfo;
    public Text[] infoText = new Text[maxPartyLength];

    public Button confirmButton;
    public Text stamina;

    public GameObject startPivot;
    public GameObject endPivot;

    public List<GameObject> halos = new List<GameObject>();

    public GameObject smallHalo;

    public Color selectedColour = new Color(255, 255, 0, 255);
    public Color unselectedColour = new Color(255, 255, 255, 255);

    public Color positionColour = new Color(0, 0, 0, 255);

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

        for (int i = 0; i < 2 * maxPartyLength; i++)
        {
            ButtonFunctions tempScript = tertiaryButtons[i].GetComponent<ButtonFunctions>();
            tempScript.id = numMainButtons + numSecondaryButtons + i;
        }

        gameController = controller.GetComponent<GameController>();
        confirmButton.onClick.AddListener(ConfirmFunction);
        HideButtons(0); //Will hide all secondary buttons & also the tertiary buttons


    }

    public void SetPrimaryButtons()
    {
        foreach (Button button in mainButtons)
        {
            button.interactable = true;
        }
    }


    public void SetColours(int[] positions, int indexOffset, bool isInteractable)
    {
        //For each character in the party
        for (int i = 0; i < tertiaryButtons.Length / 2; i++)
        {
            //set the corresonding tertiary button to active (if party is enemyParty, they are stored in the array
            //after the friendly party buttons, thus the offset is necessary)
            tertiaryButtons[i + indexOffset].gameObject.SetActive(true);
            //Setting interactable to false changes alpha value
            tertiaryButtons[i + indexOffset].interactable = false;

            //For every position in the positions array
            foreach (int pos in positions)
            {
                //if the current button is one of the referenced positions, then set its colour
                if (i == pos)
                {
                    tertiaryButtons[i + indexOffset].image.color = positionColour;
                    //interactable is only if the move is optional, and who it effects must be selected from several possibilies
                    tertiaryButtons[i + indexOffset].interactable = isInteractable;
                }
            }

        }
    }

    public void ConfirmFunction()
    {
        //As this function can only be called when its "interactable", then we know for optional moves
        //that a position must have been selected.
        //For the optional moves, the selected position is found in selectedButtons[2]
        Character script = gameController.currentPlayerScript;
        IFunction temp = script.equippedAbilities[selectedButtons[0]][selectedButtons[1]];
        Abilities ability = (Abilities) temp;

        //Call the Effect function, which will carryout the purpose of each ability.
        temp.Effect();

        if (script.stamina < ability.stamina)
            confirmButton.interactable = false;

        //Remove all the halos, before changing to the next person's turn:
        halos.ForEach(x => Destroy(x));

        //Also hide secondary buttons and set primary buttons to uninteractable:
        HideButtons(0);
        foreach(Button button in mainButtons)
        {
            button.interactable = false;
        }
        //Changes the selected buttons colour back to the unselected colour.
        mainButtons[selectedButtons[0]].image.color = unselectedColour;

        gameController.ChangeTurn();
    }

    public void DisplayTertiaryInformation(int id) {
        HidePivots();
        //Get the Character script of the current player
        Character script = gameController.currentPlayerScript;
        
        //Then get the moves corresponding to the main Button selected (fight, defend, etc) and the id of the secondary button pressed 
        //the id is referencing a specific move within the equipped abilities 2d array (equippedAbilities[mainButton, secondaryButton])
        IFunction temp = script.equippedAbilities[selectedButtons[0]][id];
        //Cast to Abilities type to get access to the abilities data
        Abilities ability = (Abilities)temp;

        if(ability.pivotAtStart == 1)
        {
            startPivot.SetActive(true);
        }
        else if(ability.pivotAtStart == 0)
        {
            endPivot.SetActive(true);
        }
        //Get the damage stats and colours from the specific child class of Abilities through the interface IFunction
        string[] texts = temp.TextValues();
        Color[] colours = temp.ColourValues();

        //Set the text, colour, and activity of each of the texts (these show the damage to each player)
        if (secondaryButtons[id].interactable)
        {
            for (int i = 0; i < infoText.Length; i++)
            {
                if (texts[i] != null || texts[i] != "")
                {
                    infoText[i].gameObject.SetActive(true);
                    infoText[i].text = texts[i];
                    infoText[i].color = colours[i];
                }
                //This is just in case, it shouldn't be necessary, although thats untested:
                else
                {
                    infoText[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach(Text info in infoText)
            {
                info.gameObject.SetActive(false);
            }
        }
        //interactable is only if the move is optional, and who it effects must be selected from several possibilies
        bool isInteractable = ability.optional;
        bool selectedPosition = !ability.optional;

        //set the parent gameobject to active
        tertiaryInfo.SetActive(true);

        //Set the colour of each button corresponding to a position in the fromPos array
        SetColours(ability.fromPos, 0, false);

        //Do the same for the toPos array
        SetColours(ability.toPos, maxPartyLength, isInteractable);

        //Display the halo around effected enemies

        //But only if you are in a position that can use the ability
        if (secondaryButtons[id].interactable && gameController.playersTurn)
        {
            foreach (Vector3 position in temp.GetHaloPositions())
            {
                //If this is the case, instantiate a halo gameobject at the right position and set its colour.
                Vector3 newPlace = new Vector3(position.x - gameController.HaloOffset.x, position.y + gameController.HaloOffset.y);

                GameObject halo = Instantiate(smallHalo, newPlace, Quaternion.identity);
                halo.GetComponent<SpriteRenderer>().color = positionColour;
                halos.Add(halo);
            }
        }

        stamina.text = "Requires " + ability.stamina + " stamina";
        //If the ability isn't optional then set confirm button to active (which is the opposite of isInteractable)

        Character currentScript = gameController.currentPlayerScript;

        if(selectedPosition && currentScript.stamina >= ability.stamina && gameController.playersTurn)
            confirmButton.interactable = true;

    }

    public void DisplaySecondaryButtons(int id) {
        //Get the current players "Player" script and find the relevant abilities list
        Character script = gameController.currentPlayerScript;
        IFunction[] abilities = script.GetAbilities(id);
        //Loop through all the secondary buttons
        for (int i = 0; i < secondaryButtons.Length; i++) {

            //Check that the player has an ability "saved?"

            if (abilities[i] != null) {
                //If so, then set that button active with the name of the ability
                Abilities ability = (Abilities)abilities[i];

                

                secondaryButtons[i].gameObject.SetActive(true);
                
                secondaryButtons[i].interactable = false;
                //Only allow you to select ability if you're in an acceptable position
                foreach (int pos in ability.fromPos)
                { 
                    if (gameController.currentPlayerScript.position == pos)
                    {
                        secondaryButtons[i].interactable = true;
                        break;

                    }
                }

                bool allDead = true;
                foreach (int pos in ability.toPos)
                {
                    if (!gameController.enemyParty[pos].GetComponent<Character>().dead)
                    {
                        allDead = false;
                        break;
                    }
                }

                if (allDead)
                {
                    secondaryButtons[i].interactable = false;
                }

                Text text = secondaryButtons[i].GetComponentInChildren<Text>();
                
                //Access abilities script
               
                text.text = ability.weaponName + ": " + ability.abilityName;
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
        else if(id - numMainButtons < numSecondaryButtons){// Must be a secondary button otherwise:
            if(id - numMainButtons != selectedButtons[1] && selectedButtons[1] != -1) {
                secondaryButtons[selectedButtons[1]].image.color = unselectedColour;
                selectedButtons[1] = -1;
                HideButtons(1);
            }
            DisplayTertiaryInformation(id - numMainButtons);
            //But set the interactability of the confirm button to false when hovering.
            confirmButton.interactable = false;
        }
        else
        {
            //tertiary Button
        }
    }

    private void HidePivots()
    {
        startPivot.SetActive(false);
        endPivot.SetActive(false);
    }

    public void OnHoverExit(int id) {
        if (id < numMainButtons) {
            if(id != selectedButtons[0]) {
                mainButtons[id].image.color = unselectedColour;
            }
   
        }
        else if (id - numMainButtons < numSecondaryButtons)
        { //Must be a secondary button otherwise:
            if (id - numMainButtons != selectedButtons[1]) {
                secondaryButtons[id - numMainButtons].image.color = unselectedColour;
            }
            if (id - numMainButtons != selectedButtons[1])
            {
                HideButtons(1);
            }
        }
        else
        {
            //Tertiary button
        }
    }

    public void HideButtons(int id) {

        if (id == 0) { //id for secondary buttons
            foreach (Button button in secondaryButtons) {
                button.image.color = unselectedColour;
                button.gameObject.SetActive(false);
            }
            HideButtons(1);

        }
        else { //otherwise its for the tertiary buttons
            //Hide Tertiary buttons / info           
            foreach(Button button in tertiaryButtons)
            {
                button.image.color = unselectedColour;
                button.gameObject.SetActive(false);
            }
            tertiaryInfo.SetActive(false);
            //Lambda expression which deletes each element of the list 
            halos.ForEach(x => Destroy(x));
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
                HideButtons(1);
                DisplayTertiaryInformation(id - numMainButtons);
            }
        } else
        {
            //Tertiary button
            if (selectedButtons[2] == id - (numMainButtons + numSecondaryButtons))
            { //If you toggle selected primary button:
                selectedButtons[2] = -1;
                //Set confirm button to uninteractable
                confirmButton.interactable = false;
            }
            else
            { //Otherwise
                selectedButtons[2] = id - (numMainButtons + numSecondaryButtons);
                tertiaryButtons[id - (numMainButtons + numSecondaryButtons)].image.color = selectedColour;
                //Allow you to select the confirm button
                confirmButton.interactable = true;
            }
        }
    }

    // Update is called once per frame
    void Update() {

     
    }
}

