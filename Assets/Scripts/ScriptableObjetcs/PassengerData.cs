using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PassengerType {
    IDOSO, ESTUDANTE, TURISTA, VAREJISTA, TRABALHADOR
};

[CreateAssetMenu(menuName = "Custom/Passenger")]
public class PassengerData : ScriptableObject {

    public Color color;
    public PassengerType type;
}
