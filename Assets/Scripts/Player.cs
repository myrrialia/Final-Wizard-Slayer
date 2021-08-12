using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{

    private int requiredExp;    //Exp required for next level

    //Experience required for next level
    private const int EXP_REQ_1 = 5;
    private const int EXP_REQ_2 = 10;
    private const int EXP_REQ_3 = 20;
    private const int EXP_REQ_4 = 25;
    private const int EXP_REQ_DEF = 30;

    //Items
    private int hpPotions = 0;      //Number of health potions the player has found
    private int atkPotions = 0;     //Number of attack potions the player has found
    private int defPotions = 0;     //Number of defense potions the player has found
    private int bombs = 0;          //Number of bombs the player has found
    private int atkBoost = 0;       //Turns of double attack power remaining
    public int defBoost = 0;        //Turns of reduced damage remaining
    [SerializeField] private Text txt_HpPots;
    [SerializeField] private Text txt_AtkPots;
    [SerializeField] private Text txt_DefPots;
    [SerializeField] private Text txt_Bombs;
    private const int ATK_BOOST_TURNS = 4;  //Atk potion is active for 3 turns + the turn it was used
    private const int DEF_BOOST_TURNS = 3;  //Def potion is active for 3 turns

    public Player()
    {
        this.CharacterName = "Player";
        this.Lvl = 1;
        this.MaxHp = 20 * this.Lvl;
        this.Hp = this.MaxHp;
        this.Atk = 4 * this.Lvl + 1;
        this.Exp = 0;
        this.RequiredExp = 5;
    }

    public int RequiredExp { get => requiredExp; set => requiredExp = value; }

    public override void Attack()
    {
        if (atkBoost > 0 && !this.Target.Defending)       //If an attack potion is active and the enemy's not defending
        {
            int damage = Convert.ToInt32(UnityEngine.Random.Range(this.Lvl, this.Atk));     //Roll for damage
            Debug.Log("Original dmg: " + damage);
            this.Target.TakeDamage(damage*2);
            manager.txt_message.text = CharacterName + " attacks " + this.Target.CharacterName + " for " + damage*2 + " damage!";
        }
        else                                            //If no attack potion is active (regular damage)
            base.Attack();
        manager.buttonPressed = true;   //Tell the game manager it can continue
    }

    public override void Defend()
    {
        base.Defend();
        manager.buttonPressed = true;   //Tell the game manager it can continue
    }

    public void UseItem()
    {
        manager.DisableCanvas(manager.combatCanvas);                    //Disable action select buttons
        manager.txt_message.text = "Select an item to use.";
        manager.EnableCanvas(manager.itemCanvas, manager.btn_hpPot);    //Enable item select buttons
    }

    public void Fight()
    {
        manager.txt_message.text = "You enter the next room to investigate the sound...";
        manager.buttonPressed = true;
    }

    public void Search()
    {
        //Obtain a random item
        int i = UnityEngine.Random.Range(1, 5);
        switch(i)
        {
            case 1:             //Health potion
                hpPotions++;
                txt_HpPots.text = "Health Potion (" + hpPotions + ")";
                manager.txt_message.text = "You found a health potion! \nDrinking this potion restores half your maximum HP.";
                break;
            case 2:             //Attack potion
                atkPotions++;
                txt_AtkPots.text = "Attack Potion (" + atkPotions + ")";
                manager.txt_message.text = "You found an attack potion! \nDrinking this potion doubles your attack power for the next 3 turns.";
                break;
            case 3:             //Defense potion
                defPotions++;
                txt_DefPots.text = "Defense Potion (" + defPotions + ")";
                manager.txt_message.text = "You found a defense potion! \nDrinking this potion reduces your damage taken for the next 3 turns.";
                break;
            case 4:             //Bomb
                bombs++;
                txt_Bombs.text = "Bomb (" + bombs + ")";
                manager.txt_message.text = "You found a bomb! Throwing this at an enemy will deal heavy damage.";
                break;
        }
        manager.currentFight++;     //Get one room closer to the boss if player chooses to search instead of fight
        manager.buttonPressed = true;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (this.Hp > 0)
            manager.txt_PlayerHp.text = "HP: " + this.Hp + " / " + this.MaxHp;
        else
            manager.txt_PlayerHp.text = "HP: 0 / " + this.MaxHp;
    }

    public void LevelUp()
    {
        this.Lvl++;
        this.MaxHp = 20 * this.Lvl;
        this.Hp = this.MaxHp;                   //Player returns to full health after level up
        manager.txt_PlayerHp.text = "HP: " + this.Hp + " / " + this.MaxHp;
        this.Atk = 4 * this.Lvl + 1;
        this.Exp = this.Exp - this.RequiredExp; //Extra experience carries over to next level
        switch(this.Lvl)
        {
            case 1:
                this.RequiredExp = EXP_REQ_1;
                break;
            case 2:
                this.RequiredExp = EXP_REQ_2;
                break;
            case 3:
                this.RequiredExp = EXP_REQ_3;
                break;
            case 4:
                this.RequiredExp = EXP_REQ_4;
                break;
            default:
                this.RequiredExp = EXP_REQ_DEF;
                break;
        }
    }

    //Called at the beginning of the player's turn
    public void Upkeep()
    {
        Defending = false;  //Stop defending
        anim.SetBool("Defend", false);
        if(atkBoost > 0)
            atkBoost--;     //Decrement remaining turns of increased attack
        if(defBoost > 0)
            defBoost--;     //Decrement remaining turns of decreased damage
    }

    public void UseHpPotion()
    {
        if(hpPotions <= 0)  //Item missing
            MissingItem();
        else                //Item found
        {
            hpPotions--;
            txt_HpPots.text = "Health Potion (" + hpPotions + ")";
            if (this.Hp + (this.MaxHp / 2) > MaxHp)
                this.Hp = MaxHp;            //Make sure current HP does not surpass max Hp
            else
                this.Hp += (MaxHp / 2);
            manager.txt_PlayerHp.text = "HP: " + this.Hp + " / " + this.MaxHp;
            manager.txt_message.text = "You restore some health!";
            manager.DisableCanvas(manager.itemCanvas);                      //Disable item select buttons
            manager.buttonPressed = true;   //Tell the game manager it can continue
        }
    }

    public void UseAtkPotion()
    {
        if(atkPotions <= 0) //Item missing
            MissingItem();
        else                //Item found
        {
            atkPotions--;
            txt_AtkPots.text = "Attack Potion (" + atkPotions + ")";
            atkBoost = ATK_BOOST_TURNS;
            manager.txt_message.text = "Your attack is doubled for the next 3 turns!";
            manager.DisableCanvas(manager.itemCanvas);                      //Disable item select buttons
            manager.buttonPressed = true;   //Tell the game manager it can continue
        }
    }

    public void UseDefPotion()
    {
        if (defPotions <= 0) //Item missing
            MissingItem();
        else                //Item found
        {
            defPotions--;
            txt_DefPots.text = "Defense Potion (" + defPotions + ")";
            defBoost = DEF_BOOST_TURNS;
            manager.txt_message.text = "You receive reduced damage for the next 3 turns!";
            manager.DisableCanvas(manager.itemCanvas);                      //Disable item select buttons
            manager.buttonPressed = true;   //Tell the game manager it can continue
        }
    }

    public void UseBomb()
    {
        if (bombs <= 0) //Item missing
            MissingItem();
        else                //Item found
        {
            bombs--;
            txt_Bombs.text = "Bomb (" + bombs + ")";
            Target.TakeDamage(Target.MaxHp / 2);    //Deal half the enemy's max hp as damage
            manager.txt_message.text = "You throw the bomb at " + Target.CharacterName + " and deal " + Target.MaxHp/2 + " damage!";
            manager.DisableCanvas(manager.itemCanvas);                      //Disable item select buttons
            manager.buttonPressed = true;   //Tell the game manager it can continue
        }
    }
    
    //If the player tries to use an item they have 0 of
    private void MissingItem()
    {
        manager.DisableCanvas(manager.itemCanvas);                      //Disable item select buttons
        manager.txt_message.text = "You aren't carrying that item!";
        manager.EnableCanvas(manager.combatCanvas, manager.btn_attack); //Enable action select buttons
    }
}
