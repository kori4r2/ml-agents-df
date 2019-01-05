using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

// These enums make the observation and action vectors somewhat readable
public enum DECISION {
    AttackEnemy1 = 0,
    AttackEnemy2,
    DoubleEnemy1,
    DoubleEnemy2,
    PoisonEnemy1,
    PoisonEnemy2,
    StunEnemy1,
    StunEnemy2,
    BlindEnemy1,
    BlindEnemy2,
    BlockSelf,
    HealSelf,
    HealAlly,
    Length
};

public enum OBSERVATION {
    TurnCount = 0,
    LastAtkHit,
    DoubleCooldown,
    PoisonCooldown,
    StunCooldown,
    BlindCooldown,
    BlockCooldown,
    HealCooldown,
    Enemy1HP,
    Enemy1Poisoned,
    Enemy1Stunned,
    Enemy1Blinded,
    Enemy1DefUp,
    Enemy2HP,
    Enemy2Poisoned,
    Enemy2Stunned,
    Enemy2Blinded,
    Enemy2DefUp,
    SelfHP,
    SelfPoisoned,
    SelfStunned,
    SelfBlinded,
    SelfDefUp,
    AllyHP,
    AllyPoisoned,
    AllyStunned,
    AllyBlinded,
    AllyDefUp,
    Length
};


public class UnitAgent : Agent {
    public enum TYPEAGENT {
        Individualistic,
        Cooperative
    };
    public TYPEAGENT type;
    Unit self, ally = null, enemy1 = null, enemy2 = null, aux;

	// Use this for initialization
	void Start () {
        self = gameObject.GetComponent<Unit>();
        GameObject[] units = GameObject.FindGameObjectsWithTag("Player");
        aux = units[0].GetComponent<Unit>();
        for (int i = 0; i < units.Length; i++) {
            if (aux.team.Equals(self.team)) {
                ally = aux;
            }else {
                if (enemy1 == null)
                    enemy1 = aux;
                else
                    enemy2 = aux;
            }
            aux = (i + 1 < units.Length) ? units[i + 1].GetComponent<Unit>() : null;
        }
	}

    public override void CollectObservations() {
        // Get number of current turn (allows the brain to notice when they have been stunned)
        AddVectorObs((BattleManager.turnCounter - 0.0f)/(200.0f));
        // Adds if the last attack hit
        AddVectorObs((self.lastAtkHit)? 1.0f: 0.0f);
        // Get cooldown of all skills
        Skill skill = self.skillList.Find(skll => skll.name == "Double");
        if (!skill.Available) {
            SetActionMask((int)DECISION.DoubleEnemy1);
            SetActionMask((int)DECISION.DoubleEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f)/(skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Poison");
        if (!skill.Available) {
            SetActionMask((int)DECISION.PoisonEnemy1);
            SetActionMask((int)DECISION.PoisonEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Stun");
        if (!skill.Available) {
            SetActionMask((int)DECISION.StunEnemy1);
            SetActionMask((int)DECISION.StunEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Blind");
        if (!skill.Available) {
            SetActionMask((int)DECISION.BlindEnemy1);
            SetActionMask((int)DECISION.BlindEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Block");
        if (!skill.Available)
            SetActionMask((int)DECISION.BlockSelf);
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Heal");
        if (!skill.Available) {
            SetActionMask((int)DECISION.HealAlly);
            SetActionMask((int)DECISION.HealSelf);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        // Get HP of enemy 1
        AddVectorObs((enemy1.CurHP - 0.0f)/(enemy1.MaxHP - 0.0f));
        if(enemy1.CurHP <= 0) {
            SetActionMask((int)DECISION.AttackEnemy1);
            SetActionMask((int)DECISION.DoubleEnemy1);
            SetActionMask((int)DECISION.PoisonEnemy1);
            SetActionMask((int)DECISION.StunEnemy1);
            SetActionMask((int)DECISION.BlindEnemy1);
        }
        // Get how many turns are left on status effects afflicting enemy 1
        StatusEffect status = enemy1.effects.Find(effect => effect.text == "Poisoned");
        AddVectorObs((status==null? 0.0f: (status.TurnsLeft - 0.0f)) / (status==null? 1.0f: (status.duration - 0.0f)));
        status = enemy1.effects.Find(effect => effect.text == "Stunned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy1.effects.Find(effect => effect.text == "Blinded");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy1.effects.Find(effect => effect.text == "DefUp");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        // Get HP of enemy 2
        AddVectorObs((enemy2.CurHP - 0.0f) / (enemy2.MaxHP - 0.0f));
        if (enemy2.CurHP <= 0) {
            SetActionMask((int)DECISION.AttackEnemy2);
            SetActionMask((int)DECISION.DoubleEnemy2);
            SetActionMask((int)DECISION.PoisonEnemy2);
            SetActionMask((int)DECISION.StunEnemy2);
            SetActionMask((int)DECISION.BlindEnemy2);
        }
        // Get how many turns are left on status effects afflicting enemy 2
        status = enemy2.effects.Find(effect => effect.text == "Poisoned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy2.effects.Find(effect => effect.text == "Stunned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy2.effects.Find(effect => effect.text == "Blinded");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy2.effects.Find(effect => effect.text == "DefUp");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        // Get HP of self
        AddVectorObs((self.CurHP - 0.0f) / (self.MaxHP - 0.0f));
        // Get how many turns are left on status effects afflicting self
        status = self.effects.Find(effect => effect.text == "Poisoned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = self.effects.Find(effect => effect.text == "Stunned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = self.effects.Find(effect => effect.text == "Blinded");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = self.effects.Find(effect => effect.text == "DefUp");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        if (type == TYPEAGENT.Cooperative) {
            // Get HP of ally
            AddVectorObs((ally.CurHP - 0.0f) / (ally.MaxHP - 0.0f));
            if (ally.CurHP <= 0)
                SetActionMask((int)DECISION.HealAlly);
            // Get how many turns are left on status effects afflicting ally
            status = ally.effects.Find(effect => effect.text == "Poisoned");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
            status = ally.effects.Find(effect => effect.text == "Stunned");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
            status = ally.effects.Find(effect => effect.text == "Blinded");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
            status = ally.effects.Find(effect => effect.text == "DefUp");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        } else {
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        DECISION decision = (DECISION)vectorAction[0];
        switch (decision) {
            case DECISION.AttackEnemy1:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Attack");
                BattleManager.targetUnit = enemy1;
                break;
            case DECISION.AttackEnemy2:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Attack");
                BattleManager.targetUnit = enemy2;
                break;
            case DECISION.DoubleEnemy1:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Double");
                BattleManager.targetUnit = enemy1;
                break;
            case DECISION.DoubleEnemy2:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Double");
                BattleManager.targetUnit = enemy2;
                break;
            case DECISION.PoisonEnemy1:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Poison");
                BattleManager.targetUnit = enemy1;
                break;
            case DECISION.PoisonEnemy2:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Poison");
                BattleManager.targetUnit = enemy2;
                break;
            case DECISION.StunEnemy1:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Stun");
                BattleManager.targetUnit = enemy1;
                break;
            case DECISION.StunEnemy2:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Stun");
                BattleManager.targetUnit = enemy2;
                break;
            case DECISION.BlindEnemy1:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Blind");
                BattleManager.targetUnit = enemy1;
                break;
            case DECISION.BlindEnemy2:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Blind");
                BattleManager.targetUnit = enemy2;
                break;
            case DECISION.BlockSelf:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Block");
                BattleManager.targetUnit = self;
                break;
            case DECISION.HealSelf:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Heal");
                BattleManager.targetUnit = enemy1;
                break;
            case DECISION.HealAlly:
                BattleManager.selectedAction = self.skillList.Find(skill => skill.name == "Heal");
                BattleManager.targetUnit = enemy2;
                break;
        }
        BattleManager.TargetSelected();
    }
}
