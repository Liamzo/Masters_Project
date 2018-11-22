using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterBase : BaseObject {

    protected int healthMax;
    public int healthCur;

    protected int defense;
    protected int attack;

    public virtual int Attack {
        get {
            return attack;
        }
        set {
            attack = value;
        }
    }

    public virtual int Defense {
        get {
            return defense;
        }
        set {
            defense = value;
        }
    }

    public virtual int HealthMax {
        get {
            return healthMax;
        }
        set {
            healthMax = value;
        }
    }

    public virtual void TakeDamage(int dam) {
        healthCur -= dam;

        if (healthCur <= 0) {
            Death();
        }
        
    }

    
    public virtual void AttackTarget(FighterBase fighter) {
        int dam = Attack - fighter.Defense;

        if (dam > 0) {
            fighter.TakeDamage(dam);
        }
    }
    
    

    public void Heal (int heal) {
        healthCur += heal;

        if (healthCur > HealthMax) {
            healthCur = HealthMax;
        }
    }

    protected virtual void Death () {
        GameManager.gameManager.NewMessage("The " + objectName + " dies");
    }
}
