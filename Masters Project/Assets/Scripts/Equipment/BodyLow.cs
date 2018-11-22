using UnityEngine;
using System.Collections;

public class BodyLow : EquipmentBase {

    void Start() {
        objectName = "Copper Plate";

        equipmentSlot = 3;

        attackBonus = 0;
        defenseBonus = 1;
        healthBonus = 0;

        description = "A " + objectName + "\n" +
            "Slot: Body\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
