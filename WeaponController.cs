using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private GameController gameController;
    public int weaponID;
    public string weaponName;
    public int hands;

    public List<GameObject> possibleAttacks = new List<GameObject>();

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        foreach (GameObject attack in possibleAttacks)
        {
            Abilities ability = attack.GetComponent<Abilities>();
            ability.value = gameController.damageRules[weaponID, ability.id];
            ability.gameController = gameController;
            ability.weaponName = weaponName;
    
        }
        //possibleAttacks = gameController.weapons[weaponID];            
    }

}
