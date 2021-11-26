using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/Vector3.MoveTowards.html
public class BobombScript : MonoBehaviour
{
    public Rigidbody rb;
    private bool awake = false;
    private GameObject target;
    void Start()
    {
        target = GameObject.Find("Mario").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(awake){
            rb.useGravity = false;
            if(Vector3.Distance(transform.position, target.transform.position) > 0.5f){
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 4.0f * Time.deltaTime);
                Quaternion rot = Quaternion.LookRotation(-target.transform.position + transform.position, Vector3.up);
                transform.rotation = rot;
            }else{ // try to explode
                target.GetComponent<PlayerMovement>().PlayerDamage();
                Destroy(transform.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag.Equals("Player")){
            print("Bobomb found player");
            awake = true;
            rb.useGravity = true;
        }
    }
}
