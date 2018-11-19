using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    // Fixed stats
    private int maxHP;
    private int maxMP;
    private int baseDef;
    private int baseBth;
    private int baseInitiative;
    // Changeable stats
    private int curHP;
    private int curMP;
    private int baseAtk;
    private int maxAtk;
    private int def;
    private int bth;
    public char team;
    public bool acted;
    public bool lastAtkHit;
    public new string name;
    // Properties (getters/setters)
    private int CurHP { get; set; }
    private int CurMP { get; set; }
    private int BaseAtk { get; set; }
    private int MaxAtk { get; set; }
    public int Def { get; set; }
    public int Bth { get; set; }
    public int Dmg {
        get { return DmgRoll(); }
    }
    public int Atk {
        get { return AtkRoll(); }
    }
    // Aux variables
    private List<StatusEffect> effects;
    public List<Skill> skillList;
    // Aux functions
    private int AtkRoll() {
        return baseBth + Random.Range(0, 100);
    }
    private int DmgRoll(){
        return Random.Range(BaseAtk, MaxAtk);
    }
    // Public functions
    public bool CheckHit(int bth) {
        return bth > Def;
    }
    public void Purge() {
        //to do
    }
    public void CountdownEffects() {
        //to do
    }
    public void DealDamage(int value, string element="None") {
        //to do
    }
    // Use this for initialization
    void Start () {
		//to do
	}
	
	// Update is called once per frame
	void Update () {
		// to do
	}
}
