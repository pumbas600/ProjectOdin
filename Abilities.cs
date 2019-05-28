using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities
{
    public string abilityName;
    //public string weaponName; //could also be used for class specific attacks.
    public float value;
    public int[] fromPos;
    public int[] toPos;
    public string desc;
    public bool optional;
    public int methodToCall;


    public Abilities(string abilityName, float value, int[] fromPos, int[] toPos, string desc, bool optional, int attackType, int methodToCall) {
        this.abilityName = abilityName;
        this.value = value;
        this.fromPos = fromPos;
        this.toPos = toPos;
        this.desc = desc;
        this.optional = optional;
        this.methodToCall = methodToCall;
    }

    public void MethodCall(GameController gameController) {
        switch (methodToCall) {
            case 0:
                Attack(gameController);
                break;
            case 1:
                Heal(gameController);
                break;
             
        }
    }

    private bool CorrectSpot(GameController gameController) {
        //Checking that you are in a position that is acceptable for the attack:
        foreach (int pos in fromPos) {
            if (pos == gameController.currentPlayer) {
                return true;
            }
        }
        return false;
    }

    //Ability Functions:
    public void Attack(GameController gameController) {

        //If you're in an acceptable position:
        if (CorrectSpot(gameController)) {

            //If its optional, then you would've selected an enemy before you could initiate the attack on one of the buttons (this would be stored in toPos)
            //Otherwise, apply the attack to each of the effected enemies (The effected enemies would be constant & dependent on the attack)
            foreach (int opposition in toPos) {
                if (gameController.playersTurn) {
                    gameController.enemyParty[opposition].GetComponent<Enemy>().TakeDamage(value);
                }
                else {
                    gameController.friendlyParty[opposition].GetComponent<Player>().TakeDamage(value);
                }
               
                //LATER ADD DAMAGE SCROLL TEXT:
            }
        }
    }

    public void Heal(GameController gameController) {
        if (CorrectSpot(gameController)) {
            foreach (int teamMate in toPos) {

                //The players turn:
                if (gameController.playersTurn) {
                    if (value <= 1) { //Its a percentage then:
                        Character script = gameController.friendlyParty[teamMate].GetComponent<Character>();
                        script.Heal(script.maxHealth * value);

                    } //Otherwise its an actual value:
                    else {
                        gameController.friendlyParty[teamMate].GetComponent<Character>().Heal(value);
                    }
                }
                //The oppositions turn:
                else { 
                    if (value <= 1) { //Its a percentage then:
                        Character script = gameController.enemyParty[teamMate].GetComponent<Character>();
                        script.Heal(script.maxHealth * value);
                    } //Otherwise its an actual value:
                    else {
                        gameController.enemyParty[teamMate].GetComponent<Character>().Heal(value);
                    }
                }
                //LATER ADD HEAL SCROLL TEXT:
            }
        }    
    }




}
