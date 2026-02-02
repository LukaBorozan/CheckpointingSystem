using SaveSystem;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TestMonobehavior : MonoBehaviour, ISaveable
{
    [Save][SerializeField] public string heroName;
    [Save][SerializeField] public ulong heroClass;

    [Save][SerializeField] public short level;
    [Save][SerializeField] public int experience;

    [Save][SerializeField] public short attack;
    [Save][SerializeField] public short defense;
    [Save][SerializeField] public short power;
    [Save][SerializeField] public short knowledge;
    
    [Save][SerializeField] public short morale;
    [Save][SerializeField] public short luck;

    [Save][SerializeField] public ulong specialty;
    [Save][SerializeField] public List<(ulong, short)> skills = new(8);

    [Save][SerializeField] public float movementPoints;
    [Save][SerializeField] public float roughtTerrainPenalty;
    [Save][SerializeField] public int spellPoints;

    [Save][SerializeField] public ulong[] inventory = new ulong[18];
    [Save][SerializeField] public List<ulong> backpack = new(64);

    [Save]
    private Vector3 Position 
    {
        get 
        {
            return transform.position;
        }

        set
        {
            transform.position = value;
        }
    }

    [Save]
    private Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }

        set
        {
            transform.rotation = value;
        }
    }

    private System.Random random = new();

    private static List<string> names = new()
    {
        "Orin", "Crag Hack", "Sandro", "Gelu", "Astral", "Gunnar",
        "Malekith", "Solmyr", "Yog", "Christian", "Jeremy", "Ivor",
        "Marius", "Xyron", "Lacus", "Neela", "Straker", "Isra",
        "Valeska", "Clancy", "Kyrre", "Elleshar", "Octavia", "Nagash",
        "Shakti", "Damacon", "Dessa", "Tazar", "Alkin", "Thunar",
        "Ciele", "Corkes", "Todd",
    };

    public void Randomize()
    {
        heroName = names[random.Next(0, names.Count - 1)];
        heroClass = (ulong)random.Next(0, 48);

        level = (short)random.Next(1, 74);
        experience = random.Next(0, 1810034207);

        attack = (short)random.Next(1, 99);
        defense = (short)random.Next(1, 99);
        power = (short)random.Next(1, 99);
        knowledge = (short)random.Next(1, 99);

        morale = (short)random.Next(-3, 3);
        luck = (short)random.Next(-3, 3);

        ulong specialty = (ulong)random.Next(0, 48);
        for (int i = 0; i < 8; ++i)
        {
            
            if (random.Next(1, 10) > 5)
            {
                ulong skillId = (ulong)random.Next(0, 27);
                short level = (short)random.Next(0, 3);
                skills.Add((skillId, level));
            }
        }

        movementPoints = random.Next(1000, 3000);
        roughtTerrainPenalty = random.Next(1, 10) / 10.0f;
        spellPoints = random.Next(0, 999);

        for (int i = 0; i < 18; ++i)
        {
            if (random.Next(1, 10) > 5)
            {
                inventory[i] = (ulong)random.Next(0, 512);
            }
        }

        for (int i = 0; i < 64; ++i)
        {
            if (random.Next(1, 10) > 8)
            {
                backpack.Add((ulong)random.Next(0, 512));
            }
        }
    }
}
