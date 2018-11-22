using UnityEngine;
using System.Collections;

public class ShieldMed : EquipmentBase {

    void Start() {
        objectName = "Silver Shield";

        equipmentSlot = 1;

        attackBonus = 0;
        defenseBonus = 2;
        healthBonus = 0;

        description = "A " + objectName + "\n" +
            "Slot: Off Hand\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
