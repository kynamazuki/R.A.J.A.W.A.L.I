using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Manage a set of waves.
    /// </summary>
    public class WavesController : MonoBehaviour
    {
        [Header("General")]

        [SerializeField]
        protected List<WaveController> waveControllers = new List<WaveController>();
        public List<WaveController> WaveControllers { get { return waveControllers; } }

        [SerializeField]
        protected bool loopWaves = false;

        [SerializeField] private int totalLevels = 2;

        [SerializeField, Tooltip("Name of the Loadout scene to return to after mission complete.")]
        protected string loadoutSceneName = "LoadoutScene";  // <-- assign in inspector

        protected int lastSpawnedWaveIndex = -1;
        public int LastSpawnedWaveIndex
        {
            get { return lastSpawnedWaveIndex; }
        }

        protected bool wavesDestroyed = false;

        [Header("Events")]

        public UnityEvent onWavesDestroyed;


        protected virtual void Awake()
        {
            foreach (WaveController waveController in waveControllers)
            {
                waveController.onWaveDestroyed.AddListener(OnWaveDestroyed);
            }
        }

        /// <summary>
        /// Spawn a wave at a specific index in the list.
        /// </summary>
        /// <param name="index">The wave index to spawn.</param>
        public virtual void SpawnWave(int index)
        {

            if (index < 0 || index >= waveControllers.Count) return;

            waveControllers[index].Spawn();
            lastSpawnedWaveIndex = index;
        }

        /// <summary>
        /// Spawn a random wave in the list.
        /// </summary>
        public virtual void SpawnRandomWave()
        {
            SpawnWave(Random.Range(0, waveControllers.Count));
        }

        /// <summary>
        /// Spawn the next wave in the list.
        /// </summary>
        public virtual void SpawnNextWave()
        {
            // Iterate
            int nextWaveSpawnIndex = lastSpawnedWaveIndex + 1;
            if (nextWaveSpawnIndex >= waveControllers.Count)
            {
                if (loopWaves)
                {
                    ResetWaves();
                    nextWaveSpawnIndex = 0;
                }
                else
                {
                    return;
                }
            }

            SpawnWave(nextWaveSpawnIndex);

        }

        public virtual void ResetWaves()
        {
            // Make sure all the wave controllers are reset
            foreach (WaveController waveController in waveControllers)
            {
                waveController.ResetWave();
            }

            // Reset destroyed flag
            wavesDestroyed = false;
        }

        protected virtual void OnWaveDestroyed()
        {
            // Check if all the waves have been destroyed
            if (!wavesDestroyed)
            {
                wavesDestroyed = true;
                for (int i = 0; i < waveControllers.Count; ++i)
                {
                    if (!waveControllers[i].Destroyed)
                    {
                        wavesDestroyed = false;
                    }
                }

                if (wavesDestroyed)
                {
                    // ===== CAMPAIGN PROGRESS =====
                    var dataManager = FindObjectOfType<VSX.UniversalVehicleCombat.Loadout.LoadoutDataManagerJSON>();

                    if (dataManager != null)
                    {
                        var data = dataManager.LoadData();

                        if (data == null)
                            data = new VSX.UniversalVehicleCombat.Loadout.LoadoutData();

                        // store popup info before increment
                        data.showUnlockPopup = true;
                        data.lastCompletedMissionIndex = data.currentMissionIndex;

                        // progress++
                        data.currentMissionIndex++;
                        data.campaignScore = LeaderboardManager.Instance.currentScore;

                        Debug.Log("MISSION COMPLETE → NEW INDEX: " + data.currentMissionIndex);

                        dataManager.SaveData(data);
                    }
                    else
                    {
                        Debug.LogError("LoadoutDataManagerJSON NOT FOUND IN MISSION SCENE!");
                    }
                    // =============================

                    onWavesDestroyed.Invoke();

                    // CHECK IF FINAL LEVEL


                    var dataManager2 = FindObjectOfType<VSX.UniversalVehicleCombat.Loadout.LoadoutDataManagerJSON>();

                    if (dataManager2 != null)
                    {
                        var data2 = dataManager2.LoadData();

                        if (data2.currentMissionIndex >= totalLevels)
                        {
                            Debug.Log("FINAL LEVEL COMPLETE");

                            data2.showFinalLeaderboard = true;
                            data2.showUnlockPopup = false;

                            dataManager2.SaveData(data2);

                            StartCoroutine(ReturnToLoadoutAfterDelay());
                            return;
                        }
                    }

                    // Otherwise → normal flow
                    StartCoroutine(ReturnToLoadoutAfterDelay());
                }
            }
        }

        IEnumerator ReturnToLoadoutAfterDelay()
        {
            yield return new WaitForSeconds(2f); // short pause after win

            // Fade out
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeOut();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(loadoutSceneName);
        }

        IEnumerator ShowFinalLeaderboardAfterDelay()
        {
            yield return new WaitForSeconds(2f);

            if (ScreenFader.Instance != null)
                yield return ScreenFader.Instance.FadeOut();

            UnityEngine.SceneManagement.SceneManager.LoadScene(loadoutSceneName);

            yield return new WaitForSeconds(1f);

            if (PlayerProfileUI.Instance != null)
            {
                PlayerProfileUI.Instance.ShowAfterDeath();
            }
        }
    }
}

