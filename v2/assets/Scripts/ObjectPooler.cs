using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField]
    private int NumberOfPooledItemsAllowed = 50;

    private int CurrentNumberOfItemsActive = 0;

    public static ObjectPooler Instance;

    [System.Serializable]
    public struct PoolableObject
    {
        // Number of objects instantiated and stored on startup
        public int pooledAmount;
        // Object to instantiate
        public GameObject obj;
    }

    // A list of all the objects that can be generated (objects generated from a pool)
    [SerializeField]
    List<PoolableObject> poolDefinitions = new List<PoolableObject>();

    Dictionary<string, List<GameObject>> poolDictionary;
    Dictionary<string, GameObject> prefabDictionary;

    void Awake()
    {
        Instance = this;


        poolDictionary = new Dictionary<string, List<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (PoolableObject p in poolDefinitions)
        {

            prefabDictionary.Add(p.obj.name, p.obj);
            poolDictionary.Add(p.obj.name, new List<GameObject>());

            for (int index = 0; index < p.pooledAmount; ++index)
            {
                poolDictionary[p.obj.name].Add(CreatePooledObject(prefabDictionary[p.obj.name]));
            }
        }
    }

    /// <summary>
    /// GetPooledObject - retrieves an inactive pooled object, or instantiates
    /// a new object and returns it.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <returns>An inactive pooled object, or null if the object type is not in the pool.</returns>
    public GameObject GetPooledObject(string name)
    {
        return GetFirstInactivePooledObject(name);
    }

    private GameObject GetFirstInactivePooledObject(string name)
    {
        foreach (var g in poolDictionary[name])
        {
            if (!g.activeInHierarchy)
                return g;
        }

        return null;
    }

    /// <summary>
    /// ReturnPooledObject - returns a GameObject to the pool.
    /// </summary>
    /// <param name="obj">The object to return to the pool.</param>
    public void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);
        --CurrentNumberOfItemsActive;
    }

    /// <summary>
    /// Creates a new object from a prefab, deactivates it, and returns it.
    /// </summary>
    /// <param name="prefab">The GameObject to instantiate.</param>
    /// <returns>The deactivated, instantiated game object.</returns>
    GameObject CreatePooledObject(GameObject prefab)
    {
        GameObject obj = (GameObject)Instantiate(prefab);
        obj.name = prefab.name;
        // Default object to inactive
        obj.SetActive(false);
        return obj;
    }
}