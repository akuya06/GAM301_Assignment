using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;
    public Camera cutsceneCam;
    public Camera playerCam;

    void Start()
    {
        cutsceneCam.enabled = true;
        playerCam.enabled = false;

        director.stopped += OnCutsceneEnd;
    }

    void OnCutsceneEnd(PlayableDirector d)
    {
        cutsceneCam.enabled = false;
        playerCam.enabled = true;
        // Bật lại script ZombieSpawn sau 3 giây
        StartCoroutine(EnableZombieSpawnAfterDelay(3f));
    }

    System.Collections.IEnumerator EnableZombieSpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ZombieSpawn zombieSpawn = FindObjectOfType<ZombieSpawn>();
        if (zombieSpawn != null)
        {
            zombieSpawn.enabled = true;
        }
    }
}
