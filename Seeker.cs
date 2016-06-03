using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------
    public GameObject seekerTarget;

    //Seeker's steering force (will be added to acceleration)
    private Vector3 ultimateforce;

    //WEIGHTING!!!!
    public float seekWeight = 5.0f;
	public float seperateWeight = 20;
	public float cohesionWeight = 100;
	public float alignmentWeight = 106;
	public float safeDistance = 100.0f;
	public float avoidWeight = 5.0f;
    //-----------------------------------------------------------------------
    // Start - No Update
    //-----------------------------------------------------------------------
	// Call Inherited Start and then do our own
	override public void Start () {
        //call parent's start
		base.Start();

        //initialize
		ultimateforce = Vector3.zero;
	}

    //-----------------------------------------------------------------------
    // Class Methods
    //-----------------------------------------------------------------------

    protected override void CalcSteeringForces() {
        //reset value to (0, 0, 0)
		ultimateforce = Vector3.zero;

        //got a seeking force
		ultimateforce += Seek(seekerTarget.transform.position)*seekWeight;
		ultimateforce += Seperation(5)*seperateWeight;
		ultimateforce += Cohesion (cohesionWeight);
		ultimateforce += Alignment (alignmentWeight);


		
		for (int i = 0; i < gm.Obstacles.Length; i++) {
			ultimateforce += avoidWeight*AvoidObstacle (gm.Obstacles [i], safeDistance)* safeDistance ;
		}
        //limited the seeker's steering force
		ultimateforce = Vector3.ClampMagnitude(ultimateforce, maxForce);

        //applied the steering force to this Vehicle's acceleration (ApplyForce)




		//ultimateforce.y *= Vector3.zero;
		ApplyForce(ultimateforce);
		
    }
	

}
