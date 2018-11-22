using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KoboldBase : EnemyBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        objectName = "Kobold";
        description = "A small, dirty humanoid figure. Looks like they hunt in packs.\n" +
            "+1 attack for each adjacent Kobold";
        blocks = true;
    }

    public override int Attack {
        get {
            // Check if any other Kobolds are next to this one,
            // Gain extra attack if so
            int bonus = 0;
            List<GameObject> enemies = GameManager.mapManager.EnemiesInRange(x, y, 1);

            foreach (GameObject enemy in enemies) {
                if (enemy.GetComponent<BaseObject>().objectName.Equals("Kobold")) {
                    bonus++;
                }
            }

            return (attack + bonus);
        }

        set {
            attack = value;
        }
    }

    public override void Turn() {
        // Will only take turn if has been spotted by the player

        if (GameManager.mapManager.TileExplored(x, y)) {
            // Move towards player
            if (DistanceTo(player) >= 2) {
                MoveTowards(player.x, player.y);
            } else if (player.healthCur > 0) {
                AttackTarget(player);
            }
        }
    }
}
