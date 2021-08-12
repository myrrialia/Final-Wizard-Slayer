using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    protected bool charged = false; //Whether the enemy has prepared a charged attack

    public Enemy()
    {
        this.Lvl = 1;
        this.MaxHp = 10 * this.Lvl;
        this.Hp = this.MaxHp;
        this.Atk = 3 * this.Lvl + 2;
        this.Exp = 4 + this.Lvl;
    }

    public void TakeAction()
    {
        this.Defending = false;         //Stop defending at start of turn
        anim.SetBool("Defend", false);
        if (charged)                    //If charged up, perform a charged attack
        {
            this.ChargedAttack();
        }
        else                            //If not charged up, perform a random action
        {
            int i = UnityEngine.Random.Range(0, 4);
            switch (i)
            {
                case 0:
                case 1:
                    this.Attack();          //Perform regular attack
                    break;
                case 2:
                    this.Defend();          //Defend
                    break;
                case 3:
                    this.charged = true;    //Prep for charged attack
                    anim.SetBool("ChargeUp", true);
                    manager.txt_message.text = CharacterName + " is charging up for a powerful attack!";
                    break;
            }
        }
    }

    public override void Attack()
    {
        if(manager.player.defBoost > 0 && !manager.player.Defending) //Half damage if player used a def potion and isn't defending
        {
            int damage = Convert.ToInt32(UnityEngine.Random.Range(this.Lvl, this.Atk)); //Roll for damage
            Debug.Log("Original dmg: " + damage);
            this.Target.TakeDamage(damage/2);
            manager.txt_message.text = CharacterName + " attacks " + this.Target.CharacterName + " for " + damage/2 + " damage!";
        }
        else
        base.Attack();
    }

    private void ChargedAttack()
    {
        anim.SetTrigger("ChargedAttack");
        if (this.Target.Defending)      //If the target is defending, take no damage
        {
            manager.txt_message.text = CharacterName + " unleashes a powerful attack, but " + this.Target.CharacterName + " takes no damage!";
            this.Target.Defending = false;
        }
        else
        {
            int damage = Convert.ToInt32(UnityEngine.Random.Range(this.Lvl, this.Atk)); //Roll for damamge
            Debug.Log("Original dmg: " + damage);
            if (manager.player.defBoost > 0)    ////Regular damage if player used a def potion
            {
                manager.txt_message.text = CharacterName + " unleashes a powerful attack against " + this.Target.CharacterName + " for " + damage + " damage!";
                this.Target.TakeDamage(damage);   
            }
            else
            {
                manager.txt_message.text = CharacterName + " unleashes a powerful attack against " + this.Target.CharacterName + " for " + damage * 2 + " damage!";
                this.Target.TakeDamage(damage * 2);   //Deal double damage
            }
        }
        charged = false;
        anim.SetBool("ChargeUp", false);
    }

    //Scales enemy stats based on player's current level
    public virtual void SetStats(int level)
    {
        this.Lvl = level;
        this.MaxHp = 10 * this.Lvl;
        this.Hp = this.MaxHp;
        this.Atk = 3 * this.Lvl + 2;
        this.Exp = 4 + this.Lvl;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (this.Hp > 0)
            manager.txt_EnemyHp.text = "HP: " + this.Hp + " / " + this.MaxHp;
        else
            manager.txt_EnemyHp.text = "HP: 0 / " + this.MaxHp;
    }
}
