using UnityEngine;

public abstract class InstructionSO : ScriptableObject
{
    [SerializeField, Tooltip("Use this to leave notes about the instruction, such as use cases")]
    private string Description;

    //Move to separet subclass 
    //[SerializeField, Tooltip("Time, in seconds, into the interaction for the instruction to be executed. If more than interaction length, wont happen")]
    //private float instructionTime;
    //public float InstructionTime { get { return instructionTime; } }

}

