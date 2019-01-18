using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class DFAcademy : Academy {
    public List<UnitAgent> agents;
    
    public bool HasTrainingBrain() {
        foreach (UnitAgent agent in agents) {
            if (agent.brain.brainType == BrainType.External)
                return true;
        }
        return false;
    }
    private void Swap(int a, int b) {
        UnitAgent aux = agents[a];
        agents[a] = agents[b];
        agents[b] = aux;
    }
    public override void InitializeAcademy() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            agents.Add(player.GetComponent<UnitAgent>());
        }
        if(agents[0].GetComponent<Unit>().team != agents[1].GetComponent<Unit>().team) {
            if (agents[1].GetComponent<Unit>().team != agents[2].GetComponent<Unit>().team)
                Swap(1, 2);
            else
                Swap(1, 3);
        }
        Debug.Log("Final list: " + agents[0].name + ", " + agents[1].name + ", " + agents[2].name + ", " + agents[3].name);
    }
    public override void AcademyReset() {
        float[] HPBonus = new float[4];
        // Calculate how much each unit has as reward value based on remaining health
        for (int i = 0, j = 0; i < agents.Count; i++) {
            Unit unit = agents[i].GetComponent<Unit>();
            if (BattleManager.winningTeam == unit.team) {
                // HPBonus[index unit1, bonus unit1, index unit2, bonus unit2]
                HPBonus[j++] = i;
                HPBonus[j++] = (unit.MaxHP - unit.CurHP) / (unit.MaxHP * 2.0f);
            }
        }
        foreach(UnitAgent agent in agents) {
            int index = (agents.IndexOf(agent) == HPBonus[0]) ? 0 : (agents.IndexOf(agent) == HPBonus[2]) ? 2 : -1;
            switch (index) {
                // In case of agent on losing team
                case -1:
                    // Receives a reward of up to 0.5 based on how much health the enemies lost
                    agent.AddReward((HPBonus[1]+HPBonus[3])/2.0f);
                    break;
                // In case of agent on the winning team
                default:
                    // Receives a punishment based on how much health agent (and ally, if cooperative) lost
                    float reward = (agent.type == UnitAgent.TYPEAGENT.Individualistic) ? HPBonus[index + 1] : (HPBonus[index + 1] + HPBonus[(index + 3) % 4]) / 2.0f;
                    reward *= -1.0f;
                    agent.AddReward(reward);
                    break;
            }
        }
        foreach (UnitAgent agent in agents) {
            Debug.Log("agent " + agent.name + " has reward of " + agent.GetReward());
        }
        BattleManager.BattleLog = "";
        foreach (UnitAgent agent in agents) {
            agent.Done();
        }
    }
}
