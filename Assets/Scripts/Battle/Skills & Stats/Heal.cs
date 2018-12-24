using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill {
    private int damage;
    public Heal(Unit unit) :
        base(unit, "Heal", 15, TARGETS.ALLIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // auto hit
        // deal negative damage as needed equivalent to 20% maxhp
        damage += BattleManager.targetUnit.DealDamage((int)(BattleManager.targetUnit.MaxHP * -0.2f), "Heal");
        BattleManager.BattleLog += thisUnit.name + "->" + name + "->" + BattleManager.targetUnit.name + " (" + damage + ")";
    }
}