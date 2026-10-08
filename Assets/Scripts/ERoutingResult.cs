using UnityEngine;

public enum ERoutingResult
{
    Default,
    Success,                
    FailedNoPathPermanent,  //When path is blocked by objects that don't move on their own (furniture, walls)
    FailedNoPathTemporary,  //When path is blocked by objects that move (characters), maybe try again later
}
