using Unity.VisualScripting;
using UnityEngine;

public class ManagerMono : MonoBehaviour
{
    protected CharacterAIHandler characterAIHandler;
    protected CharacterControl characterControl;
    protected InteractionEngine interactionEngine;
    protected NeedsEngine needsEngine;
    protected LotManager lotManager;
    protected UIController UIController;
    protected CharacterRelationshipEngine relationshipEngine;
    protected ItemManager itemManager;
    protected CapabilityHandler capabilityHandler;
    protected CharacterPathfinding characterPathfinding;
    protected InstructionEngine instructionEngine;
   
    private int oneUnitOfTime = 1;
    protected int OneUnitOfTime { get { return oneUnitOfTime; } }
    
    private Vector3 negSpawnPos= new Vector3(-1,-1,-1);
    protected Vector3 NegSpawnPos { get { return negSpawnPos; } }

    [SerializeField]
    protected bool debugLog;
    [SerializeField]
    protected bool displayGizmos;

    protected virtual void Start()
    {
        characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        characterControl = FindAnyObjectByType<CharacterControl>();
        interactionEngine = FindAnyObjectByType<InteractionEngine>();
        needsEngine = FindAnyObjectByType<NeedsEngine>();
        lotManager = FindAnyObjectByType<LotManager>();
        UIController = FindAnyObjectByType<UIController>();
        relationshipEngine = FindAnyObjectByType<CharacterRelationshipEngine>();
        itemManager = FindAnyObjectByType<ItemManager>();
        capabilityHandler = FindAnyObjectByType<CapabilityHandler>();
        characterPathfinding = FindAnyObjectByType<CharacterPathfinding>();

        instructionEngine = FindAnyObjectByType<InstructionEngine>();
    }
}
