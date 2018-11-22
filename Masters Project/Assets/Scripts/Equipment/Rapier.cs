using UnityEngine;
using System.Collections;

public class Rapier : EquipmentBase {

    void Start() {
        objectName = "Rapier";

        equipmentSlot = 0;

        attackBonus = 3;
        defenseBonus = 1;
        healthBonus = 0;

        description = "A rapier.\n" +
            "Slot: Main Hand\n" +
            "Gives " + attackBonus + " bonus attack\n" +
            "Gives " + defenseBonus + " bonus defense";
    }
}
