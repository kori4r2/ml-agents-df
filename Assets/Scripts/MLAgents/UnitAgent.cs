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
    public BattleManager battleManager;
    public enum TYPEAGENT {
        Individualistic,
        Cooperative
    };
    public TYPEAGENT type;
    public int CurrentHealth{
        get{ return self.CurHP; }
    }
    [SerializeField] private Unit self;
    [SerializeField] private Unit ally;
    [SerializeField] private Unit enemy1;
    [SerializeField] private Unit enemy2;
    [SerializeField] private DecisionLog decisionLog = null;
    private int myLogIndex = -1;
    
	// Use this for initialization
	void Awake () {
        // Debug.Log(self.name + " has " + ally.name + " as ally, " + enemy1.name + " as enemy1 and " + enemy2.name + " as enemy2");
        if(decisionLog && brain.brainType == BrainType.Internal){
            myLogIndex = decisionLog.AddAgent(this);
        }
	}

    public override void CollectObservations() {
        // Get number of current turn (allows the brain to notice when they have been stunned)
        AddVectorObs((battleManager.turnCounter - 0.0f)/(200.0f));
        // Adds if the last attack hit
        AddVectorObs((self.lastAtkHit)? 1.0f: 0.0f);
        // Get cooldown of all skills
        Skill skill = self.skillList.Find(skll => skll.name == "Double");
        if (!skill.Available) {
            // Debug.Log("Double is on cooldown");
            SetActionMask((int)DECISION.DoubleEnemy1);
            SetActionMask((int)DECISION.DoubleEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f)/(skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Poison");
        if (!skill.Available) {
            // Debug.Log("Poison is on cooldown");
            SetActionMask((int)DECISION.PoisonEnemy1);
            SetActionMask((int)DECISION.PoisonEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Stun");
        if (!skill.Available) {
            // Debug.Log("Stun is on cooldown");
            SetActionMask((int)DECISION.StunEnemy1);
            SetActionMask((int)DECISION.StunEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Blind");
        if (!skill.Available) {
            // Debug.Log("Blind is on cooldown");
            SetActionMask((int)DECISION.BlindEnemy1);
            SetActionMask((int)DECISION.BlindEnemy2);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Block");
        if (!skill.Available){
            // Debug.Log("Block is on cooldown");
            SetActionMask((int)DECISION.BlockSelf);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));
        skill = self.skillList.Find(skll => skll.name == "Heal");
        if (!skill.Available) {
            // Debug.Log("Heal is on cooldown");
            SetActionMask((int)DECISION.HealAlly);
            SetActionMask((int)DECISION.HealSelf);
        }
        AddVectorObs((skill.CurCD - 0.0f) / (skill.cooldown - 0.0f));

        // Get HP of enemy 1
        AddVectorObs((enemy1.CurHP - 0.0f)/(enemy1.MaxHP - 0.0f));
        if(enemy1.CurHP <= 0) {
            // Debug.Log("enemy1(" + enemy1.name + ") is dead");
            SetActionMask((int)DECISION.AttackEnemy1);
            SetActionMask((int)DECISION.DoubleEnemy1);
            SetActionMask((int)DECISION.PoisonEnemy1);
            SetActionMask((int)DECISION.StunEnemy1);
            SetActionMask((int)DECISION.BlindEnemy1);
        }
        // Get how many turns are left on status effects afflicting enemy 1
        StatusEffect status = enemy1.effects.FindLast(effect => effect.text == "Poisoned");
        AddVectorObs((status==null? 0.0f: (status.TurnsLeft - 0.0f)) / (status==null? 1.0f: (status.duration - 0.0f)));
        status = enemy1.effects.FindLast(effect => effect.text == "Stunned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy1.effects.FindLast(effect => effect.text == "Blinded");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy1.effects.FindLast(effect => effect.text == "DefUp");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));

        // Get HP of enemy 2
        AddVectorObs((enemy2.CurHP - 0.0f) / (enemy2.MaxHP - 0.0f));
        if (enemy2.CurHP <= 0) {
            // Debug.Log("enemy2(" + enemy2.name + ") is dead");
            SetActionMask((int)DECISION.AttackEnemy2);
            SetActionMask((int)DECISION.DoubleEnemy2);
            SetActionMask((int)DECISION.PoisonEnemy2);
            SetActionMask((int)DECISION.StunEnemy2);
            SetActionMask((int)DECISION.BlindEnemy2);
        }
        // Get how many turns are left on status effects afflicting enemy 2
        status = enemy2.effects.FindLast(effect => effect.text == "Poisoned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy2.effects.FindLast(effect => effect.text == "Stunned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy2.effects.FindLast(effect => effect.text == "Blinded");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = enemy2.effects.FindLast(effect => effect.text == "DefUp");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));

        // Get HP of self
        AddVectorObs((self.CurHP - 0.0f) / (self.MaxHP - 0.0f));
        // Get how many turns are left on status effects afflicting self
        status = self.effects.FindLast(effect => effect.text == "Poisoned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = self.effects.FindLast(effect => effect.text == "Stunned");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = self.effects.FindLast(effect => effect.text == "Blinded");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        status = self.effects.FindLast(effect => effect.text == "DefUp");
        AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));

        if (type == TYPEAGENT.Cooperative) {
            // Get HP of ally
            AddVectorObs((ally.CurHP - 0.0f) / (ally.MaxHP - 0.0f));
            if (ally.CurHP <= 0){
                // Debug.Log("ally(" + ally.name + ") is dead");
                SetActionMask((int)DECISION.HealAlly);
            }
            // Get how many turns are left on status effects afflicting ally
            status = ally.effects.FindLast(effect => effect.text == "Poisoned");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
            status = ally.effects.FindLast(effect => effect.text == "Stunned");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
            status = ally.effects.FindLast(effect => effect.text == "Blinded");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
            status = ally.effects.FindLast(effect => effect.text == "DefUp");
            AddVectorObs((status == null ? 0.0f : (status.TurnsLeft - 0.0f)) / (status == null ? 1.0f : (status.duration - 0.0f)));
        } else {
            if (ally.CurHP <= 0){
                // Debug.Log("ally(" + ally.name + ") is dead");
                SetActionMask((int)DECISION.HealAlly);
            }
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
            AddVectorObs(0.0f);
        }
        // Debug.Log("Calculated reward at obs= " + GetCumulativeReward());
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        DECISION decision = (DECISION)vectorAction[0];
        switch (decision) {
            case DECISION.AttackEnemy1:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Attack");
                battleManager.targetUnit = enemy1;
                break;
            case DECISION.AttackEnemy2:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Attack");
                battleManager.targetUnit = enemy2;
                break;
            case DECISION.DoubleEnemy1:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Double");
                battleManager.targetUnit = enemy1;
                break;
            case DECISION.DoubleEnemy2:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Double");
                battleManager.targetUnit = enemy2;
                break;
            case DECISION.PoisonEnemy1:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Poison");
                battleManager.targetUnit = enemy1;
                break;
            case DECISION.PoisonEnemy2:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Poison");
                battleManager.targetUnit = enemy2;
                break;
            case DECISION.StunEnemy1:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Stun");
                battleManager.targetUnit = enemy1;
                if(decisionLog){
                    StatusEffect stunEffect = enemy1.effects.FindLast(effect => effect.text == "Stunned");
                    if(stunEffect != null && stunEffect.TurnsLeft == 1 && myLogIndex != -1)
                        decisionLog.logs[myLogIndex].coordinatedStuns++;
                }
                break;
            case DECISION.StunEnemy2:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Stun");
                battleManager.targetUnit = enemy2;
                if(decisionLog){
                    StatusEffect stunEffect = enemy2.effects.FindLast(effect => effect.text == "Stunned");
                    if(stunEffect != null && stunEffect.TurnsLeft == 1 && myLogIndex != -1)
                        decisionLog.logs[myLogIndex].coordinatedStuns++;
                }
                break;
            case DECISION.BlindEnemy1:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Blind");
                battleManager.targetUnit = enemy1;
                if(decisionLog){
                    StatusEffect blindEffect = enemy1.effects.FindLast(effect => effect.text == "Blinded");
                    if(blindEffect != null && blindEffect.TurnsLeft == 1 && myLogIndex != -1)
                        decisionLog.logs[myLogIndex].coordinatedBlinds++;
                }
                break;
            case DECISION.BlindEnemy2:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Blind");
                battleManager.targetUnit = enemy2;
                if(decisionLog){
                    StatusEffect blindEffect = enemy2.effects.FindLast(effect => effect.text == "Blinded");
                    if(blindEffect != null && blindEffect.TurnsLeft == 1 && myLogIndex != -1)
                        decisionLog.logs[myLogIndex].coordinatedBlinds++;
                }
                break;
            case DECISION.BlockSelf:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Block");
                battleManager.targetUnit = self;
                break;
            case DECISION.HealSelf:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Heal");
                battleManager.targetUnit = self;
                break;
            case DECISION.HealAlly:
                battleManager.selectedAction = self.skillList.Find(skill => skill.name == "Heal");
                battleManager.targetUnit = ally;
                if(decisionLog && myLogIndex != -1){
                    decisionLog.logs[myLogIndex].healedAlly++;
                    // Same health percentage the heuristic uses to classify unit as vulnerable
                    if(ally.CurHP < 0.65 * ally.MaxHP){
                        decisionLog.logs[myLogIndex].healedVulnerableAlly++;
                    }
                    // The heal skill heals based on max hp percentage of the target
                    // For consistency's sake the comparison also uses health percentage
                    if(ally.CurHP / ally.MaxHP < self.CurHP / self.MaxHP){
                        decisionLog.logs[myLogIndex].healedWeakerAlly++;
                    }
                }
                break;
        }
        
        // Debug.Log("Calculated reward at action = " + GetCumulativeReward());
        battleManager.TargetSelected();
    }

    public new void AddReward(float value){
        base.AddReward(value);
        if(decisionLog && myLogIndex != -1){
            decisionLog.logs[myLogIndex].reward += value;
        }
    }

    public void OnApplicationQuit(){
        // No longer necessary, battlemanager takes care of this
        // if(decisionLog && myLogIndex != -1){
        //     decisionLog.RemoveAgent(this);
        // }
    }

    public override void AgentReset() {
        // self.ResetStats();
        // Debug.Log("agent was reset");
    }
}
