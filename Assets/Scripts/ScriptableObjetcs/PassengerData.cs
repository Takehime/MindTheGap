using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Passenger")]
public class PassengerData : ScriptableObject {

    public enum PassengerType {
        IDOSO, ESTUDANTE, TURISTA, VAREJISTA
    };
    
    public Color color;
    public PassengerType type;
}
