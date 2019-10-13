using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill {
    private int damage;
    public Heal(Unit unit) :
        base(unit, "Heal", 18, TARGETS.ALLIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // auto hit
        // deal negative damage as needed equivalent to 20% maxhp
        damage += thisUnit.battleManager.targetUnit.DealDamage((int)(thisUnit.battleManager.targetUnit.MaxHP * -0.2f), "Heal");
        thisUnit.battleManager.BattleLog += thisUnit.name + "->" + name + "->" + thisUnit.battleManager.targetUnit.name + " (" + damage + ")";
    }
}