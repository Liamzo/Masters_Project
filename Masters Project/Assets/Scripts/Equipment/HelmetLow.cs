using UnityEngine;
using System.Collections;

public class HelmetLow : EquipmentBase {

    void Start() {
        objectName = "Copper Helmet";

        equipmentSlot = 2;

        attackBonus = 0;
        defenseBonus = 0;
        healthBonus = 10;

        description = "A " + objectName + "\n" +
            "Slot: Head\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
