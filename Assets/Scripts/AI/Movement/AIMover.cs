using UnityEngine;

public class AIMover : MonoBehaviour
{
	public float maxSpeed;
	public float maxDegreesDelta;
	public bool lockY = true;
	public bool debug;

	private Animator animator;
	[SerializeField] private AIAgentDecisionMaker decisionMaker;
	public Vector3 Velocity { get; set; }

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (debug)
			Debug.DrawRay(transform.position, Velocity, Color.red);

		GetKinematicAvg(out Vector3 average);
		Velocity = average;
		transform.position += Velocity * maxSpeed * Time.deltaTime;
		Quaternion targetRotation = Quaternion.LookRotation(transform.forward);

		if (Velocity != Vector3.zero)
		{
			targetRotation = Quaternion.LookRotation(Velocity.normalized);
			targetRotation = new Quaternion(0, targetRotation.y, targetRotation.z, targetRotation.w);
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta * Time.deltaTime);

		animator.SetBool("running", Velocity.magnitude > 0);
	}

	private void GetKinematicAvg(out Vector3 kinematicAvg)
	{
		kinematicAvg = Vector3.zero;
		AIMovement[] movements = GetComponents<AIMovement>();
		int count = 0;
		foreach (AIMovement movement in movements)
		{
			kinematicAvg += movement.GetSteeringOutput(decisionMaker);

			++count;
		}

		if (count > 0)
		{
			kinematicAvg /= count;
		}
		else
		{
			kinematicAvg = Velocity;
		}
	}

}
