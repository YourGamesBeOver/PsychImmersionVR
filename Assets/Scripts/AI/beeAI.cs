using UnityEngine;
using PsychImmersion;
using PsychImmersion.Experiment;

public class beeAI : DifficultySensitiveBehaviour {
    
    //The difficulty level we are on
    public Difficulity diff;

    //the different waypoint arrays for beginning intermediate and advanced levels
    public Transform[] waypointsBeginner;
    public Transform[] waypointsIntermediate;
    public Transform[] waypointsAdvanced;

    //the current waypont point array we are on
    public Transform[] currentWaypoints;
    //the current waypoint
    public int cWaypoint = 0;

    //the waypoint's position
    private Vector3 targetPos;
    //the direction we are moving in
    public Vector3 moveDirection;

    //if we hit a wall
    public bool hitEnvironment;
    //direction we were going when we hit a wall
    public Vector3 toReverse;
    //counts down once moving in the opposite direction of the wall, at zero it goes back into its normal randomized movement
    public int counter;

    //deals with collisions
    string tagLevel;
    //we will start to ignore a collision with the waypoint if it has already collided once
    public bool firstCollision;
    //check if it has collided
    public bool checkForCollision;

    public Transform waypoint;

    public Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        //inital difficulty is beginner
        diff = Difficulity.Beginner;
        currentWaypoints = waypointsBeginner;
        waypoint = currentWaypoints[0];
        currentWaypoints[cWaypoint].gameObject.tag = "beginner current";
        counter = 0;
        hitEnvironment = false;
        firstCollision = true;
        rb = GetComponent<Rigidbody>();
    }
	
	void FixedUpdate () {
        //slerp to waypoint
        lookAtTarget();

        //if we do not need to reverse anything
        if (counter == 0)
            //just move normally
            Move();
        else if (counter > 0)
            //move in reverse
            Move(toReverse);
    }

    /// <summary>
    ///  Moves the bee
    /// </summary>
    void Move()
    {
        //Set target Position to waypoint position
        targetPos = currentWaypoints[cWaypoint].position;
        //stores the direction that the object was going when it hit the wall;
        toReverse = Random.insideUnitSphere;
        //Set direction to move towards
        moveDirection = (targetPos - transform.position + toReverse * 1f).normalized;
        //Move's the object
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime * .2f);
    }

    /// <summary>
    /// Moves the bee in reverse
    /// </summary>
    /// <param name="reverseVector"> 
    /// stores the vector it was going in when it crashed into the wall
    /// </param>
    void Move(Vector3 reverseVector)
    {
        //Set target Position to waypoint position
        targetPos = currentWaypoints[cWaypoint].position;
        //Set direction to move towards
        moveDirection = (targetPos - transform.position + toReverse * 2f).normalized;
        //moves the object the opposite direction of the wall
        rb.MovePosition(rb.position + (-reverseVector * 2f).normalized * Time.fixedDeltaTime * .2f);
        counter--;
    }


    /// <summary>
    /// enters a collider with a certain tag at a certain state
    /// </summary>
    /// <param name="other">
    /// the collider we are entering
    /// </param>
    void OnTriggerEnter(Collider other)
    {
        switch (diff)
        {
            case Difficulity.Tutorial:
                //do nothing; the simulation has not started
                break;
            case Difficulity.Beginner:
                //if the collider entered is the waypoint we want, go to the next one
                //if the collider entered is any other way point, do nothing
                //if the collider entered is a wall, bounce off
                BeginnerState(other);
                break;
            case Difficulity.Intermediate:
                IntermediateState(other);
                break;
            case Difficulity.Advanced:
                AdvancedState(other);
                break;
        }
    }

    /// <summary>
    /// Handles the beginner level AI waypoints
    /// </summary>
    /// <param name="other">
    /// what object we collided with
    /// </param>
    void BeginnerState(Collider other)
    {
        if (other.gameObject.tag == "beginner current")
        {
            //it is now no longer the first collision
            if (firstCollision)
                firstCollision = false;
            //set current waypoint to be ignored
            other.gameObject.tag = "beginner removed";

            //make sure you're not at the last waypoint
            if (cWaypoint + 1 < currentWaypoints.Length)
            {
                //start moving towards the next waypoint
                cWaypoint++;
                //set waypoint for AI to lookAt
                waypoint = currentWaypoints[cWaypoint];
                //set next waypoint to be used
                currentWaypoints[cWaypoint].gameObject.tag = "beginner current";
            }
            else
            {
                //start moving towards the next waypoint if you were at the last one
                cWaypoint = 0;
                //set waypoint for AI to lookAt
                waypoint = currentWaypoints[cWaypoint];
                //set next waypoint to be used
                currentWaypoints[cWaypoint].gameObject.tag = "beginner current";
            }
        }
        if (other.gameObject.tag == "wall")
        {
            //for 6 iterations of fixed update go backwards - bounce into wall effect
            counter = 6;
        }
    }

   
    /// <summary>
    /// Handles the Intermediate Level AI waypoints
    /// 
    /// </summary>
    /// <param name="other">
    /// the object we collided with
    /// </param>
    void IntermediateState(Collider other)
    {
        if (other.gameObject.tag == "intermediate current")
        {
            if (firstCollision)
                firstCollision = false;
            //set current waypoint to be ignored
            other.gameObject.tag = "intermediate removed";

            //make sure you're not at the last waypoint
            if (cWaypoint + 1 < currentWaypoints.Length)
            {
                //start moving towards the next waypoint
                cWaypoint++;
                //set waypoint for AI to lookAt
                waypoint = currentWaypoints[cWaypoint];
                //set next waypoint to be used
                currentWaypoints[cWaypoint].gameObject.tag = "intermediate current";
            }
            else
            {
                //start moving towards the next waypoint
                cWaypoint = 0;
                //set waypoint for AI to lookAt
                waypoint = currentWaypoints[cWaypoint];
                //set next waypoint to be used
                currentWaypoints[cWaypoint].gameObject.tag = "intermediate current";
            }
        }
        if (other.gameObject.tag == "wall")
        {
            //for 6 iterations of fixed update go backwards - bounce into wall effect
            counter = 6;
        }
    }

    /// <summary>
    /// Handles Adanved AI waypoints
    /// </summary>
    /// <param name="other">
    /// the object we collided with
    /// </param>
    void AdvancedState(Collider other)
    {
        if (other.gameObject.tag == "advanced current")
        {
            if (firstCollision)
                firstCollision = false;
            //set current waypoint to be ignored
            other.gameObject.tag = "advanced removed";

            //make sure you're not at the last waypoint
            if (cWaypoint + 1 < currentWaypoints.Length)
            {
                //start moving towards the next waypoint
                cWaypoint++;
                //set waypoint for AI to lookAt
                waypoint = currentWaypoints[cWaypoint];
                //set next waypoint to be used
                currentWaypoints[cWaypoint].gameObject.tag = "advanced current";
            }
            else
            {
                //start moving towards the next waypoint
                cWaypoint = 0;
                //set waypoint for AI to lookAt
                waypoint = currentWaypoints[cWaypoint];
                //set next waypoint to be used
                currentWaypoints[cWaypoint].gameObject.tag = "advanced current";
            }
        }
        if (other.gameObject.tag == "wall")
        {
            //for 6 iterations of fixed update go backwards - bounce into wall effect
            counter = 6;
        }
    }

    /// <summary>
    /// Rotates the bee without using the y vector
    /// </summary>
    void lookAtTarget()
    {
        Vector3 temp = new Vector3(waypoint.localPosition.x, 0, waypoint.localPosition.z);
        Vector3 temp2 = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        Quaternion rotation = Quaternion.LookRotation(temp - temp2);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime));
    }

    public override void SetLevel(Difficulity level)
    {
        cWaypoint = 0;
        firstCollision = true;
        diff = level;
        switch (diff)
        {
            case Difficulity.Tutorial:
                //do nothing
                break;
            case Difficulity.Beginner:
                currentWaypoints = waypointsBeginner;
                currentWaypoints[0].tag = "beginner current";
                break;
            case Difficulity.Intermediate:
                currentWaypoints = waypointsIntermediate;
                currentWaypoints[0].tag = "intermediate current";
                break;
            case Difficulity.Advanced:
                currentWaypoints = waypointsAdvanced;
                currentWaypoints[0].tag = "advanced current";
                break;
        }
    }
}
