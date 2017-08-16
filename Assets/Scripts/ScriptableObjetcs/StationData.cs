using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Station")]
public class StationData : ScriptableObject {

	public string name;
	public List<PassengerType> perfis;

}
