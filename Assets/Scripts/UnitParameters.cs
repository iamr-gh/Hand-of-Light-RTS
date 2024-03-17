using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParameters : MonoBehaviour
{
    public float maxHP = 10.0f;
    
    private float HP = 10.0f;
    private float AttackDamage = 2f;
    private float MovementSpeed = 2f;
    private float AttackRate = 1f; // Attacks / second
    private float AttackRange =  2f; // Range from center of object
    private float Mana = 100f; 
    private float HitboxSize = 1f;
    private int NumberOfKills = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Setters
    public void setHP(float hp) { HP = hp; }
    public void setAttackDamage(float atkDmg) { AttackDamage = atkDmg; }
    public void setMovementSpeed(float movSpd) { MovementSpeed = movSpd; }
    public void setAttackRate(float atkRate) { AttackRate = atkRate; }
    public void setAttackRange(float atkRange) { AttackRange = atkRange; }
    public void setMana(float mana) { Mana = mana; }
    public void setHitboxSize(float size) { HitboxSize = size; }
    public void setNumberOfKills(int kills) { NumberOfKills = kills; }

    // Getters
    public float getHP() { return HP; }
    public float getAttackDamage() { return AttackDamage; }
    public float getMovementSpeed() { return MovementSpeed; }
    public float getAttackRate() { return AttackRate; }
    public float getMana() { return Mana; }
    public float getHitboxSize() { return HitboxSize; }
    public float getNumberOfKills() { return NumberOfKills; }

}
