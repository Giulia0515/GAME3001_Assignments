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

        character = Instantiate(characterPrefab, randomPosition1, Quaternion.identity);

        switch (behavior)
        {
            case SteeringBehaviours.Behavior.Seek:
                target = Instantiate(targetPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().target = target.transform;
                break;
            case SteeringBehaviours.Behavior.Flee:
                enemy = Instantiate(enemyPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().enemy = enemy.transform;
                break;
            case SteeringBehaviours.Behavior.Arrive:
                target = Instantiate(targetPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().target = target.transform;
                break;
            case SteeringBehaviours.Behavior.Avoid:
                enemy = Instantiate(enemyPrefab, randomPosition2, Quaternion.identity);
                character.GetComponent<SteeringBehaviours>().enemy = enemy.transform;
                break;
        }

        character.GetComponent<SteeringBehaviours>().currentBehavior = behavior;
        character.GetComponent<SteeringBehaviours>().enabled = true;
    }

    void DestroyTargets()
    {
        if (target != null) Destroy(target);
        if (enemy != null) Destroy(enemy);
    }

    Vector2 GetRandomPositionWithinBounds()
    {
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        float randomX = Random.Range(screenBottomLeft.x, screenTopRight.x);
        float randomY = Random.Range(screenBottomLeft.y, screenTopRight.y);

        return new Vector2(randomX, randomY);
    }
}
