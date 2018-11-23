using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill {
    public override bool Available {
        get {
            return true;
        }
        protected set {
            available = true;
        }
    }
    public Heal(Unit unit) :
        base(unit, "Heal", 15, TARGETS.ALLIES) {
        Available = true;
    }
    public override void Use() {
        // auto hit
        // deal negative damage as needed equivalent to 20% maxhp
        BattleManager.targetUnit.DealDamage((int)(BattleManager.targetUnit.MaxHP * -0.2f), "Heal");
    }
}