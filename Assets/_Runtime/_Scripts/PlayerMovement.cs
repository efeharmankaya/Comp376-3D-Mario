// ------------------------------------------------------------------------------
// Quiz
// Written by: Efe Harmankaya - 40077277
// For COMP 376 – Fall 2021
// Controls the basic playermovement parameters and additional movement based collision logic (ie. enemies, coins, speed boost, etc...)
// -----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CharacterController controller;
    [SerializeField, Min(0)] private float speed = 5f;
    [SerializeField, Min(0)] private float rotationSpeed = 10f;

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField, Min(0)] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField, Min(0)] private float jumpHeight = 2f;

    private Vector3 movement;
    private Vector3 gravitationalForce;
    private bool isGrounded;
    private bool jumpMomentumCheck;

    public AudioSource jumpSound;
    public AudioSource coinPickupSound;
    public Animator animator;
    private int health = 3;
    private int coinsCollected = 0;
    public Text coinsText;
    public Text startRaceText;
    private float speedBoostDelta = 5f;
    private float speedBoostDuration = 5f;
    private bool allowSpeedUp = true;
    private bool allowGetHurt = true;

    private GameObject heart1, heart2, heart3;
    private void Start(){
        heart1 = GameObject.Find("heart1");
        heart2 = GameObject.Find("heart2");
        heart3 = GameObject.Find("heart3");
    }

    private void Update()
    {
        // calculate movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        movement = right * horizontal + forward * vertical;

        // check if player is trying to move
        if (movement != Vector3.zero)
        {
            // look in the direction of the movement
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotationSpeed * Time.deltaTime);
            animator.SetBool("Run", true);
        }else{
            animator.SetBool("Run", false);
        }

        // check if mario is grounded
        isGrounded = false;
        Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, whatIsGround);
        for (int i = 0; i < hitColliders.Length; ++i)
        {
            if (hitColliders[i].gameObject == this.gameObject)
                continue;

            if (!hitColliders[i].isTrigger)
            {
                isGrounded = true;
                break;
            }
        }

        jumpMomentumCheck = jumpMomentumCheck && Input.GetButton("Jump") && !isGrounded;

        // simulate gravity
        if (isGrounded)
        {
            // mario is standing on ground
            gravitationalForce.y = gravity * Time.deltaTime;
            jumpMomentumCheck = true;
            animator.SetBool("Jump", false);
        }
        else
        {
            // mario is in the air
            if (!jumpMomentumCheck && gravitationalForce.y > 0){
                gravitationalForce.y = 0;
            }
            else
            {
                gravitationalForce.y += gravity * Time.deltaTime;
            }
        }

        // jump
        if (Input.GetButton("Jump") && isGrounded){
            jumpSound.Play();
            gravitationalForce.y = Mathf.Sqrt(-2 * jumpHeight * gravity);
            animator.SetBool("Run", false);
            animator.SetBool("Jump", true);
        }

        if (Input.GetKey(KeyCode.Mouse0)) // pressed left click
            animator.SetBool("Punch", true);
        
        if (Input.GetKeyUp(KeyCode.Mouse0)) // left click released
            animator.SetBool("Punch", false);
        

        // if (Input.GetKeyDown(KeyCode.Escape)) // stop game condition
        //     UnityEditor.EditorApplication.isPlaying = false;
        

        // move mario
        controller.Move((movement * speed * Time.deltaTime) + (gravitationalForce * Time.deltaTime));

        if(Input.GetKeyDown(KeyCode.Alpha1)) // testing hurting mechanics
            StartCoroutine(getHurt());
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.gameObject.name);
        if(collision.collider.gameObject.tag.Equals("Thwomp")){
            if(allowGetHurt)
                StartCoroutine(getHurt());
            print("***********");
            print("DEAD");
            print("***********");
        }
    }

    // private void OnCollisionStay(Collision collision)
    // {
    //     // print(collision.collider.gameObject.name);
    //     // if(collision.collider.gameObject.tag.Equals("Crate") && Input.GetKeyDown(KeyCode.Mouse0)){
            
    //     // }
    // }

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.tag.Equals("Boo") && !GameObject.Find("Boo").GetComponent<BooScript>().awake){
            print("in range of boo");
            startRaceText.gameObject.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E)){
                startRaceText.gameObject.SetActive(false);
                GameObject.Find("Boo").GetComponent<BooScript>().awake = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag.Equals("Boo")){
            startRaceText.gameObject.SetActive(false);
        }
    }

    private void collectCoin(){
        coinsCollected++;
        coinPickupSound.Play();
        coinsText.text = "Coins: " + coinsCollected;
        if(allowSpeedUp)
            StartCoroutine(SpeedUp());
    }

    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag.Equals("Coin")){
            print("coin");
            collectCoin();
            Destroy(collider.gameObject);
        }else if(collider.gameObject.tag.Equals("RedCoin")){
            print("red coin");
            collectCoin();
            Destroy(collider.gameObject);
        }else if(collider.gameObject.tag.Equals("Flag")){
            SceneManager.LoadScene("Assets/_Runtime/_Scenes/BombombBattlefield.unity");
        }
    }

    public void PlayerDamage(){
        if(allowGetHurt)
            StartCoroutine(getHurt());
    }

    IEnumerator getHurt(){
        allowGetHurt = false;
        removeHealth();
        yield return new WaitForSeconds(2f);

        allowGetHurt = true;
    }
    void removeHealth(){
        switch(health){
            case 3:
                heart1.SetActive(false);
                break;
            case 2:
                heart2.SetActive(false);
                break;
            case 1:
                heart3.SetActive(false);
                break;
            default:
                break;
        }

        if(--health <= 0)
            SceneManager.LoadScene("Assets/_Runtime/_Scenes/BombombBattlefield.unity");
    }

    IEnumerator SpeedUp(){
        allowSpeedUp = false;
        speed += speedBoostDelta;
        yield return new WaitForSeconds(speedBoostDuration);
        speed -= speedBoostDelta;
        allowSpeedUp = true;
    }
}
