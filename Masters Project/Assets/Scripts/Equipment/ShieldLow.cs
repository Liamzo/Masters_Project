using UnityEngine;
using System.Collections;

public class ShieldLow : EquipmentBase {

    void Start() {
        objectName = "Copper Shield";

        equipmentSlot = 1;

        attackBonus = 0;
        defenseBonus = 1;
        healthBonus = 0;

        description = "A " + objectName + "\n" +
            "Slot: Off Hand\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
