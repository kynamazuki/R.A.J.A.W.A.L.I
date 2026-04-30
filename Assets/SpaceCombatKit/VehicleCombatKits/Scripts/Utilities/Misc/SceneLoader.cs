using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VSX.UniversalVehicleCombat.Loadout;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Load a scene using a name or a build index.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {

        public LoadoutManager loadoutManager;

        [SerializeField]
        protected string sceneName;

        /// <summary>
        /// Load a scene with the name set in the inspector.
        /// </summary>
        public void LoadScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Load a scene with a specified name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        public void LoadScene(string sceneName)
        {

            if (LeaderboardUI.Instance != null)
            {
                LeaderboardUI.Instance.HideLeaderboard();
            }

            SceneManager.LoadScene(sceneName);

            LoadoutManager.Instance.OnDestroy();
        }

        /// <summary>
        /// Load a scene with a specified build index.
        /// </summary>
        /// <param name="sceneBuildIndex">The build index of the scene to load.</param>
        public void LoadScene(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }


        /// <summary>
        /// Reload the current scene.
        /// </summary>
        public void ReloadActiveScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        public void ResetMission()
        {
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.currentMission = null;
            }
        }



        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitApplication()
        {
            Application.Quit();

        }



        /*public void OnApplicationQuit()
        {
            LeaderboardManager.Instance.SaveCurrentSession();
        }*/
    }
}


