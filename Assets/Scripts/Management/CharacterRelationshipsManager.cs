using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class CharacterRelationshipsManager : ManagementCore
{
    private Dictionary<Character, Dictionary<Character, CharacterRelationship>> relationshipsByCharacter = new();
    public Dictionary<Character, Dictionary<Character, CharacterRelationship>> RelationshipsByCharacter { get { return relationshipsByCharacter; } }

    protected override void Start()
    {
        base.Start();
    }

    public void NewCharacter(Character chara)
    {
        relationshipsByCharacter.Add(chara, new());
    }
    public bool HasExistingRelationship(Character thisChara, Character otherChara)
    {
        if (relationshipsByCharacter[thisChara].ContainsKey(otherChara))
            return true;
        else
            return false;
    }
    public void NewRelationship(Character thisCharacter, Character target)
    {
        relationshipsByCharacter[thisCharacter].Add(target, new CharacterRelationship(target));
    }
    public void AdjustRelationship(Character thisCharacter, Character target, int adjustAmmount)
    {
        relationshipsByCharacter[thisCharacter][target].RelationshipScore += adjustAmmount;
    }

    //DEBUG
    public void PrintRelations()
    {
        int i = 0;
        string s = "Relationships:\n";
        foreach (Character chara in RelationshipsByCharacter.Keys)
        {
            s += $"Character {chara.ItemName}'s relations: \n";
            foreach (CharacterRelationship crela in relationshipsByCharacter[chara].Values)
            {
                s += $"{crela.Target.ItemName}: {crela.RelationshipScore}\n";
            }
        }
        Debug.Log(s);
    }
}

public class CharacterRelationship
{
    private Character target;
    public Character Target { get { return target; } }

    public int RelationshipScore;
    //public int RelationshipScore { get { return relationshipScore; } }

    public CharacterRelationship(Character chara)
    {
        target = chara;
        RelationshipScore = 0;
    }

}



