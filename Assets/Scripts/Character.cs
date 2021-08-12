using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private string characterName;   
    private int hp;     //Health
    private int maxHp;  //Maximum HP
    private int atk;    //Attack
    private int exp;    //Experience
    private int lvl;    //Level
    private Character target;  //Target the character is fighting
    private bool defending;     //Is the character currently defending?
    protected GameManager manager;
    protected Animator anim;

    //Properties
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Atk { get => atk; set => atk = value; }
    public int Exp { get => exp; set => exp = value; }
    public int Lvl { get => lvl; set => lvl = value; }
    public Character Target { get => target; set => target = value; }
    public bool Defending { get => defending; set => defending = value; }

    private void Start()
    {
        manager = GameManager.instance; //Cache instance of game manager
        anim = this.GetComponentInParent<Animator>();
    }

    public virtual void Attack()
    {
        if (this.Target.Defending)      //If the target is defending, take no damage
        {
            manager.txt_message.text = CharacterName + " attacks, but " + this.Target.CharacterName + " takes no damage!";
            this.Target.Defending = false;
        }
        else
        {
            int damage = Convert.ToInt32(UnityEngine.Random.Range(this.Lvl, this.Atk)); //Roll for damage
            Debug.Log("Original dmg: " + damage);
            this.Target.TakeDamage(damage); 
            manager.txt_message.text = CharacterName + " attacks " + this.Target.CharacterName + " for " + damage + " damage!";
        }
        anim.SetTrigger("Attack");
    }

    public virtual void Defend()
    {
        this.Defending = true;
        anim.SetBool("Defend", true);
        manager.txt_message.text = CharacterName + " is defending.";
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log("Taken dmg: " + damage);
        this.Hp -= damage;
        anim.SetTrigger("Hit");
        if(this.Hp <= 0)
        {
            this.Die();
        }
    }

    public virtual void Die()
    {
        anim.SetBool("Death", true);
    }
}
