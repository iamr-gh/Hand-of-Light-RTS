using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitParameters : MonoBehaviour
{
    public float maxHP = 10.0f;

    [SerializeField] public float HP = 10.0f;
    [SerializeField] private float AttackDamage = 2f;
    [SerializeField] private float MovementSpeed = 2f;
    [SerializeField] private float AttackCooldown = 0.5f; // s
    [SerializeField] private float AttackDuration = 0.1f; // s, frozen for that time period
    [SerializeField] private float Mana = 100f;
    [SerializeField] private float HitboxSize = 1f;

    [Header("Vision Settings")]
    [SerializeField] private float AggroRange = 10f;
    [SerializeField] private float SightRange = 10f;
    [SerializeField] private float AttackRange = 2f; // Range from center of object

    [Header("Voice Line Settings")]
    [SerializeField] private List<AudioClip> SelectVoiceLines;
    [SerializeField] private float SelectVoiceLineVolume = 1f;
    [SerializeField] private List<AudioClip> MoveVoiceLines;
    [SerializeField] private float MoveVoiceLineVolume = 1f;
    [SerializeField] private List<AudioClip> AttackMoveVoiceLines;
    [SerializeField] private float AttackMoveVoiceLineVolume = 1f;
    [SerializeField] private List<AudioClip> DeathVoiceLines;
    [SerializeField] private float DeathVoiceLineVolume = 1f;
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
    public void setAttackCooldown(float atkCooldown) { AttackCooldown = atkCooldown; }
    public void setAttackDuration(float atkDuration) { AttackDuration = atkDuration; }
    public void setAggroRange(float aggroRange) { AggroRange = aggroRange; }
    public void setSightRange(float sightRange) { SightRange = sightRange; }
    public void setAttackRange(float atkRange) { AttackRange = atkRange; }
    public void setMana(float mana) { Mana = mana; }
    public void setHitboxSize(float size) { HitboxSize = size; }
    public void setNumberOfKills(int kills) { NumberOfKills = kills; }

    public void setSelectVoiceLines(List<AudioClip> clips) { SelectVoiceLines = clips; }
    public void setMoveVoiceLines(List<AudioClip> clips) { MoveVoiceLines = clips; }
    public void setAttackMoveVoiceLines(List<AudioClip> clips) { AttackMoveVoiceLines = clips; }
    public void setDeathVoiceLines(List<AudioClip> clips) { DeathVoiceLines = clips; }
    public void setSelectVoiceLineVolume(float volume) { SelectVoiceLineVolume = volume; }
    public void setMoveVoiceLineVolume(float volume) { MoveVoiceLineVolume = volume; }
    public void setAttackMoveVoiceLineVolume(float volume) { AttackMoveVoiceLineVolume = volume; }
    public void setDeathVoiceLineVolume(float volume) { DeathVoiceLineVolume = volume; }

    // Getters
    public float getHP() { return HP; }
    public float getAttackDamage() { return AttackDamage; }
    public float getMovementSpeed() { return MovementSpeed; }
    
    public float getAttackCooldown() { return AttackCooldown; }
    public float getAttackDuration() { return AttackDuration; }
    public float getAggroRange() { return AggroRange; }
    public float getSightRange() { return SightRange; }
    public float getAttackRange() { return AttackRange; }
    public float getMana() { return Mana; }
    public float getHitboxSize() { return HitboxSize; }
    public float getNumberOfKills() { return NumberOfKills; }

    public List<AudioClip> getSelectVoiceLines() { return SelectVoiceLines; }
    public List<AudioClip> getMoveVoiceLines() { return MoveVoiceLines; }
    public List<AudioClip> getAttackMoveVoiceLines() { return AttackMoveVoiceLines; }
    public List<AudioClip> getDeathVoiceLines() { return DeathVoiceLines; }
    public float getSelectVoiceLineVolume() { return SelectVoiceLineVolume; }
    public float getMoveVoiceLineVolume() { return MoveVoiceLineVolume; }
    public float getAttackMoveVoiceLineVolume() { return AttackMoveVoiceLineVolume; }
    public float getDeathVoiceLineVolume() { return DeathVoiceLineVolume; }
}
