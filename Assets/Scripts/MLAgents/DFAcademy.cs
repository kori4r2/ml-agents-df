using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class DFAcademy : Academy {
    public List<UnitAgent> agents;
    private bool changedLogic;
    private int debug = 0;
    
    public bool HasTrainingBrain() {
        //return true;
        foreach (UnitAgent agent in agents) {
            if (agent.brain.brainType == BrainType.External)
                return true;
        }
        return false;
    }

    public override void InitializeAcademy() {
        changedLogic = false;
        debug = 0;
        Debug.Log("Final list: " + agents[0].name + ", " + agents[1].name + ", " + agents[2].name + ", " + agents[3].name);
    }

    public void CalculateRewards() {
        float[] rewards = new float[agents.Count];
        /*
        // Print cumulative rewards
        foreach (UnitAgent agent in agents) {
            Debug.Log("agent " + agent.name + " has reward of " + agent.GetCumulativeReward());
        }
        */
        float[] HPBonus = new float[4];
        // Calculate how much each unit has as reward value based on remaining health
        for (int i = 0, j = 0; i < agents.Count; i++) {
            Unit unit = agents[i].GetComponent<Unit>();
            if (BattleManager.winningTeam == unit.team) {
                rewards[i] = 0.0f;
                agents[i].AddReward(1.0f);
                // HPBonus[index unit1, bonus unit1, index unit2, bonus unit2]
                HPBonus[j++] = i;
                HPBonus[j++] = (unit.MaxHP - unit.CurHP) / (unit.MaxHP * 1.0f);
                /*
                if (HPBonus[j - 1] == 0.5f)
                    HPBonus[j - 1] = 0.0f;
                */
                Debug.Log("HPBonus of unit " + agents[(int)HPBonus[j-2]].name + " is " + HPBonus[j-1]+ "; j-1 = " + (j-1));
            } else
                rewards[i] = 0.0f;
            rewards[i] = 0.0f;
        }
        for(int i = 0; i < agents.Count; i++) {
            int index = (i == HPBonus[0]) ? 0 : (i == HPBonus[2]) ? 2 : -1;
            // In case of agent on losing team
            if (index == -1) {
                // Receives a reward of up to 1.0 based on how much health the winners lost
                rewards[i] += ((HPBonus[1] + HPBonus[3]) / 2.0f);
            // In case of agent on the winning team
            } else {
                // Receives a punishment based on how much health agent (and ally, if cooperative) lost
                float reward = (agents[i].type == UnitAgent.TYPEAGENT.Individualistic) ? (1.0f - HPBonus[index + 1]) : ((1.0f - HPBonus[index + 1]) + (1.0f - HPBonus[(index + 3) % 4])) / 2.0f;
                rewards[i] += reward;
            }
            Debug.Log("Adding reward of " + rewards[i] + " to " + agents[i].name);
            agents[i].AddReward(rewards[i]);
        }
        // Print cumulative rewards
        foreach (UnitAgent agent in agents) {
            Debug.Log("agent " + agent.name + " has reward of " + agent.GetCumulativeReward());
        }
    }

    public override void AcademyReset() {
        //debug++;
        CalculateRewards();
        // Clear battle log
        if(!HasTrainingBrain())
            BattleManager.BattleLog = "";
        // During curriculum training, changes the heuristic brain's logic if necessary
        if ((!changedLogic && resetParameters["logic"] == 1.0f) || debug >= 5) {
            gameObject.GetComponentInChildren<SimpleHeuristic>().logic = BEHAVIOUR.SimpleLogic;
            changedLogic = true;
        } else
            resetParameters["logic"] = 0.0f;
        // Resets all agents and the scene as well
        foreach (UnitAgent agent in agents) {
            agent.Done();
        }
    }
}
