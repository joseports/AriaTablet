using UnityEngine;

namespace Assets.New_Scripts
{
    public class PersistentObjectData : MonoBehaviour {

        void Awake()
        {

            DontDestroyOnLoad(transform.gameObject);
        }
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
