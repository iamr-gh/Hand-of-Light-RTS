using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitJuice : MonoBehaviour {
    private NavMeshAgent navAgent;

    // For Audio Juice
    [SerializeField] string attackAudio;
    [SerializeField] string damageAudio;
    [SerializeField] string deathAudio;
    [SerializeField] string movementAudio;
    [SerializeField] float movementSpeedThresholdForAudio = 0.5f;
    AudioClip attackSound;
    AudioClip damageSound;
    AudioClip deathSound;
    AudioClip movementSound;

    // For Sprite Rotation
    private string facing = "Left";
    private bool isTurning = false;
    [SerializeField] float turnDuration = 0.2f;
    [SerializeField] float hopHeight = 0.2f;
    [SerializeField] float hopRotation = 5f;
    [SerializeField] bool doHop = true;

    private float moveStartTime;

    UnitParameters parameters;
    GameObject attackVFX;
    GameObject createdSlash;

    private bool damageJuiceActive = false;
    private bool attackJuiceActive = false;
    private bool movementJuiceActive = false;

    // Start is called before the first frame update
    void Start() {
        attackSound = Resources.Load<AudioClip>(attackAudio);
        damageSound = Resources.Load<AudioClip>(damageAudio);
        deathSound = Resources.Load<AudioClip>(deathAudio);
        movementSound = Resources.Load<AudioClip>(movementAudio);
        parameters = GetComponentInParent<UnitParameters>();
        navAgent = GetComponentInParent<NavMeshAgent>();
        attackVFX = Resources.Load<GameObject>("VisualEffects/SlashVFX");
    }

    // Update is called once per frame
    void Update() {
        if (!movementJuiceActive && navAgent.velocity.magnitude >= movementSpeedThresholdForAudio) {
            //StartCoroutine(MovementJuice()); // Commented out because it's really loud and poorly implemented rn
        }

        // Sprite 'Hopping'
        if (doHop) {
            if (navAgent.velocity.magnitude < 0.5f) { // Randomize the start time so they all "hop" differently
                moveStartTime = Time.time + Random.Range(0f, 0.3f); // Update the start time
            } else {
                float positionInCycle = Mathf.Abs(Mathf.Sin(2 * Mathf.PI * (Time.time - moveStartTime)));
                transform.position = transform.parent.position + hopHeight * transform.up * positionInCycle;
            }
        }

        // Sprite Rotation: Only start a turn after one is finished and not attacking
        if (!isTurning && !attackJuiceActive) {
            if (facing == "Left" && navAgent.velocity.x > 0) {
                facing = "Right";
                StartCoroutine(RotateObject(180f, turnDuration));
            } else if (facing == "Right" && navAgent.velocity.x < 0) {
                facing = "Left";
                StartCoroutine(RotateObject(180f, turnDuration));
            }
        }
    }

    // Update is called once per frame
    private void LateUpdate() {
        // Destroy dead unit
        if (parameters.getHP() <= 0) {
            var deathSounds = parameters.getDeathVoiceLines();
            if (deathSounds != null && deathSounds.Count > 0) {
                AudioManager.instance.PlayAudioClip(deathSounds[Random.Range(0, deathSounds.Count)], parameters.getDeathVoiceLineVolume());
            } else {
                AudioManager.instance.PlayAudioClip(deathSound);
            }
            Destroy(transform.parent.gameObject);
        }
    }

    public void TakeDamage(float damage) {
        parameters.setHP(parameters.getHP() - damage);
        StartCoroutine(DamageJuice());
    }

    IEnumerator MovementJuice() {
        movementJuiceActive = true;

        // Play audio clip and sleep for it's length
        AudioManager.instance.PlayAudioClip(movementSound);
        yield return new WaitForSeconds(movementSound.length);

        movementJuiceActive = false;
    }

    public IEnumerator AttackJuice() {
        attackJuiceActive = true;
        AudioManager.instance.PlayAudioClip(attackSound);
        createdSlash = Instantiate(attackVFX, transform.position, transform.rotation);
        yield return new WaitForSeconds(parameters.getAttackDuration());
        Destroy(createdSlash);
        attackJuiceActive = false;
    }

    IEnumerator DamageJuice() {
        if (damageJuiceActive) { yield break; }
        damageJuiceActive = true;

        AudioManager.instance.PlayAudioClip(damageSound);
        yield return new WaitForSeconds(damageSound.length);

        damageJuiceActive = false;
    }

    IEnumerator RecallJuice() { yield break; }

    IEnumerator RotateObject(float angle, float duration_sec) {
        isTurning = true;
        float initialTime = Time.time;
        float angularVelocity = angle / duration_sec;
        float deltaAngle = 0;
        while (Mathf.Abs(Time.time - initialTime) < duration_sec) {
            transform.RotateAround(transform.position, transform.up, angularVelocity * Time.deltaTime);
            deltaAngle += angularVelocity * Time.deltaTime;
            yield return null;
        }
        transform.RotateAround(transform.position, transform.up, angle - deltaAngle);

        isTurning = false;
    }

    void OnDestroy() {
        Debug.Log("Destroying Unit Juice");
        if(createdSlash != null) { Destroy(createdSlash); }
    }
}