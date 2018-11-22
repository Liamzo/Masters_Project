using UnityEngine;
using System.Collections;

public class HelmetMed : EquipmentBase {

    void Start() {
        objectName = "Silver Helmet";

        equipmentSlot = 2;

        attackBonus = 0;
        defenseBonus = 0;
        healthBonus = 20;

        description = "A " + objectName + "\n" +
            "Slot: Head\n" +
            "Gives " + healthBonus + " bonus health\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
