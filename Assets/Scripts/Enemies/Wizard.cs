using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    private const int MAX_HP = 100;
    private const int ATK = 20;

    public Wizard()
    {
        this.CharacterName = "Wizard";
        //Debug.Log("Is wizard");
    }

    //Wizard's Hp and Atk stats are not influenced by player level
    public override void SetStats(int level)
    {
        base.SetStats(level);
        this.MaxHp = MAX_HP;
        this.Hp = this.MaxHp;
        this.Atk = ATK;
    }

    public override void Die()
    {
        base.Die();
        manager.wizardDefeated = true;
    }
}

