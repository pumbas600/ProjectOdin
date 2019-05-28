using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //GameController Variables:
    public GameObject controller;
    protected GameController gameController;

    //ARMOUR ID:
    public int armourID = 0;
    const int NO_ARMOUR     = 0;
    const int LOW_ARMOUR    = 1;
    const int MEDIUM_ARMOUR = 2;
    const int HEAVY_ARMOUR  = 3;

    //Health & Stamina Bar Variables
    public float maxHealth;
    public float health;
    public float stamina = 100f;
    public bool dead = false;

    //If this characters being shielded, -1 means no one's shielding this character.
    public int shielder = -1;

    public void TakeDamage(float damage) {
        if (shielder == -1) {
            //How will armour work? the damage may need to be reduced here then
            health -= damage;
            if (health <= 0) {
                health = 0;
                dead = true;
            }
        }
        else {
            //Then the shielders shield needs to take damage
            Huskarl huskarlScript = gameController.friendlyParty[shielder].GetComponent<Huskarl>();
            if (!huskarlScript.TakeShieldDamage(damage))
                shielder = -1;
        }
    }

    public void Heal(float amount) {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }


    public void Start() {
        gameController = controller.GetComponent<GameController>();
        health = maxHealth;
    }
}
