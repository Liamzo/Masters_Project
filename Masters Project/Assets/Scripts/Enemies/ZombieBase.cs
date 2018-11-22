using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBase : EnemyBase {

	// Use this for initialization
	protected override void Start () {
        base.Start();

        objectName = "Zombie";
        description = "A slow moving animated corpse that will chase you when spotted. Looks like it can take a hit.";
        blocks = true;
	}

    
    public override void Turn () {
        // Will only take turn if has been spotted by the player

        if (GameManager.mapManager.TileExplored(x,y)) {
            // Move towards player
            if (DistanceTo(player) >= 2) {
                MoveTowards(player.x, player.y);
            } else if (player.healthCur > 0) {
                AttackTarget(player);
            }
        }
    }
}
