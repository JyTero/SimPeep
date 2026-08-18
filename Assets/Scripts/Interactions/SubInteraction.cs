using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SubInteraction
{
    [SerializeField]
    private InteractionSO storedInteractionSO;
    public InteractionSO StoredInteractionSO {  get { return storedInteractionSO; } }

    [SerializeField, Tooltip("The stored interaction is on an object crearted by the parent interaction")]
    private bool interactionOnCreatedObject;
    public bool InteractionOnCreatedObject { get { return interactionOnCreatedObject; } }

}
