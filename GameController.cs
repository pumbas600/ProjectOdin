using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Damage multiplier based on attack type and armour of enemy:
    //Weapon Armour table:
    public float[,] combatRules = new float[5, 5]
    //Armour: None , Low , Med , Hvy , Demon
    /*Blunt*/{{1.3f, 0.7f, 1.0f, 1.2f, 1.0f},
    /*Blade*/ {1.3f, 1.1f, 1.0f, 0.8f, 1.0f},
    /*Range*/ {1.3f, 1.2f, 1.1f, 0.7f, 1.0f},
    /*Magic*/ {1.0f, 1.0f, 1.0f, 1.0f, 1.2f},
    /*Pierc*/ {1.3f, 1.1f, 1.2f, 1.3f, 1.0f}};
    //Access through combatRules[attackType, armourID];

    //Weapon Damage table:
    public float[,] damageRules = new float[7,5]
    //Attack:  strk, slsh, thrt, sevr, ostk 
    /*1HAxe*/{{15f , 25f , 0f  , 30f , 15f },
    /*2HAxe*/ {20f , 35f , 0f  , 45f , 20f },
    /*1HSwd*/ {15f , 15f , 20f , 0f  , 25f },
    /*2HSwd*/ {25f , 25f , 25f , 0f  , 35f },
    /*Daggr*/ {10f , 10f , 25f , 0f  , 5f  },
    /*1HMce*/ {25f , 0f  , 15f , 0f  , 20f },
    /*2HMce*/ {30f , 0f  , 20f , 0f  , 30f }};
    //Access through damageRules[weaponID, abilityID];


    public MultiDimentionalArray[] classWeapons;
    //Access through classWeapons[classID].weapons[weaponID];


    //Array of all the different types of attacks (initialised in Awake);
    public IFunction[] attacks;
    //List of all the abilities each weapon can do
    public List<List<IFunction>> weapons = new List<List<IFunction>>();

    public Character currentPlayerScript;
    public int currentPlayer = -1;
    public int selectedEnemy = -1;
    public GameObject[] friendlyParty = new GameObject[4];
    public GameObject[] enemyParty = new GameObject[4];
    public Character[] playerOrder;

    //Prebabs
    public GameObject selectedHalo;
    public FadingText fadingTextPrefab;

    //Button Controller:

    public ButtonController buttonController;
    //Canvas
    public Canvas topUI;

    public GameObject healthBarParent;
    public GameObject topHealthBar;
    public GameObject bottomHealthBar;
    public GameObject deadCross;
    private List<GameObject> healthBarList = new List<GameObject>();

    public Camera currentCamera;

    public Vector3 HaloOffset;

    public Button confirmButton;
    public Button skipTurnButton;
    public Text turnText;
    
    public bool playersTurn = true;
    public float attackDamage = -1;

    void Start()
    {
        //Gets the gamecontroller script to pass to the abilities scripts.
        GameController gameController = GetComponent<GameController>();

        SetCharacterPositions();
        SetHealthBars();

        //set the halo to the first person who's turn it is.
        DetermineAttackOrder();
        UpdateCurrentPlayer();
        //For testing purposes. Will remove later.
        skipTurnButton.onClick.AddListener(ChangeTurn);

        
        
        foreach(Character character in playerOrder)
        {
            print(character.name + " : " + character.speed);
        }
    }

    public void UpdateCurrentPlayer()
    {
        //Will eventually move to next character in lineup too!
        //Sets the current player script, which will be accessed by other scripts
        currentPlayer++;
        
        if (currentPlayer == playerOrder.Length)
            currentPlayer = 0;

        currentPlayerScript = playerOrder[currentPlayer];
        if (currentPlayerScript.dead)
        {
            UpdateCurrentPlayer();
        }

        //Set playersTurn boolean
        playersTurn = currentPlayerScript.friendly;

        if (playersTurn)
            buttonController.SetPrimaryButtons();
       
        //fills their stamina when its their turn:
        currentPlayerScript.RechargeStamina();
        //Updates the selected halo prefab
        SelectedHalo();
    }


    //Updates the halo which displays what players turn it is.
    public void SelectedHalo()
    {

        Vector2 pos = currentPlayerScript.transform.position;
        //Move the halo to a new position using the halo offsets.
        if(playersTurn)
            selectedHalo.transform.position = new Vector2(pos.x + HaloOffset.x, pos.y + HaloOffset.y);
        else
            selectedHalo.transform.position = new Vector2(pos.x - HaloOffset.x, pos.y + HaloOffset.y);
    }
    
    //Sets the Friendly party composition. (probably will be called just prior to starting the match)
    public void SetFriendlyParty(GameObject[] charcters)
    {
        friendlyParty = charcters;
    }

    //Sets the Enemy party composition. (probably will be called just prior to starting the match)
    public void SetEnemyParty(GameObject[] charcters)
    {
        enemyParty = charcters;
    }

    public void ChangeTurn()
    {
        selectedEnemy = -1;
        //Players turn boolean is set in UpdateCurrentPlayer();
        UpdateCurrentPlayer();

        turnText.text = playersTurn ? "YOUR TURN" : "THEIR TURN";

        confirmButton.interactable = playersTurn;

        Debug.Log("Turn changed to: " + playersTurn);

    }
    //Create a fading text gameobject using a number value:
    public void CreateFadingText(float damage, Transform position)
    {
        string text = damage.ToString();

        //float red = (255 / (0.135f * damage)) + 70;
        float red = -4f * damage + 255;

        //RBGA colour can only be a maximum of 255
        if (red > 255)
            red = 255;
        else if (red < 0)
            red = 0;

        Color colour = new Color(red/255, 0, 0, 1); 
        CreateFadingText(text, position, colour);
  
    }
    //Create a fading text gameobject using a string value:
    public void CreateFadingText(string text, Transform location, Color colour)
    {
        FadingText fadingText = Instantiate(fadingTextPrefab);
        Vector2 screenPosition = currentCamera.WorldToScreenPoint(new Vector2(location.position.x + Random.Range(-0.5f,0.5f), location.position.y + Random.Range(1.2f, 1.7f)));
        

        fadingText.transform.SetParent(topUI.transform, false);
        fadingText.transform.position = screenPosition;
        //Adds a random rotation:
        fadingText.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-8.5f, 8.5f));
        fadingText.SetText(text, colour);
    }

    public void SetHealthBars()
    {
        Vector2 initialPos = new Vector2(11.2f, 7.5f);

        //Initialises the friendly healthbars
        InitialiseHealthBars(friendlyParty, new Vector2(-initialPos.x, initialPos.y));

        //Initialises the enemy healthbars
        InitialiseHealthBars(enemyParty, initialPos);
    }

    private void InitialiseHealthBars(GameObject[] array, Vector2 initialPos)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Character script = array[i].GetComponent<Character>();
            GameObject instantiatedHealthBar;
            if (i == 0)
            {
                instantiatedHealthBar = Instantiate(topHealthBar, healthBarParent.transform);
                script.InitialiseHealthBar(instantiatedHealthBar, initialPos);
            }
            else
            {
                instantiatedHealthBar = Instantiate(bottomHealthBar, healthBarParent.transform);
                script.InitialiseHealthBar(instantiatedHealthBar, new Vector2(initialPos.x, initialPos.y - 0.9f * i));
            }
            script.deadCross = deadCross;
            healthBarList.Add(instantiatedHealthBar);
        }
    }

    public void DestroyHealthBars()
    {
        //Deletes every healthbar in the healthbar list
        healthBarList.ForEach(x => Destroy(x));
    }

    public void DetermineAttackOrder()
    {
        playerOrder = new Character[friendlyParty.Length + enemyParty.Length];

        for (int i = 0; i < friendlyParty.Length; i++)
        {
            playerOrder[i] = friendlyParty[i].GetComponent<Character>();
        }

        for (int i = 0; i < enemyParty.Length; i++)
        {
            playerOrder[i + friendlyParty.Length] = enemyParty[i].GetComponent<Character>();
        }

        //Sorting algorithm:
        for (int i = 0; i < playerOrder.Length; i++)
        {
            int j = 0;
            while (j < playerOrder.Length - 1)
            {
                if (playerOrder[j].speed >= playerOrder[j+1].speed)
                {
                    j++;
                }
                else
                {
                    Character tempCharacter = playerOrder[j];
                    playerOrder[j] = playerOrder[j + 1];
                    playerOrder[j + 1] = tempCharacter;
                }

            }
        }
    }

    public void SetCharacterPositions()
    {
        for(int i = 0; i < friendlyParty.Length; i++)
        {
            friendlyParty[i].GetComponent<Character>().position = i;
        }
        for (int i = 0; i < enemyParty.Length; i++)
        {
            enemyParty[i].GetComponent<Character>().position = i;
        }
    }
}

