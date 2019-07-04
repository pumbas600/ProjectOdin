using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    public GameController gameController;
    protected string[] addition = new string[] { "1st", "2nd", "3rd", "4th" };
    protected Color[] colours = new Color[4];
    public string abilityName;
    public int pivotAtStart = 1; //0 = false, -1 = not applicable
    public string weaponName;
    public float value;
    public float stamina;
    public int[] fromPos;
    public int[] toPos;
    public int id;
    public string desc = null;
    public bool optional = false;
    
    protected bool CorrectSpot()
    {
        //Checking that you are in a position that is acceptable for the attack:
        foreach (int pos in fromPos)
        {
            if (pos == gameController.currentPlayer || pos == gameController.currentPlayer - gameController.friendlyParty.Length)
            {
                return true;
            }
        }
        return false;
    }

}

