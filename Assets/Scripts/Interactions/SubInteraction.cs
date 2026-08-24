using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SubInteraction
{
    [SerializeField]
    private InteractionSO interactionSO;
    public InteractionSO InteractionSO {  get { return interactionSO; } }

    [SerializeField, Tooltip("The stored interaction is on an object crearted by the parent interaction")]
    private bool interactionOnCreatedObject;
    public bool InteractionOnCreatedObject { get { return interactionOnCreatedObject; } }

    [SerializeField, Tooltip("The stored interaction is on an object carried by the character")]
    private bool interactionOnHeldObject;
    public bool InteractionOnHeldObject { get { return interactionOnHeldObject; } }

    [SerializeField, Tooltip("The stored interaction is on an object on one of the slots on the main object")]
    private bool interactionOnItemHeldObject;
    public bool InteractionOnItemHeldObject { get { return interactionOnItemHeldObject; } }

}