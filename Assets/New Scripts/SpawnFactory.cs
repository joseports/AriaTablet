using UnityEngine;
using UnityEngine.Networking;

namespace Assets.New_Scripts
{
    public static class SpawnFactory
    {
        public static GameObject Spawn(string resourceId, Vector3 position, Quaternion rotation)
        {
            var prefab = Resources.Load<GameObject>(resourceId);
            var instance = (GameObject)GameObject.Instantiate(prefab, position, rotation);

            NetworkServer.Spawn(instance);
            return instance;
        }

    }
}
