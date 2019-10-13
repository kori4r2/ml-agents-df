using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecisionLog", menuName = "ScriptableObject/DecisionLog")]
public class DecisionLog : ScriptableObject {

	[TextArea] public string myAgents;
	public int agentsCount;
	public long nBattles;
	public long healedAlly;
	public long coordinatedStuns;
	public long coordinatedBlinds;
	public float rewardsMean;
	public float rewardsStdDeviation;
	[HideInInspector] public float newRewards;

	public void Reset(){
		myAgents = "";
		agentsCount = 0;
		rewardsMean = 0f;
		rewardsStdDeviation = 0f;
		newRewards = 0f;
		nBattles = 0;
		healedAlly = 0;
		coordinatedBlinds = 0;
		coordinatedStuns = 0;
	}

	public void RecalculateStatistics(){
		// Recalculate mean of rewards
		rewardsMean *= (nBattles * agentsCount);
		rewardsMean += newRewards;
		nBattles++;
		rewardsMean /= (nBattles * agentsCount);

		newRewards = 0f;
	}
}
