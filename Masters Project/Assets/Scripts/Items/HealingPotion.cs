using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : ItemBase {

    void Start() {
        objectName = "Healing Potion";
        description = "Heals for 30% max health.";
    }

    public override void Use () {
        Player player = GameManager.gameManager.player;

        if (player.healthCur >= player.HealthMax) {
            GameManager.gameManager.NewMessage("You are already at full health");
            return;
        }

        int healAmount = (int)(GameManager.gameManager.player.HealthMax / 3);

        if (healAmount > (player.HealthMax - player.healthCur)) {
            healAmount = (player.HealthMax - player.healthCur);
        }

        player.Heal(healAmount);
        GameManager.gameManager.NewMessage("You quaff the Healing Potion. You recover " + healAmount + " health");

        player.RemoveItem(this.gameObject);
    }
}
