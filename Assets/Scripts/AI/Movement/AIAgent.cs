using UnityEngine;

public class AIAgent : MonoBehaviour
{
	public float maxSpeed;
	public float maxDegreesDelta;
	public bool lockY = true;
	public bool debug;

	private Animator animator;

	public Vector3 Velocity { get; set; }

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (debug)
			Debug.DrawRay(transform.position, Velocity, Color.red);

		GetKinematicAvg(out Vector3 average, out Quaternion rotation);
		Velocity = average;
		transform.position += Velocity * maxSpeed * Time.deltaTime;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, maxDegreesDelta * Time.deltaTime);

		animator.SetBool("running", Velocity.magnitude > 0);
	}

	private void GetKinematicAvg(out Vector3 kinematicAvg, out Quaternion rotation)
	{
		kinematicAvg = Vector3.zero;
		Vector3 eulerAvg = Vector3.zero;
		AIMovement[] movements = GetComponents<AIMovement>();
		int count = 0;
		foreach (AIMovement movement in movements)
		{
			kinematicAvg += movement.GetSteeringOutput(this).linear;
			eulerAvg += movement.GetSteeringOutput(this).angular.eulerAngles;

			++count;
		}

		if (count > 0)
		{
			kinematicAvg /= count;
			eulerAvg /= count;
			rotation = Quaternion.Euler(eulerAvg);
		}
		else
		{
			kinematicAvg = Velocity;
			rotation = transform.rotation;
		}
	}

}
