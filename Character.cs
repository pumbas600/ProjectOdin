using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{

    public List<GameObject> equippedWeapons = new List<GameObject>();

    public int position;
    //GameController Variables:
    public GameObject controller;
    protected GameController gameController;

    //will be turned into an array which corresponds to the different body parts.
    public int armourID = 0;
    public bool friendly = false;


    //Class variables:
    public int classID;
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float speed = 30f;
    public float rechargeStamina = 20f;

    //Used in the OnEnable function
    public List<IFunction> possibleAttacks = new List<IFunction>();
    //Jagged array of the equipped abilities (0.Attacks, 1.Defensive, 2.Items)
    public IFunction[][] equippedAbilities = new IFunction[3][] { new IFunction[4], new IFunction[4], new IFunction[4] };

    //Health & Stamina Bar Variables
    public GameObject healthBar;
    public GameObject staminaBar;
    public GameObject deadCross;
    public float health;
    public float stamina;
    public bool dead = false;


    
    //If this characters being shielded, -1 means no one's shielding this character.
    public int shielder = -1;

    //May need to be changed into a personallised initialisation function eventually
    public void Start()
    {

        gameController = controller.GetComponent<GameController>();

        if (equippedWeapons.Count != 0)
        {
            foreach (GameObject weapon in equippedWeapons)
            {
                WeaponController weaponController = weapon.GetComponent<WeaponController>();
                foreach (GameObject ability in weaponController.possibleAttacks)
                {
                    possibleAttacks.Add(ability.GetComponent<IFunction>());
                }
            }
        }
        else
        {
            print(name + " has no weapons equipped!");
        }

        //Just for testing: equips the first 4 attacks
        equippedAbilities[0][0] = possibleAttacks[0];
        equippedAbilities[0][1] = possibleAttacks[1];
        equippedAbilities[0][2] = possibleAttacks[2];
        equippedAbilities[0][3] = possibleAttacks[3];

        health = maxHealth;
        stamina = maxStamina;

    }
    public void TakeDamage(float damage) {
        if (!dead)
        {
            if (shielder == -1)
            {
                health -= damage;
                if (health <= 0)
                {
                    //Adds negative health to damage to prevent scaling below going backwards off healthbar
                    damage += health;
                    health = 0;
                    Dead();
                }
                healthBar.transform.localScale -= new Vector3(damage / maxHealth, 0f, 0f);
            }

            else
            {
                //Then the shielders shield needs to take damage
                //Needs to be updated... (shield damage will be stored in weapon script probably)
                //Huskarl huskarlScript = gameController.friendlyParty[shielder].GetComponent<Huskarl>();
                //if (!huskarlScript.TakeShieldDamage(damage))
                //    shielder = -1;
            }
        }
    }

    public void ReductStamina(float amount)
    {
        stamina -= amount;
        staminaBar.transform.localScale -= new Vector3(amount / maxStamina, 0f, 0f);
        if(stamina < 0)
        {
            print("ERROR: Attack stamina caused character: " + name + "'s stamina to drop below 0!");
        }
    }

    public void InitialiseHealthBar(GameObject healthBarPrefab, Vector2 position)
    {
        
        healthBarPrefab.transform.position = new Vector2(position.x, position.y);

        healthBar = healthBarPrefab.transform.GetChild(2).gameObject;

        staminaBar = healthBarPrefab.transform.GetChild(3).gameObject;

    }

    public void Heal(float amount) {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void EquipAttacks(IFunction attack, int pos)
    {
        equippedAbilities[0][pos] = attack;       
    }

    public IFunction[] GetAbilities(int id)
    {
        return equippedAbilities[id];
    }
    
    public void RechargeStamina()
    {
        stamina += rechargeStamina;
        float tempStamina = rechargeStamina;
        if (stamina > maxStamina)
            tempStamina -= stamina - maxStamina;
            stamina = maxStamina;
        
        staminaBar.transform.localScale += new Vector3(tempStamina / maxStamina, 0f, 0f);

        if (staminaBar == null)
            print("ERROR: Character " + name + " doesn't have a staminaBar initialised!");
    }

    private List<T> CombineLists<T>(List<T> list1, List<T> list2)
    {
        List<T> tempList = list1;
        foreach (T element in list2)
        {
            tempList.Add(element);
        }
        return tempList;
    }

    public void Dead()
    {
        dead = true;
        GameObject cross = Instantiate(deadCross, healthBar.transform.parent);
        cross.transform.position += new Vector3(0f, 0.1f, 0f);
        Destroy(staminaBar);
        Destroy(healthBar);
        Destroy(gameObject.GetComponent<SpriteRenderer>());
        
    }
}

