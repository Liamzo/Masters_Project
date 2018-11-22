using UnityEngine;
using System.Collections;

public class WolfBase : EnemyBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        objectName = "Wolf";
        description = "A fast moving, frothy mouthed beast. Looks like it moves quick.\n" +
            "+1 movement";
        blocks = true;
    }


    public override void Turn() {
        // Will only take turn if has been spotted by the player

        if (GameManager.mapManager.TileExplored(x, y)) {
            // Can move twice, but will only attack once
            for (int i = 0; i < 2; i++) {
                if (DistanceTo(player) >= 2) {
                    MoveTowards(player.x, player.y);
                } else if (player.healthCur > 0) {
                    AttackTarget(player);
                    break;
                }
            }
        }
    }
}
