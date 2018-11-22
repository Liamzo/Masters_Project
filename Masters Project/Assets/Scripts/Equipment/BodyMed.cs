using UnityEngine;
using System.Collections;

public class BodyMed : EquipmentBase {

    void Start() {
        objectName = "Silver Plate";

        equipmentSlot = 3;

        attackBonus = 0;
        defenseBonus = 1;
        healthBonus = 10;

        description = "A " + objectName + "\n" +
            "Slot: Body\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
