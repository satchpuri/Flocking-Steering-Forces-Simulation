
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    //-----------------------------------------------------------------------
    // Class Fields
    //-----------------------------------------------------------------------

	//vector to hold the center 
	public Vector3 centroid; 
	//to hold the average flock direction
	public Vector3 avgFlockDirec;

	public int numFlock = 10;

    public GameObject[] dude;
    public GameObject target;

    public GameObject dudePrefab;
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;

    private GameObject[] obstacles;

	public GameObject[] Obstacles {
		get { return obstacles; }
	}


    //-----------------------------------------------------------------------
    // Start and Update
    //-----------------------------------------------------------------------
	void Start () {

		obstacles = new GameObject[20]; 
		dude = new GameObject[numFlock];

        //Create the target (noodle)
        Vector3 pos = new Vector3(0, 0, 0);
        target = (GameObject)Instantiate(targetPrefab, pos, Quaternion.identity);
        //target = Instantiate(targetPrefab, pos, Quaternion.identity) as GameObject;


		//Create obstacles and place them in the obstacles array
		for(int i = 0; i < 20; i++){
			pos = new Vector3(Random.Range(-25, 25), 1.1f, Random.Range(-25,25));
			Quaternion rot = Quaternion.Euler(0,Random.Range(0, 180),0);
			obstacles[i] = (GameObject)Instantiate(obstaclePrefab, pos, rot);
		}

        //Create the GooglyEye Guy at (10, 1, 10)
		for (int i = 0; i < numFlock; i++) {

			pos = new Vector3 (Random.Range (0, 10), 1.1f, Random.Range (-5, 10));
			dude [i] = (GameObject)Instantiate (dudePrefab, pos, Quaternion.identity);
		
			//set the camera's target 
			Camera.main.GetComponent<SmoothFollow> ().target = dude[i].transform;
		}
        
	}
	

	void Update (){

		//for loop to make the array of dudes chase the target
		for (int i = 0; i < dude.Length; i++) {
			dude [i].GetComponent<Seeker> ().seekerTarget = target;
		
			//compare the distance between the guy and noodle
			//move the noodle if it's close
			float dist = Vector3.Distance (target.transform.position, dude [i].transform.position);
		
			//randomize the target's position
			//randomize the target's positionv

			if (dist < 5f) {
				do {
					target.transform.position = new Vector3 (Random.Range (-25, 25), 0, Random.Range (-25, 25));
				} while(NearAnObstacle());
			}
		}
		CalcCentroid ();
		CalcFlockDirection ();

	}

   bool NearAnObstacle() {
		//iterate through all obstacles and compare the distance between each obstacle and the noodle
		//if the noodle is within a 4 unit distance of the noodle, return true
			for(int i = 0; i < obstacles.Length; i++){
			if(Vector3.Distance(target.transform.position, obstacles[i].transform.position) < 5.0f){
				return true;
			}
		}
		//otherwise, the noodle is not near an obstacle
		return false;
	}

	void CalcCentroid(){

		//reset the position
		Vector3 position = Vector3.zero;

		//foreach loop to add the position to the obj position
		foreach (GameObject obj in dude) {
			position += obj.transform.position;
		}

		centroid = position / numFlock;


		//debug lines 
		for (int i = 0; i < dude.Length; i++) {
			Debug.DrawLine (dude[i].transform.position, centroid, Color.red);
		}
	}
	
	void CalcFlockDirection()
	{
		//reset the position
		avgFlockDirec = Vector3.zero;

		//foreach loop to add the objs forward vector 
		foreach (GameObject obj in dude) {
			avgFlockDirec += obj.transform.forward;
		}
		//normalize to get the direction
		avgFlockDirec.Normalize ();
	}
}

