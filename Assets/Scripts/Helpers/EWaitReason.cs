using UnityEngine;

public enum EWaitReason
{
    Default,
    WaitForSocialInteractionPartner,    //Target of social interaction waiting for partner to get in position
    WaitForItemToBeAvailable,           //Initiator waiting for item
    WaitForSocialResponder,             //Waiting for social responder be waiting
}
