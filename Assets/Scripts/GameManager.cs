using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    //UI
    [SerializeField] public Text txt_message;
    [SerializeField] public Text txt_PlayerHp;
    [SerializeField] public Text txt_EnemyHp;
    public bool buttonPressed = false;

    //Canvases and default buttons
    [SerializeField] public Canvas combatCanvas;
    [SerializeField] private Canvas searchCanvas;
    [SerializeField] public Canvas itemCanvas;
    [SerializeField] private Canvas endGameCanvas;
    [SerializeField] public Button btn_attack;
    [SerializeField] private Button btn_fight;
    [SerializeField] public Button btn_hpPot;
    [SerializeField] private Button btn_mainMenu;
    
    //Enemy
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject spawnLocation;

    //Player
    public Player player;   //Reference to the player character
    public bool wizardDefeated = false;

    //Levels
    public int currentFight = 0;        //Fight the player is currently in
    private const int bossRoom = 9;     //Room containing the boss (10th room)

    //Makes sure there's only one instance of GameManager
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        txt_PlayerHp.text = "HP: " + player.Hp + " / " + player.MaxHp;
        StartCoroutine(MainCoroutine());
    }

    //Coroutine loops until game ends (Player dies or final boss is defeated)
    IEnumerator MainCoroutine()
    {
        while (player.Hp > 0 && !wizardDefeated)
        {
            yield return StartCoroutine(BattleCoroutine());     //Start new room battle  
            currentFight++;
        }
        if (wizardDefeated)                                      //If final boss is defeated
            txt_message.text = "Congratulations, you have defeated the wizard! \nThe village is saved!";

        else                                                    //If the player dies
            txt_message.text = "You have died!";
        yield return StartCoroutine(WaitForKey());                    //Wait for keypress
        EnableCanvas(endGameCanvas, btn_mainMenu);                    //Return to main menu?
    }

    //Coroutine loops until player or current enemy dies
    IEnumerator BattleCoroutine()
    {
        Enemy enemy = SpawnEnemy();                         //Spawn a random enemy
        enemy.Target = player;
        enemy.SetStats(player.Lvl);                         //Scale enemy's stats to player's level
        txt_EnemyHp.text = "HP: " + enemy.Hp + " / " + enemy.MaxHp;
        player.Target = enemy;

        txt_message.text = "You encounter a " + enemy.CharacterName + "!";
        yield return StartCoroutine(WaitForKey());                //Wait for keypress to continue
        
        while (player.Hp > 0 && enemy.Hp > 0)               //If player and enemy are both alive
        {
            player.Upkeep();                                //Update player's buffs and stop defending
            txt_message.text = "What will you do?";         //Obtain player's choice of action
            EnableCanvas(combatCanvas, btn_attack);
            yield return StartCoroutine(WaitForButton());   //Wait for player to press an on-screen button         
            
            DisableCanvas(combatCanvas);
            yield return StartCoroutine(WaitForKey());      //Give time to display action result message      
            
            if (enemy.Hp > 0)                               //If still alive, enemy performs its turn
            {
                enemy.TakeAction();
                yield return StartCoroutine(WaitForKey());        
            }
        }
        if(player.Hp > 0)                                   //If player defeats the enemy
        {
            txt_message.text = player.CharacterName + " defeated " + enemy.CharacterName + "!"; //Display vicotry message         
            yield return StartCoroutine(WaitForKey());

            player.Exp += enemy.Exp;                        //Reward player with exp
            if ((player.RequiredExp - player.Exp) <= 0)     //Check if player levels up
            {
                txt_message.text = player.CharacterName + " gains " + enemy.Exp + " experience \n" + player.CharacterName + " levels up!";
                yield return StartCoroutine(WaitForKey());
                player.LevelUp();                           //Increase character's level and stats
                txt_message.text = player.CharacterName + " is now level " + player.Lvl + "! \nMaximum HP and attack power increased!";
                yield return StartCoroutine(WaitForKey());
            }
            else
            {
                txt_message.text = player.CharacterName + " gains " + enemy.Exp + " experience \nExperience required for next level: " + (player.RequiredExp - player.Exp);
                yield return StartCoroutine(WaitForKey());
            }
            Destroy(enemy.gameObject);                      //Destroy enemy 
            txt_EnemyHp.text = "";                          //Hide the enemy's HP

            //If we just defeated a regular mob, player can search the room
            if (!wizardDefeated)
            {
                txt_message.text = "You hear something in the next room. Will you fight or stay and search this room?";
                EnableCanvas(searchCanvas, btn_fight);
                yield return StartCoroutine(WaitForButton());

                DisableCanvas(searchCanvas);
                yield return StartCoroutine(WaitForKey());      //Give time to display action result message  
            }
        }
    }

    //Instantiates the enemy game object in the world and returns it
    private Enemy SpawnEnemy()
    {
        if (currentFight < bossRoom)   //First 9 rooms spawn regular mobs
        {
            int enemySelect = (UnityEngine.Random.Range(0, 5)); //Select a random enemy to spawn
            GameObject opponent = Instantiate(enemyPrefabs[enemySelect], spawnLocation.transform.position, spawnLocation.transform.rotation);
            return opponent.GetComponentInChildren<Enemy>();
        }
        else                    //Tenth room contains final wizard boss
        {
            GameObject opponent = Instantiate(enemyPrefabs[5], spawnLocation.transform.position, spawnLocation.transform.rotation);
            return opponent.GetComponentInChildren<Enemy>();
        }
    }

    public void EnableCanvas(Canvas canvas, Button defaultBtn)
    {
        canvas.enabled = true;
        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            btn.enabled = true;
        }
        //Select first button (for keyboard/gamepad)
        EventSystem.current.SetSelectedGameObject(defaultBtn.gameObject, null);
    }

    public void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            btn.enabled = false;
        }
        //Reset currently selected button
        EventSystem.current.SetSelectedGameObject(null);
    }

    //Coroutine waits for any key/mouse press
    IEnumerator WaitForKey()
    {
        bool unpressed = false;
        while(true)
        {
            if (!Input.anyKeyDown)      //Make sure key press doesn't trigger twice in one frame
            {
                unpressed = true;
                yield return null;
            }
            else if(unpressed)      
                break;
            else
                yield return null;
        }
    }

    //Coroutine waits for a button to be pressed on screen
    IEnumerator WaitForButton()
    {
        while (!buttonPressed)
        {
            yield return null;
        }
        buttonPressed = false;
    }
}
