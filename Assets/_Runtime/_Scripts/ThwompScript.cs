using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThwompScript : MonoBehaviour
{
    private bool smashAllowed = true;
    private bool goingDown = false;
    private bool goingUp = false;
    private float startHeight;

    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        startHeight = transform.position.y;
    }

    void Update()
    {
        // if(goingDown)
        //     transform.Translate(Vector3.down * Time.deltaTime * 1.5f, Space.World);
        if(goingUp)
            transform.Translate(Vector3.up * Time.deltaTime * 1.5f, Space.World);

        if(goingUp && transform.position.y >= startHeight){
            goingUp = false;
            smashAllowed = true;
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag.Equals("Player")){
            if(smashAllowed)
                StartCoroutine(Smash());
        }
        // if(collider.gameObject.tag.Equals("Map")){
        //     goingDown = false; 
        // }

        if(collider.gameObject.layer.Equals(8) && collider.gameObject.tag.Equals("Map")){
            print("TOUCHED MAP");
            goingDown = false;
            // StartCoroutine(ResetSmash());
        }
    }

    private void OnTriggerStay(Collider collider) {
        if(collider.gameObject.tag.Equals("Player)")){
            print("ROTATING");
            Vector3 newDir = Vector3.RotateTowards(transform.forward, collider.gameObject.transform.position, 1f, 0f);
            transform.Rotate(newDir);
        }
    }

    private void OnCollisionEnter(Collision other) {
        // print("======");
        // print(other.collider.gameObject.layer);
        // print("======");

        // if(other.gameObject.tag.Equals("Map")){
        //     goingDown = false; 
        //     print("TOUCHED MAP");

        // }
    }

    IEnumerator Smash(){
        smashAllowed = false;
        yield return new WaitForSeconds(Random.Range(0.1f, 2.0f));
        rb.useGravity = true;
        yield return new WaitForSeconds(3f);
        rb.useGravity = false;
        goingUp = true;
    }

    // IEnumerator ResetSmash(){
    //     rb.useGravity = false;
    //     yield return new WaitForSeconds(3f);
    //     goingUp = true;
    // }
}
