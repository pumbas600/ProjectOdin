using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //int position = 1;
    public string[][] playerAbilities = new string[3][];

    public GameObject controller;
    protected GameController gameController;



    public float maxHealth = 100f;
    public float health;
    public bool dead = false;
    public float stamina = 100f;

    public float damage = 10f;

    public int shielder = -1;
    public float shieldStrength = -1;

    //Temporarily changed to return String[]
    public string[] GetAbilities(int id) {
        //Return the relevant abilities when requested
        return playerAbilities[id];
    }

    public void TakeDamage(float damage) {
        if (shielder == -1) { //ID of person shielding them (-1 means no shield)
            health -= damage;
            if (health <= 0) {
                health = 0;
                dead = true;
            }
        }
        else {
            Player shielderScript = gameController.friendlyParty[shielder].GetComponent<Player>();
            shielderScript.shieldStrength -= damage; 
            

        }
        
        
    }

    public void Heal(float amount) {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;    
    }
    // Start is called before the first frame update
    void Start()
    {
        gameController = controller.GetComponent<GameController>();
        playerAbilities[0] = new string[4] { "Slash", "Thrust", "Side Slash", "Overhead" };
        playerAbilities[1] = new string[4] { "Something", "Shield", "Heal", null };
        playerAbilities[2] = new string[4] { "Health Pack", "Gold", null, null };

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }

        if (gameController.playersTurn)
        {
             
        }
            
    }
}


