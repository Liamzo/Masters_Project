using UnityEngine;
using System.Collections;

public class EquipmentBase : ItemBase {
    // 0 = main hand
    // 1 = off hand
    // 2 = head
    // 3 = body
    public int equipmentSlot;
    public bool isEqipped;

    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;

    override
    public void Use () {
        if (isEqipped == false) {
            Equip();
        } else {
            Dequip();
        }
    }

    void Equip() {
        GameManager.gameManager.player.EquipItem(this.gameObject);
        isEqipped = true;
    }

    void Dequip () {
        GameManager.gameManager.player.DequipItem(this.gameObject);
        isEqipped = false;
    }
}
