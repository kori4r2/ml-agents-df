using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class UnitAgent : Agent {

    Unit unit, ally, enemy1 = null, enemy2 = null, aux;

	// Use this for initialization
	void Start () {
        unit = gameObject.GetComponent<Unit>();
        GameObject[] units = GameObject.FindGameObjectsWithTag("Player");
        aux = units[0].GetComponent<Unit>();
        for (int i = 0; i < units.Length; i++, aux = units[i].GetComponent<Unit>()) {
            if (aux.team == unit.team) {
                if (aux.name.Equals(unit.team))
                    ally = aux;
            }else {
                if (enemy1 == null)
                    enemy1 = aux;
                else
                    enemy2 = aux;
            }
        }
	}
}
