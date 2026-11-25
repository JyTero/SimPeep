using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TV_Item : ItemBase
{

    //PLAN: Movies and TV Channels are SOs, which contain the visuals and any need/skill changes the program gives.
    //TV has a list of the available options
    //For now: Generic Movie, Generic Channel
    
    
    [Header("TV ITEM")] //These should have private set public get
    public InteractionSO TVChangeChannelSO;
    public InteractionSO TVTurnOnSO;
    public InteractionSO TVTurnOffSO;
    public InteractionSO WatchTVSO;

    //[SerializeField]
    //private List<TVChannelSO> tvChannels = new();
    //public List<TVChannelSO> TVCHannels { get { return tvChannels; } }

    [SerializeField]
    private List<TVItem_InstructionSO> changeChannelInstructionSOs = new();
    public List<TVItem_InstructionSO> ChangeChannelInstructionSOs { get { return changeChannelInstructionSOs; } }


    private Dictionary<TVItem_InstructionSO, StoredInteraction> changeChannelInteractions = new();
    public IReadOnlyDictionary<TVItem_InstructionSO,StoredInteraction> ChangeChannelInteractions => changeChannelInteractions;

    private Color defaultColor;
    [Header("DEBUG")]
    [SerializeField, Tooltip("DEBUG, DO NOT ADJUST")]
    private ETVState tvState;
    public ETVState TvState { get { return tvState; } }

    [SerializeField, Tooltip("DEBUG, DO NOT ADJUST")]
    private TVChannelSO currentTVChannel;
    public TVChannelSO CurrentTVChannel { get { return currentTVChannel; } }

    [SerializeField, Tooltip("DEBUG, DO NOT ADJUST")]
    private List<Character_Slot> sittingSlotsInArea = new();
    public List<Character_Slot> SittingSlotsInArea { get  { return sittingSlotsInArea; } }



    public void RecordDefaultColor()
    {
        defaultColor = gameObject.GetComponent<MeshRenderer>().material.color;
    }
    public void NewChangeChannelInteraction(TVItem_InstructionSO channelInstruction, StoredInteraction changeChannelInteraction)
    {
        changeChannelInteractions.Add(channelInstruction, changeChannelInteraction);
    }
    public void NewSittingSlotsInArea(Character_Slot slot)
    {
        if (sittingSlotsInArea.Contains(slot))
            throw new Exception($"Slot {slot.name} already exitst in {itemName}'s interaction area slots lits");
        else
            sittingSlotsInArea.Add(slot);
    }

    public void ChangeTVChannel(TVChannelSO tvChannelSO)
    {
        UpdateTVVisuals(tvChannelSO.ChannelColor);
        currentTVChannel = tvChannelSO;

    }

    public void TurnTvOn()
    {
        tvState = ETVState.OnChannel;
        UpdateTVVisuals(currentTVChannel.ChannelColor);
    }
    public void TurnTvOff()
    {
        tvState = ETVState.Off;
        UpdateTVVisuals(defaultColor);
    }
    private void UpdateTVVisuals(Color ChannelColor)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = ChannelColor;
    }

}
public enum ETVState
{
    Default,
    Off,
    OnChannel,
}
