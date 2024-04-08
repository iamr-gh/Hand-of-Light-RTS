using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitJuice : MonoBehaviour
{
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

    private float moveStartTime;

    UnitParameters parameters;
    GameObject attackSprite;

    private bool damageJuiceActive = false;
    private bool movementJuiceActive = false;

    // Start is called before the first frame update
    void Start()
    {
        attackSound = Resources.Load<AudioClip>(attackAudio);
        damageSound = Resources.Load<AudioClip>(damageAudio);
        deathSound = Resources.Load<AudioClip>(deathAudio);
        movementSound = Resources.Load<AudioClip>(movementAudio);
        parameters = GetComponentInParent<UnitParameters>();
        navAgent = GetComponentInParent<NavMeshAgent>();
        //attackAnimation = GetComponent<>
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementJuiceActive && navAgent.velocity.magnitude >= movementSpeedThresholdForAudio ) {
            //StartCoroutine(MovementJuice()); // Commented out because it's really loud and poorly implemented rn
        }

        // Sprite 'Hopping'
        if (navAgent.velocity.magnitude < 0.5f) { // Randomize the start time so they all "hop" differently
            moveStartTime = Time.time + Random.Range(0f, 0.3f); // Update the start time
        }
        else
        {
            float positionInCycle = Mathf.Abs(Mathf.Sin(2 * Mathf.PI * (Time.time - moveStartTime)));
            transform.position = transform.parent.position + hopHeight * transform.up * positionInCycle;
        }

        // Sprite Rotation: Only start a turn after one is finished
        if (!isTurning) { 
            if (facing == "Left" && navAgent.velocity.x > 0)
            {
                facing = "Right";
                StartCoroutine(RotateObject(180f, turnDuration));
            }
            else if (facing == "Right" && navAgent.velocity.x < 0)
            {
                facing = "Left";
                StartCoroutine(RotateObject(180f, turnDuration));
            }
        } 
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // Destroy dead unit
        if (parameters.getHP() <= 0)
        {
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
            Destroy(transform.parent.gameObject);
        }
    }

    IEnumerator MovementJuice() {
        movementJuiceActive = true;

        // Play audio clip and sleep for it's length
        AudioSource.PlayClipAtPoint(movementSound, Camera.main.transform.position);
        yield return new WaitForSeconds(movementSound.length);

        movementJuiceActive = false;
    }

    IEnumerator AttackJuice() {
        AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position);
        attackSprite.SetActive(true);
        yield return new WaitForSeconds(parameters.getAttackDuration());
        attackSprite.SetActive(false);
    }

    IEnumerator DamageJuice() {
        if (damageJuiceActive) { yield break; }
        damageJuiceActive = true;

        AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position);
        yield return new WaitForSeconds(damageSound.length);

        damageJuiceActive = false;
    }

    IEnumerator RecallJuice() { yield break; }

    IEnumerator RotateObject(float angle, float duration_sec)
    {
        isTurning = true;
        float initialTime = Time.time;
        float angularVelocity = angle / duration_sec;
        float deltaAngle = 0;
        while (Mathf.Abs(Time.time - initialTime) < duration_sec)
        {
            transform.RotateAround(transform.position, transform.up, angularVelocity * Time.deltaTime);
            deltaAngle += angularVelocity * Time.deltaTime;
            yield return null;
        }
        transform.RotateAround(transform.position, transform.up, angle - deltaAngle);

        isTurning = false;
    }
}