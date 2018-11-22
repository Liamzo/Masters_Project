using UnityEngine;
using System.Collections;

public class LongSword : EquipmentBase {
    void Start() {
        objectName = "Long Sword";

        equipmentSlot = 0;

        attackBonus = 4;
        defenseBonus = 0;
        healthBonus = 0;

        description = "A Long sword.\n" +
            "Slot: Main Hand\n" +
            "Gives " + attackBonus + " bonus attack";
    }
}
