using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "DecisionLog", menuName = "ScriptableObject/DecisionLog")]
public class DecisionLog : ScriptableObject {

	[System.Serializable]
	public class InfoLog{
		public int healedAlly;
		public int healedVulnerableAlly;
		public int healedWeakerAlly;
		public int coordinatedStuns;
		public int coordinatedBlinds;
		public int remainingHP;
		public bool won;
		public float reward;
		public int nTurns;

		public string CSVLine{
			get{
				string value = "" + (won? "1" : "0") + ",";
				value += remainingHP + ",";
				value += reward + ",";
				value += healedAlly + ",";
				value += coordinatedStuns + ",";
				value += coordinatedBlinds + ",";
				value += healedVulnerableAlly + ",";
				value += healedWeakerAlly + ",";
				value += nTurns;
				return value;
			}
		}

		public void Reset(){
			healedAlly = 0;
			healedVulnerableAlly = 0;
			healedWeakerAlly = 0;
			coordinatedStuns = 0;
			coordinatedBlinds = 0;
			remainingHP = 0;
			won = false;
			reward = 0;
			nTurns = 0;
		}
	}

	[SerializeField] private TextAsset vsWeak = null;
	[SerializeField] private TextAsset vsMedium = null;
	[SerializeField] private TextAsset vsStrong = null;
	[SerializeField] private TextAsset vsTrained = null;
	[SerializeField] private TextAsset csvFile = null;
	[SerializeField] private string csvFilePath;
	private StreamWriter streamWriter;
	// To do: Change to private with a public reference to readonly list if possible
	public List<UnitAgent> agents;
	public List<InfoLog> logs;

	public void Awake(){
		logs = new List<InfoLog>();
		agents = new List<UnitAgent>();
		Debug.Log("Chamou o awake e logs " + ((logs == null)? "" : "não ") + "é null");
		ClearLog();
	}

	public void ClearLog(){
#if UNITY_EDITOR
		// Select which file to use depending of battle type
		if(BattleManager.Instance && BattleManager.Instance.BattleMode == BattleModes.DataCollection){
			if(BattleManager.Instance.Academy.HasHeuristicBrain){
				switch(BattleManager.Instance.Academy.CurrentDifficulty){
					case 0:
						csvFile = vsWeak;
						break;
					case 1:
						csvFile = vsMedium;
						break;
					case 2:
						csvFile = vsStrong;
						break;
				}
			}else{
				csvFile = vsTrained;
			}
		}
		if(csvFile == null){
			if(BattleManager.Instance && BattleManager.Instance.BattleMode == BattleModes.DataCollection){
				Debug.Log("Log invalido");
			}
			return;
		}else if(BattleManager.Instance){
			bool shouldClear = false;
			foreach(UnitAgent agent in agents){
				if(agent.brain.brainType == MLAgents.BrainType.Internal)
					shouldClear = true;
			}
			if(!shouldClear)
				return;
		}

		csvFilePath = AssetDatabase.GetAssetPath(csvFile);
		Debug.Log("Limpou o log em " + csvFilePath);
		streamWriter = new StreamWriter(csvFilePath, false);
		streamWriter.WriteLine("Victory,Remaining HP,Reward,Times Healed Ally,Times Coordinated Stuns,Times Coordinated Blinds, Times Healed Vulnerable Ally, Times Healed Weaker Ally, Battle Length");
		streamWriter.Close();
#endif
	}

	public void SaveBattle(){
		if(csvFile == null) return;

#if UNITY_EDITOR
		if(csvFilePath == null || csvFilePath == "")
			csvFilePath = AssetDatabase.GetAssetPath(csvFile);
			
		streamWriter = new StreamWriter(csvFilePath, true);
		for(int i = 0; i < logs.Count; i++){
			logs[i].remainingHP = agents[i].CurrentHealth;
			logs[i].won = (logs[i].reward > (DFAcademy.maxRewardValue / 2.0f));
			logs[i].nTurns = BattleManager.Instance.turnCounter;
			streamWriter.WriteLine(logs[i].CSVLine);
			logs[i].Reset();
		}
		streamWriter.Close();
#endif
	}

	public void RemoveAgent(UnitAgent agent){
		if(agents != null && agents.Contains(agent)){
			agents.Remove(agent);
		}
	}

	public int AddAgent(UnitAgent newAgent){
#if UNITY_EDITOR
		if(newAgent != null){
			agents.Add(newAgent);
			if(logs == null){
				Debug.Log("WHAT THE FUUUUUUCK");
			}
			logs.Add(new InfoLog());
			return logs.Count - 1;
		}
#endif
		return -1;
	}
}
