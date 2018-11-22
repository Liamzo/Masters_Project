using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : FighterBase {

    public GameObject playerObject;
    protected Player player;

    public int xp;

    public int stunned = 0;

	// Use this for initialization
	protected virtual void Start () {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
	}

    public virtual void Turn() {
        Debug.Log("Grrr");
    }

    override
    public void AttackTarget(FighterBase fighter) {
        int dam = Attack - fighter.Defense;

        if (dam > 0) {
            fighter.TakeDamage(dam);
            GameManager.gameManager.NewMessage("The " + objectName + " deals " + dam + " damage to " + fighter.objectName);
        } else {
            GameManager.gameManager.NewMessage("The " + objectName + " attacks " + fighter.objectName + " but it has no effect");
        }
    }

    override
    protected void Death() {
        base.Death();
        GameManager.mapManager.EnemyDeath(this.gameObject); // Remove enemy from list of enemies in the map

        GameManager.gameManager.player.GainXP(xp); // Give player xp for defeating enemy

        // If this is the selected enemy, then reset the selected enemy variable so it no longer appears
        if (GameManager.gameManager.selectedEnemyObject == this.gameObject) {
            GameManager.gameManager.selectedEnemyObject = null;
        }

        Destroy(this.gameObject); // Delete enemy from map
    }

    private void OnMouseDown() {
        GameManager.gameManager.NewTarget(this.gameObject);
    }
}
