using UnityEngine;
using System.Collections;
using PsychImmersion;

public class WandMouse : DifficultySensitiveBehaviour
{
    //rigidbody of the mouse
    Rigidbody rb;
    //distance from center of circle to object
    public float circleDistance;
    //radius of circle
    public float radius;

    //position of circle
    public Vector3 circleCenter;

    //displacement vector for wander angle calculations
    Vector3 displacement;

    //wander angle
    float wanderAngle;

    //degree to change angle by
    public float angleChange;

    //walls from which we apply force
    public Transform wall1;
    public Transform wall2;
    public Transform wall3;
    public Transform wall4;


    //the force we add to the object
    Vector3 forceToAdd;

    Vector3 counter1;
    Vector3 counter2;
    Vector3 counter3;
    Vector3 counter4;

    //used to change animation blend tree
    Quaternion slerp;
    Quaternion dirQ;


    //used for preventing stalling

    Vector3[] previousDirections;
    Animator anim;
    bool isStopped;
    bool shouldRotate;
    bool checkForStall;

    //declaring up here improves coroutine performance
    WaitForSeconds pointOne;
    WaitForSeconds twoPointFive;
    WaitForSeconds one;

    //for the coroutine which adds wall force
    public bool notLevel3 = true;

    public float MinWallDistance = 0.2f;
    public float WallStrength = 0.1f;


    /// <summary>
    /// initializes the methods
    /// </summary>
    void Start()
    {

        //starts in a random direction(this will be the direction the mouse begins to move in)
        displacement = new Vector3(0, 0, 1);
        //10 is the number of ticks we check for stalling (ticks of similar position / rotation is default "stalling")
        previousDirections = new Vector3[10];
        isStopped = false;
        shouldRotate = true;
        checkForStall = false;
        pointOne = new WaitForSeconds(.1f);
        twoPointFive = new WaitForSeconds(2.5f);
        one = new WaitForSeconds(1f);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        //fill previous directions with random vector3's for the inital IsGoingToStall method
        for (int ii = 0; ii < previousDirections.Length; ii++)
        {
            previousDirections[ii] = Random.onUnitSphere * 100f;
            previousDirections[ii].y = 0;
        }
        StartCoroutine(AdjustWallForce());          //Starts calculating the force so that the spider does not hit the wall 
    }

    /// <summary>
    /// framerate independent update
    /// </summary>
    void FixedUpdate()
    {
        wander();
        WallForce();
        RotateToVelocity(100f);
        AnimController();
    }

    /// <summary>
    /// Calls the functions which make the wander force and adds it
    /// </summary>
    void wander()
    {
        drawCircle();         
        forceToAdd = drawWanderForce();             //sets the force to be of the correct magnitude and in the direction of the wander angle
        forceToAdd.y = 0;
        rb.AddForce(forceToAdd * 3f);               //adds the force

    }

    /// <summary>
    /// Adjust animations to match movement
    /// </summary>
    void AnimController()
    {
        if (rb == null)
            Debug.Log("Rigidbody was null");
        else
        {
            //if isIdle is true then the animation would not play > anim.speed set to 0 becaute there is no velocity
            if (anim.GetBool("isIdle") == false)
                anim.speed = rb.velocity.magnitude * 9;
            //these variables are from RotateToVelocity
            float target = dirQ.eulerAngles.y;
            float current = slerp.eulerAngles.y;

            //if the difference is such that the target is really low (0) and the current rotation is really high (359) we want to rotate towards 360, not 0
            if (target - current < -180f)
                target = target + 360f;
            if (target - current > 180f)
                target = target - 360f;
            float difference = (target - current) / 360f;
            anim.SetFloat("Blend", difference);
        }


    }

    /// <summary>
    /// Adds the secondary wall force,
    /// used by fixedupdate,
    /// counter is set in "adjustwallforce"
    /// </summary>
    void WallForce()
    {
        rb.AddForce(counter1);
        rb.AddForce(counter2);
        rb.AddForce(counter3);
        rb.AddForce(counter4);
    }

    /// <summary>
    ///  draws the circle, sets the displacement and changes the wander angle for the next calculation
    /// </summary>
    void drawCircle()
    {
        //makes a circle ahead of the object
        circleCenter = rb.velocity;
        circleCenter.Normalize();
        circleCenter *= circleDistance;

        displacement *= radius;
        displacement.z += circleDistance;

        //changes the angle to match the new displacement we just found
        SetAngle(ref displacement, wanderAngle);
        //adds (random value - .5f) (angleChange) angle change is set in inspector
        wanderAngle += (Random.value * angleChange) - (angleChange * .5f);
    }

    /// <summary>
    /// helper method for changeWanderAngle 
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="angle"></param>
    void SetAngle(ref Vector3 vector, float angle)
    {
        float len = vector.magnitude;
        vector.x = Mathf.Cos(angle) * len;
        vector.z = Mathf.Sin(angle) * len;
    }

    /// <summary>
    /// return the force to be used for wandering 
    /// </summary>
    /// <returns></returns>
    Vector3 drawWanderForce()
    {
        Vector3 wanderForce;
        wanderForce = circleCenter + displacement;
        if (wanderForce.x > .10f)
            wanderForce.x = .10f;
        if(wanderForce.x < -.10f)
            wanderForce.x = -.10f;
        if (wanderForce.z < -.10f)
            wanderForce.z = -.10f;
        if (wanderForce.z > .10f)
            wanderForce.z = .10f;
        return wanderForce;
    }

    /// <summary>
    /// add forces from walls,
    /// sets checkForStall used in rorateToVelocity
    /// </summary>
    /// <returns></returns>
    IEnumerator AdjustWallForce()
    {
        while (notLevel3)
        {
            // one for each wall, wall is target.position
            float distanceToTarget1 = Mathf.Min(MinWallDistance, transform.localPosition.x - wall1.localPosition.x);
            float distanceToTarget2 = Mathf.Min(MinWallDistance, -transform.localPosition.x + wall2.localPosition.x);
            float distanceToTarget3 = Mathf.Min(MinWallDistance, transform.localPosition.z - wall3.localPosition.z);
            float distanceToTarget4 = Mathf.Min(MinWallDistance, -transform.localPosition.z + wall4.localPosition.z);

            // (constant/ distance to target) = magnitude * (direction based on wall number) = vector from each wall
            counter1 = (WallStrength / distanceToTarget1) * wall1.transform.forward;
            counter2 = (WallStrength / distanceToTarget2) * wall2.transform.forward;
            counter3 = (WallStrength / distanceToTarget3) * wall3.transform.forward;
            counter4 = (WallStrength / distanceToTarget4) * wall4.transform.forward;


            // change the counterForce once every second
            yield return pointOne;
            checkForStall = true;
        }
    }

    /// <summary>
    /// Rotate to face the direction it's moving
    /// </summary>
    /// <param name="turnSpeed"></param>
    void RotateToVelocity(float turnSpeed)
    {
        Vector3 dir;
        //model not imported facing backwards so we do not set this to be negative so it faces the right way
        dir = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Feed the current direction into the check for stalling method
        if (checkForStall)
        {
            if (IsGoingToStall(dir) == true)
                shouldRotate = false;
        }

        //rotate if velocity is greater than 0
        if ((Mathf.Abs(dir.x) > 0 || Mathf.Abs(dir.z) > 0) && shouldRotate == true)
        {
            //slerps to the direction the mouse is moving
            dirQ = Quaternion.LookRotation(dir);
            slerp = Quaternion.Slerp(transform.rotation, dirQ, dir.magnitude * turnSpeed * Time.deltaTime);
            rb.MoveRotation(slerp);
        }
        

    }
    
    /// <summary>
    /// checks to see if the mouse is jamming up against the wall (or sliding down it)
    /// </summary>
    /// <param name="currentDirection"></param>
    bool IsGoingToStall(Vector3 currentDirection)
    {
        //cycle through previous directions and step them back;
        for (int ii = 0; ii < previousDirections.Length - 1; ii++)
            previousDirections[ii] = previousDirections[ii + 1];
        //set the end value to the currentDirection
        previousDirections[previousDirections.Length - 1] = currentDirection;

        Vector3 differenceInDirections = Vector3.zero;
        for (int ii = 1; ii < previousDirections.Length - 1; ii++)
        {
            differenceInDirections.x += Mathf.Abs((previousDirections[ii].x - previousDirections[ii - 1].x));
            differenceInDirections.z += Mathf.Abs((previousDirections[ii].z - previousDirections[ii - 1].z));
        }

        //hard coded threshold do decide if the mouse was stationary enough to play the idle animation (.004 without wall force)
        if (differenceInDirections.x < .004 || differenceInDirections.z < .004)
        {
            //keep # of coroutines at 1
            if (!isStopped)
            {
                isStopped = true;
                //stop the object from moving while we play the idle animation
                rb.constraints = RigidbodyConstraints.FreezeAll;

                //stop wander force for duration of idle animation
                StartCoroutine(ChangeAnimations(Vector3.one));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// if rotation has not changed in X ticks and transform.x or transform.y 
    ///  have not changed in X ticks, move to idle animation, then use rootmotion to turn the mouse aruond and restart the wander script
    /// </summary>
    /// <param name="temp"></param>
    /// <returns></returns>
    IEnumerator ChangeAnimations(Vector3 temp)
    {

        anim.SetBool("isIdle", true);
        anim.SetFloat("IdleBlend", Random.value * 2 - 1);
        if (Random.value > .5)
            anim.SetBool("Mirror", true);
        else
            anim.SetBool("Mirror", false);

        //let idle animation play for 2.5 seconds
        yield return twoPointFive;

        //Re-add constraints
        shouldRotate = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

        anim.SetBool("isIdle", false);

        //wait for a second before letting the mouse idle again
        yield return one;
        isStopped = false;
        
    }

    public override void SetLevel(Difficulity level)
    {
        if (level != Difficulity.Adjustment) {
            enabled = true;
            rb.isKinematic = false;
        } else {
            enabled = false;
            rb.isKinematic = true;
            return;
        }
        notLevel3 = level != Difficulity.Advanced;
    }
}
