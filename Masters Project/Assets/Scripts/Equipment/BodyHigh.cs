using UnityEngine;
using System.Collections;

public class BodyHigh : EquipmentBase {

    void Start() {
        objectName = "Gold Plate";

        equipmentSlot = 3;

        attackBonus = 0;
        defenseBonus = 2;
        healthBonus = 15;

        description = "A " + objectName + "\n" +
            "Slot: Body\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
