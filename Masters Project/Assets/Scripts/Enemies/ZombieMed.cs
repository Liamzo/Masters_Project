using UnityEngine;
using System.Collections;

public class ZombieMed : ZombieBase {

    // Use this for initialization
    protected override void Start() {
        base.Start();

        HealthMax = 22;
        healthCur = HealthMax;
        Attack = 6;
        Defense = 3;
    }
}
