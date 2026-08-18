using UnityEngine;

public enum InteractionQueuePriority
{
    UrgentReaction,         //Fire, emergency
    AINeedFixing,           //Critical Need
    SuggestedFollowup,      
    UserSelectNPCReaction,
    UserSelect,
    NormalAISelect,
}
