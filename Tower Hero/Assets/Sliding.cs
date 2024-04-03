using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.C;
    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        slideTimer = maxSlideTime;

        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && pm.grounded)
        {
            StartSliding();
        }

        if (Input.GetKeyUp(slideKey) && pm.sliding)
        {
            StopSliding();
        }
    }

    private void FixedUpdate()
    {
        if(pm.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSliding()
    {
        pm.sliding = true;

        transform.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //  normal sliding
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        //  slope sliding
        else 
        {
            rb.AddForce(pm.GetSlopeMovementDirection(inputDirection) * slideForce, ForceMode.Force);
        }
        

        if(slideTimer <= 0)
        {
            StopSliding();
        }
    }

    private void StopSliding()
    {
        pm.sliding = false;

        transform.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
