using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Abilities:
    public Abilities slash = new Abilities("Slash", "short sword", 25f, new int[] { 0 }, new int[] { 0 }, "Slash is a light attack which can only be dealt \n from the first position in your lineup \n and applies only to the first positon in the opponents \n lineup", false, 0);
    public Abilities thrust = new Abilities("Thrust", "short sword", 35f, new int[] { 0, 1 }, new int[] { 0, 1 }, "Thrust is a medium attack which can only be dealth \n from the first position in your lineup \n and applies to the first and second positions in the \n opponents lineup", false, 0);
    public Abilities sideSlash = new Abilities("Side Slash", "short sword", 30f, new int[] { 0 }, new int[] {0}, "", false, 0);
    public Abilities overheadSlash = new Abilities("Overhead Slash", "short sword", 40f, new int[] { 0 }, new int[] { 0 }, "", false, 0);

    public Abilities heal = new Abilities("Heal", "bandage", 20f, new int[] { 0, 1, 2, 3 }, new int[] { 0, 1, 2, 3 }, "", true, 1);
  
    public int currentPlayer;
    public int selectedEnemy = -1;
    public GameObject[] friendlyParty;
    public GameObject[] enemyParty;

    public Button skipTurnButton;
    public Text turnText;
    
    public bool playersTurn = true;
    public float attackDamage = -1;

    void Start()
    {

        skipTurnButton.onClick.AddListener(delegate { if (playersTurn) ChangeTurn(); }); 

        for(int i = 0; i < enemyParty.Length; i++) {
            Enemy enemyScript = enemyParty[i].GetComponent<Enemy>();
            enemyScript.id = i;
        }
        //Text strikeText = strikeButton.GetComponentInChildren<Text>();
        //strikeText.text = "Test";
    }

    public void SetFriendlyParty(GameObject[] charcters)
    {
        friendlyParty = charcters;
    }

    public void SetEnemyParty(GameObject[] charcters)
    {
        enemyParty = charcters;
    }

    public void ChangeTurn()
    {
        selectedEnemy = -1;
        playersTurn = !playersTurn;
        turnText.text = playersTurn ? "YOUR TURN" : "THEIR TURN";
        
        Debug.Log("Turn changed to: " + playersTurn);

    }

    

    public void Strike() {
        Player playerScript = friendlyParty[currentPlayer].GetComponent<Player>();
        attackDamage = playerScript.damage;
        
    }
}
