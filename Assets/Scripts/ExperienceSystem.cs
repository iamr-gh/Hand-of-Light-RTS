using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceSystem : MonoBehaviour
{
    // These compound
    public float[] experienceModifiers = {0f, 1.2f, 1.3f, 1.3f};
    public int[] experienceThresholds = {10, 25, 50};
    private int maxLevel = 3;
    private int level = 0;
    private int XP = 0;
    private UnitParameters parameters;

    // Start is called before the first frame update
    void Start()
    {
        parameters = GetComponent<UnitParameters>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LevelUp() {
        if(level == 3) { return; } // Already maximum level, do nothing

        level += 1; // Increment Level
        if(parameters == null) { return; } // If there are no params to change, just change level
        float modifier = experienceModifiers[level];

        ModifyParameters(modifier);
        parameters.setHP(parameters.maxHP); // Heal to full health
    }

    void ModifyParameters(float modifier) {
        parameters.maxHP *= modifier;
        parameters.setAttackDamage(parameters.getAttackDamage() * modifier);
        parameters.setMovementSpeed(parameters.getMovementSpeed() * modifier);
        parameters.setAttackCooldown(parameters.getAttackCooldown() * modifier);
        parameters.setAttackRange(parameters.getAttackRange() * modifier);
        parameters.setMana(parameters.getMana() * modifier);
    }

    // Setters
    public void setLevel(int lvl) { level = lvl; }
    // This is commented out as I don't believe there should be a use case for this, 
    // we should always use gainXP()
    // public void setXP(int xp) { XP = xp; } 
    public void gainXP(int xp) { 
        XP += xp; 
        if(level < maxLevel && XP >= experienceThresholds[level]) {
            LevelUp();
        }
    }

    // Getters
    public int getLevel() { return level; }
    public int getXP() { return XP; }
    
}
