using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedsEngine : ManagementCore
{

    private List<Need_Instruction> activeNeedInstructions = new();

    //private Dictionary<Need, List<INeedAlertable>> needsToHandle = new();
    private Dictionary<Character, List<Need>> needsByCharacter = new();
    private List<Need> changedNeeds = new();

    protected override void TimedUpdate(float dt)
    {
        base.TimedUpdate(dt);
        timeSinceLastUdate += deltaTime;

        if (TooEarlyForNextTick(updateInterval))
            return;

        NeedsUpdate(dt);
        InstructionsUpdate(deltaTime);

        //Alerting changes
        foreach (Need need in changedNeeds)
        {
            AlertNeedChange(need);
        }
        changedNeeds.Clear();
    }

    //Decay
    private void NeedsUpdate(float dt)
    {
        //Decay
        foreach (List<Need> needs in needsByCharacter.Values)
        {
            foreach (Need need in needs)
            {
                need.TimeSinceLastNeedDecay += (dt + updateInterval);
                if (need.TimeSinceLastNeedDecay > OneUnitOfTime)
                {
                    need.TimeSinceLastNeedDecay -= OneUnitOfTime;
                    DecayNeed(need);
                }

            }
        }
    }

    private void DecayNeed(Need need)
    {
        AdjustNeed(need, need.NeedDecay);
    }
    private void AlertNeedChange(Need need)
    {
        foreach (INeedAlertable alertable in need.Alertables)
        {
            alertable.UpdateAndRefreshData(need);
        }
    }
    //Instructions
    public void NewInstructions(List<Need_Instruction> needInstructions)
    {
        foreach (Need_Instruction needInstruction in needInstructions)
        {
            if (activeNeedInstructions.Contains(needInstruction))
                continue;
            activeNeedInstructions.Add(needInstruction);
        }
    }
    //Useful template for elsewhere
    //private void InstructionsUpdate(float deltaTime)
    //{
    //    for (int i = activeNeedInstructions.Count - 1; i >= 0; i--)
    //    {
    //        Need_Instruction needInstruction = activeNeedInstructions[i];
    //        if (needInstruction.InstructionLifetime >= needInstruction.InstructionTriggerTime)
    //        {
    //            RunInstruction(needInstruction);
    //            activeNeedInstructions.Remove(needInstruction);
    //        }
    //        else
    //            needInstruction.InstructionLifetime += deltaTime;
    //    }
    //}
    private void InstructionsUpdate(float deltaTime)
    {
        for (int i = activeNeedInstructions.Count - 1; i >= 0; i--)
        {
            float dt = deltaTime + updateInterval;
            Need_Instruction instruction = activeNeedInstructions[i];
            if (instruction.TimeSinceLastUse > OneUnitOfTime)
            {
                instruction.TimeSinceLastUse -= OneUnitOfTime;
                RunInstruction(instruction);
                //For Now, all interactions are fire once
                activeNeedInstructions.Remove(instruction);
            }
            else
                instruction.TimeSinceLastUse += dt;



            //if (instruction.InstructionLenght > 0)
            //{
            //    if (instruction.InstructionLifetime >= instruction.InstructionLenght)
            //        activeNeedInstructions.Remove(instruction);
            //    else
            //        instruction.InstructionLifetime += dt;
            //}
            //else
            //    activeNeedInstructions.Remove(instruction);
        }
    }

    private void RunInstruction(Need_Instruction needInstruction)
    {
        Need need = needInstruction.NeedOwner.Needs[needInstruction.NeedToAdjust];
        AdjustNeed(need, needInstruction.NeedAdjustValuePerTic);
    }

    //Need Manipulation
    private void AdjustNeed(Need need, int adjustAmount)
    {
        need.NeedValue += adjustAmount;
        if (!changedNeeds.Contains(need))
            changedNeeds.Add(need);
    }
    private void SetNeed(Need need, int value)
    {
        need.NeedValue = value;
        if (!changedNeeds.Contains(need))
            changedNeeds.Add(need);
    }


    //Registering
    public void RegisterToNeedsEngine(Character character)
    {
        needsByCharacter.Add(character, new List<Need>());
        foreach (Need need in character.Needs.Values)
        {
            needsByCharacter[character].Add(need);
        }
    }
    public void RegisterForNeedChangeAlert(INeedAlertable needAlertable, Need need)
    {
        need.AddAlertable(needAlertable);

    }

    public void DeRegisterFromNeedsEngine(Character character)
    {
        needsByCharacter.Remove(character);
    }
    public void DeRegisterToNeedChangeAlert(INeedAlertable needAlertable, Need need)
    {
        need.RemoveAlertable(needAlertable);
    }
}
