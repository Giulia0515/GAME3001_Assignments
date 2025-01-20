using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject characterPrefab;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] GameObject enemyPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
    }

}
