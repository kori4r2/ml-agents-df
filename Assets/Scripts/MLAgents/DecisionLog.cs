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
		public int coordinatedStuns;
		public int coordinatedBlinds;
		public int remainingHP;
		public bool won;
		public float reward;

		public string CSVLine{
			get{
				string value = "" + (won? "1" : "0") + ",";
				value += remainingHP + ",";
				value += reward + ",";
				value += healedAlly + ",";
				value += coordinatedStuns + ",";
				value += coordinatedBlinds;
				return value;
			}
		}

		public void Reset(){
			healedAlly = 0;
			coordinatedStuns = 0;
			coordinatedBlinds = 0;
			remainingHP = 0;
			won = false;
			reward = 0;
		}
	}

	[SerializeField] private TextAsset csvFile = null;
	[SerializeField] private string csvFilePath;
	private StreamWriter streamWriter;
	public List<UnitAgent> agents;
	public List<InfoLog> logs;

	public void Awake(){
		logs = new List<InfoLog>();
		agents = new List<UnitAgent>();
		Debug.Log("Chamou o awake e logs " + ((logs == null)? "" : "não ") + "é null");
		ClearLog();
	}

	public void ClearLog(){
		if(csvFile == null) return;

#if UNITY_EDITOR
		csvFilePath = AssetDatabase.GetAssetPath(csvFile);
		Debug.Log(csvFilePath);
		streamWriter = new StreamWriter(csvFilePath, false);
		streamWriter.WriteLine("Victory,Remaining HP,Reward,Times Healed Ally,Times Coordinated Stuns,Times Coordinated Blinds");
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
			streamWriter.WriteLine(logs[i].CSVLine);
			logs[i].Reset();
		}
		streamWriter.Close();
#endif
	}

	public void RemoveAgent(UnitAgent agent){
		if(agents.Contains(agent)){
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
