using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------

    //movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;

	protected GameManager gm;

    public Vector3 Velocity {
        get { return velocity; }
    }

    //public for changing in Inspector
    //define movement behaviors
    public float maxSpeed = 6.0f;
    public float maxForce = 12.0f;
    public float mass = 1.0f;
    public float radius = 1.0f;

    //access to Character Controller component
    CharacterController charControl;
    

    abstract protected void CalcSteeringForces();


    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	virtual public void Start(){
        //acceleration = new Vector3 (0, 0, 0);     
        acceleration = Vector3.zero;
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>();
		gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>(); 
	}

	
	// Update is called once per frame
	protected void Update () {

		CalcSteeringForces();

		velocity += acceleration * Time.deltaTime;
		velocity.y = 0;

		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

		transform.forward = velocity.normalized;

		charControl.Move (velocity * Time.deltaTime);

		acceleration = Vector3.zero;

	}


    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

	public Vector3 Seperation(float dist)
	{

		Vector3 sum_vector = new Vector3();
		int count = 0;
		
		// For each obj, check the distance from this obj, and if withing a neighbourhood, add to the sum_vector
		for (int i= 0; i< gm.dude.Length; i++)
		{
			float d = Vector3.Distance(transform.position, gm.dude[i].transform.position);
			
			if (d  < dist && d > 0) // d > 0 prevents including obj
			{
				sum_vector += (transform.position - gm.dude[i].transform.position).normalized / d;
				count++;
			}
		}		
		// Average the sum_vector
		if (count > 0)
		{
			sum_vector /= count;
		}
		return  sum_vector;
	}
	
	public Vector3 Alignment(float alignment)
	{
		Vector3 sum_vector = new Vector3();
		int count = 0;
		
		// For each obj, check the distance from this obj, and if withing a neighbourhood, add to the sum_vector
		for (int i=0; i< gm.dude.Length; i++)
		{
			float dist = Vector2.Distance(transform.position, gm.dude[i].transform.position);
			
			if (dist < alignment && dist > 0) // dist > 0 prevents including this boid
			{
				sum_vector += velocity -gm.dude[i].transform.position;
				count++;
			}
		}
		
		// Average the sum_vector and clamp magnitude
		if (count > 0)
		{
			sum_vector /= count;
			sum_vector = Vector2.ClampMagnitude(sum_vector, 1);
		}
		
		return sum_vector;
	}


	public Vector3 Cohesion(float centroid)
	{

		Vector3 sum_vector = new Vector3();
		int count = 0;
		
		// For each obj, check the distance from this obj, and if withing a neighbourhood, add to the sum_vector
		for (int i=0; i< gm.dude.Length; i++)
		{
			float dist = Vector3.Distance(transform.position, gm.dude[i].transform.position);
			
			if (dist < centroid && dist > 0) // dist > 0 prevents including this boid
			{
				sum_vector += gm.dude[i].transform.position;
				count++;
			}
		}
		
		// Average the sum_vector and return value
		if (count > 0)
		{
			sum_vector /= count;
			return  sum_vector - transform.position;
		}
		
		return sum_vector; 

	}

	protected Vector3 AvoidObstacle(GameObject ob, float safe) {
		
		//reset desired velocity
		desired = Vector3.zero;
		//get radius from obstacle's script
		float obRad = ob.GetComponent<ObstacleScript>().Radius;
		//get vector from vehicle to obstacle
		Vector3 vecToCenter = ob.transform.position - transform.position;
		//zero-out y component (only necessary when working on X-Z plane)
		vecToCenter.y = 0;
		//if object is out of my safe zone, ignore it
		if(vecToCenter.magnitude > safe){
			return Vector3.zero;
		}
		//if object is behind me, ignore it
		if(Vector3.Dot(vecToCenter, transform.forward) < 0){
			return Vector3.zero;
		}
		//if object is not in my forward path, ignore it
		if(Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius){
			return Vector3.zero;
		}
		
		//if we get this far, we will collide with an obstacle!
		//object on left, steer right
		if (Vector3.Dot(vecToCenter, transform.right) < 0) {
			desired = transform.right * maxSpeed;
			//debug line to see if the dude is avoiding to the right
			Debug.DrawLine(transform.position, ob.transform.position, Color.red);
		}
		else {
			desired = transform.right * -maxSpeed;
			//debug line to see if the dude is avoiding to the left
			Debug.DrawLine(transform.position, ob.transform.position, Color.green);
		}
		return desired;
	}

    protected void ApplyForce(Vector3 steeringForce) {
        acceleration += steeringForce / mass;
    }

    protected Vector3 Seek(Vector3 targetPos) {
        desired = targetPos - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;
        desired.y = 0;
        return desired;
    }

}
