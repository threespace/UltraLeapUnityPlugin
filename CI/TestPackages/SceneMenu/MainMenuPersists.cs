using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPersists : MonoBehaviour
{
    public GameObject StagePrefab;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        ResetScene();

        void ResetScene()
        {
            foreach(Transform obj in transform)
            {
                Destroy(obj.gameObject);
            }
            
            // TODO: Find out of there's an interaction manager in this scene, if not then instantiate one.
            
            Instantiate(StagePrefab, transform);
        }

        void ResetSceneWithDiscards(Scene _, Scene __) => ResetScene();

        SceneManager.activeSceneChanged += ResetSceneWithDiscards;
    }
}