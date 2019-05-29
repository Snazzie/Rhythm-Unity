using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class GameLaunchSetup : MonoBehaviour
    {
        private bool userControl;

        // Use this for initialization
        private GameObject _Cursor { get; set; }
        void Start()
        {
            Application.targetFrameRate = 300;
            _Cursor = GameObject.FindWithTag("Cursor");


            Debug.Log(SceneManager.GetActiveScene().name);

        }

        // Update is called once per frame


    }
}
