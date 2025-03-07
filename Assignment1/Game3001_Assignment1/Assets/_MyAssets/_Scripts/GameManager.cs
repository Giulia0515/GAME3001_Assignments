using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] GameObject enemyPrefab;

    private GameObject character;
    private GameObject target;
    private GameObject enemy;

    private AudioSource audioSource;
    public AudioClip meow;
    public AudioClip growl;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnAndSetBehavior(SteeringBehaviours.Behavior.Seek);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnAndSetBehavior(SteeringBehaviours.Behavior.Flee);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnAndSetBehavior(SteeringBehaviours.Behavior.Arrive);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnAndSetBehavior(SteeringBehaviours.Behavior.Avoid);
        }
    }

    void SpawnAndSetBehavior(SteeringBehaviours.Behavior behavior)
    {
        if (character != null)
        {
            Destroy(character);
        }

        DestroyTargets();

        Vector2 randomPosition1 = GetRandomPositionWithinBounds();
        Vector2 randomPosition2 = GetRandomPositionWithinBounds();

        if (behavior == SteeringBehaviours.Behavior.Flee)
        {
            randomPosition2 = randomPosition1 + (Random.insideUnitCircle * 2f);
            randomPosition2 = ClampPositionWithinBounds(randomPosition2);
            PlayDogSound();
        }


        switch (behavior)
        {
            case SteeringBehaviours.Behavior.Seek:
                character = Instantiate(characterPrefab, randomPosition1, Quaternion.identity);
                target = Instantiate(targetPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().target = target.transform;
                break;
            case SteeringBehaviours.Behavior.Flee:
                character = Instantiate(characterPrefab, randomPosition1, Quaternion.identity);
                enemy = Instantiate(enemyPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().enemy = enemy.transform;
                break;
            case SteeringBehaviours.Behavior.Arrive:
                character = Instantiate(characterPrefab, randomPosition1, Quaternion.identity);
                target = Instantiate(targetPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().target = target.transform;
                break;
            case SteeringBehaviours.Behavior.Avoid:
                Vector2 enemyPosition = randomPosition1;
                Vector2 characterPosition = randomPosition1 - new Vector2(5, 0);
                Vector2 targetPosition = randomPosition1 + new Vector2(4, 0);

                characterPosition = ClampPositionWithinBounds(characterPosition);
                targetPosition = ClampPositionWithinBounds(targetPosition);

                if (enemy != null) Destroy(enemy);
                enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
                target = Instantiate(targetPrefab, targetPosition, Quaternion.identity);
                character = Instantiate(characterPrefab, characterPosition, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().target = target.transform;
                character.GetComponent<SteeringBehaviours>().enemy = enemy.transform;
                PlayDogSound();
                break;
        }

        character.GetComponent<SteeringBehaviours>().currentBehavior = behavior;
        character.GetComponent<SteeringBehaviours>().enabled = true;
        PlayCatSound();
    }

    void PlayCatSound()
    {
        if (audioSource != null && meow != null)
        {
            audioSource.PlayOneShot(meow);
        }
    }

    void PlayDogSound()
    {
        if (audioSource != null && growl != null)
        {
            audioSource.PlayOneShot(growl);
        }
    }

    void DestroyTargets()
    {
        if (target != null) Destroy(target);
        if (enemy != null) Destroy(enemy);
    }

    Vector2 GetRandomPositionWithinBounds()
    {
        Vector3 screenBottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenTopRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane)); 
        float randomX = Random.Range(screenBottomLeft.x, screenTopRight.x);
        float randomY = Random.Range(screenBottomLeft.y, screenTopRight.y);

        return new Vector2(randomX, randomY);
    }

    Vector2 ClampPositionWithinBounds(Vector2 position)
    {
        Vector3 screenBottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenTopRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float clampedX = Mathf.Clamp(position.x, screenBottomLeft.x, screenTopRight.x);
        float clampedY = Mathf.Clamp(position.y, screenBottomLeft.y, screenTopRight.y);

        return new Vector2(clampedX, clampedY);
    }
}
