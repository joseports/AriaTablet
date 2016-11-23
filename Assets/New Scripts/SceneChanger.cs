using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.New_Scripts
{
    public class SceneChanger : MonoBehaviour {
        public bool Scene1;
        public bool Scene2;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "GameController")
            {
                if (Scene1)
                {
                               
                    SceneManager.LoadScene("Scene2");
                }
           
            }


        }
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
