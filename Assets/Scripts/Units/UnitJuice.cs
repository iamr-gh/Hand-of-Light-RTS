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
    [SerializeField] float movementSpeedAudioThreshold = 0.5f;
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
        if (!movementJuiceActive && navAgent.velocity.magnitude > movementSpeedAudioThreshold ) {
            StartCoroutine(MovementJuice());
        }

        // Sprite Rotation: Only start a turn after one is finished
        if (!isTurning) { 
            if (facing == "Left" && navAgent.velocity.x > 0)
            {
                facing = "Right";
                Quaternion targetRotation = Quaternion.Euler(-30, 180, 0);
                StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
            }
            else if (facing == "Right" && navAgent.velocity.x < 0)
            {
                facing = "Left";
                Quaternion targetRotation = Quaternion.Euler(30, 0, 0);
                StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
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

    IEnumerator RotateObject(Quaternion initialQuaternion, Quaternion targetQuaternion, float duration_sec)
    {
        isTurning = true;
        float initial_time = Time.time;
        float progress = 0;
        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / duration_sec;
            Quaternion newRotation = Quaternion.Lerp(initialQuaternion, targetQuaternion, progress);
            transform.rotation = newRotation;
            yield return null;
        }
        transform.rotation = targetQuaternion;
        isTurning = false;
    }
}