using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningScroll : ItemBase {

    int arcLength = 4;
    int arcNumber = 5;

    int damage = 6;

    void Start() {
        objectName = "Lightning Scroll";
        description = "Channels a bolt of lightning that arcs between enemies.";
    }

    public override void Use() {

        Player player = GameManager.gameManager.player;

        // Find closest target to player
        int x = player.x;
        int y = player.y;

        List<GameObject> enemiesInRange = GameManager.mapManager.EnemiesInRange(x, y, arcLength);
        GameObject target;

        if (enemiesInRange.Count < 1) {
            // No targets in range first time, so don't use scroll
            GameManager.gameManager.NewMessage("No enemies in range, you think it wise to save the scroll");
            return;
        } else {
            GameManager.gameManager.NewMessage("The scroll turns to dust as you are momentarily deafend by a massive thunder clap!");
        }


        for (int i = 0; i < arcNumber; i++) {

            if (enemiesInRange.Count < 1) {
                // No targets to arc to
                break;
            } else {
                int rand = Random.Range(0, enemiesInRange.Count);
                target = enemiesInRange[rand];

                FighterBase fb = target.GetComponent<FighterBase>();

                GameManager.gameManager.NewMessage("The " + fb.objectName + " is struck by a bolt of lightning for " + damage + " damage");
                fb.TakeDamage(damage);

                x = fb.x;
                y = fb.y;

                enemiesInRange = GameManager.mapManager.EnemiesInRange(x, y, arcLength);
            }
        }

        player.RemoveItem(this.gameObject);
    }
}
