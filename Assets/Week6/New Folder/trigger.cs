using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trigger : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("wall")){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        

    }
    
}
