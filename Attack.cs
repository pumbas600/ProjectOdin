using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Abilities, IFunction
{
    public int attackType;
    //Temp damage is just so that the variable value doesn't get modified
    private float tempDamage;
    private int offset = 0;

    public List<Vector3> GetHaloPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        UpdateOffset();

        foreach(int character in toPos)
        {
            int pos = character + offset;
            if(pos >= 0 && pos < gameController.enemyParty.Length)
            {
                if (!gameController.enemyParty[pos].GetComponent<Character>().dead)
                {
                    positions.Add(gameController.enemyParty[pos].transform.position);
                }
            }
        }

        return positions;
        
    }

    //Where the first / last person is.
    private void UpdateOffset()
    {
        if (pivotAtStart == 1)
        {
            for (int i = 0; i < gameController.enemyParty.Length; i++)
            {
                if (!gameController.enemyParty[i].GetComponent<Character>().dead)
                {
                    offset = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = gameController.enemyParty.Length - 1; i >= 0; i--)
            {
                if (!gameController.enemyParty[i].GetComponent<Character>().dead)
                {
                    offset = i - 3;
                    break;
                }
            }
        }
    }

    public void Effect()
    {
        Damage();
    }

    public string[] TextValues()
    {
        string[] values = new string[4];
        UpdateOffset();
        for (int i = 0; i < toPos.Length; i++)
        {
            if (toPos[i] + offset >= 0 && toPos[i] + offset < gameController.enemyParty.Length)
            {
                Character script = gameController.enemyParty[toPos[i] + offset].GetComponent<Character>();
                tempDamage = value;
                float multiplier = gameController.combatRules[attackType, script.armourID];
                tempDamage *= multiplier;
                string str = addition[toPos[i] + offset] + " character receives " + tempDamage + " damage";
                values[i] = str;
                if (multiplier == 1)
                {
                    colours[i] = Color.white;
                }
                else
                {
                    colours[i] = multiplier > 1 ? Color.green : Color.red;
                }
            }
        }
        return values;    
    }

    public Color[] ColourValues()
    {
        return colours;
    }

    public void Damage()
    {
        //If you're in an acceptable position: - shouldn't be necessary
        //if (CorrectSpot())

        //Reduces the character's amount of stamina.
        gameController.currentPlayerScript.ReductStamina(stamina);
        UpdateOffset();

        //If its optional, then you would've selected an enemy before you could initiate the attack on one of the buttons (this would be stored in toPos)
        //Otherwise, apply the attack to each of the effected enemies (The effected enemies would be constant & dependent on the attack)
        foreach (int character in toPos)
        {
            if (character + offset >= 0 && character + offset < gameController.enemyParty.Length)
            {
                Character script;
                Transform transform;

                if (gameController.playersTurn)
                {
                    transform = gameController.enemyParty[character + offset].transform;
                    script = gameController.enemyParty[character + offset].GetComponent<Character>();
                }
                else
                {
                    transform = gameController.friendlyParty[character + offset].transform;
                    script = gameController.friendlyParty[character + offset].GetComponent<Character>();
                }
                //sets the damage based on the attack and weapon being used.
                tempDamage = value;

                //updates the damage considering the type of attack and the armour the character is wearing.
                tempDamage *= gameController.combatRules[attackType, script.armourID];
                script.TakeDamage(tempDamage);
                gameController.CreateFadingText(tempDamage, transform);
            }
            
        }

    }

}
