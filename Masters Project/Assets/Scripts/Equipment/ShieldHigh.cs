using UnityEngine;
using System.Collections;

public class ShieldHigh : EquipmentBase {

    void Start() {
        objectName = "Gold Shield";

        equipmentSlot = 1;

        attackBonus = 1;
        defenseBonus = 3;
        healthBonus = 0;

        description = "A " + objectName + "\n" +
            "Slot: Off Hand\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
