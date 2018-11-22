using UnityEngine;
using System.Collections;

public class FireballScroll : ItemBase {

    int range = 6;
    int damage = 15;

    // Use this for initialization
    void Start() {
        objectName = "Fireball Scroll";
        description = "Hurls a massive ball of fire towards the targeted enemy.";
    }

    public override void Use() {
        Player player = GameManager.gameManager.player;

        GameObject target = GameManager.gameManager.selectedEnemyObject;

        if (target == null) {
            GameManager.gameManager.NewMessage("It would be best to find a target before reading this scroll");
            return;
        }
        EnemyBase eb = target.GetComponent<EnemyBase>();
        if (player.DistanceTo(eb) > range) {
            GameManager.gameManager.NewMessage("The enemy is too far away for the fireball to reach");
            return;
        }

        // Target is in range
        GameManager.gameManager.NewMessage("The scroll bursts into flames as a giant flaming ball hurls forward!");
        GameManager.gameManager.NewMessage("The " + eb.objectName + " is struck by the fireball for " + damage + " damage");
        eb.TakeDamage(damage);

        player.RemoveItem(this.gameObject);
    }
}