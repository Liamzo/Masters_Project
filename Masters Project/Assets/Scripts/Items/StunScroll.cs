using UnityEngine;
using System.Collections;

public class StunScroll : ItemBase {

    int stunPower = 6;
    int range = 10;

    // Use this for initialization
    void Start() {
        objectName = "Stunning Scroll";
        description = "Stuns targeted enemy for " + stunPower + " turns.";
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
            GameManager.gameManager.NewMessage("The enemy is too far away for the scroll to take effect");
            return;
        }

        // Target is in range
        GameManager.gameManager.NewMessage("The scroll disintegrates in your hands as you finish the incantation!");
        GameManager.gameManager.NewMessage("The " + eb.objectName + "'s eyes look vacant as they begin to wobble on their feet");
        eb.stunned += stunPower;

        player.RemoveItem(this.gameObject);
    }
}
