using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BooScript : MonoBehaviour
{
    public Transform[] waypoints;
    public bool awake = false;
    private int index = 0;

    // Update is called once per frame
    void Update()
    {
        if(index >= 13){
            print("DONE");
            awake = false;
            SceneManager.LoadScene("Assets/_Runtime/_Scenes/BombombBattlefield.unity");
        }
        if(awake){
            Vector3 newPos = waypoints[index].position;
            if(Vector3.Distance(transform.position, newPos) > 0.5f){
                    transform.position = Vector3.MoveTowards(transform.position, newPos, 3.0f * Time.deltaTime);
                    Quaternion rot = Quaternion.LookRotation(newPos - transform.position, Vector3.up);
                    transform.rotation = rot;
            }else{ // next waypoint
                index++;
            }
        }
        
    }
}
