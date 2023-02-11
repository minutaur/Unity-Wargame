using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Week5;

public class MySpawner : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPrefab;
    public float spawnspan = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine( Spawn());
        player = FindObjectOfType<ThirdPersonController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnspan);
            GameObject go = Instantiate(spawnPrefab);
            go.transform.position = transform.position;
            go.GetComponent<NavTest>().target = player.transform;
        }
    }
}
