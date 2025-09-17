using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
[DisallowMultipleComponent]
public class NavmeshAgentRandomWalk : MonoBehaviour
{

    public float walkRadius = 10f;         // Maximum distance from the agent's current position
    public float minWaitTime = 2f;         // Minimum wait before picking a new destination
    public float maxWaitTime = 5f;         // Maximum wait before picking a new destination
    public int maxTries = 30;              // Safety to avoid infinite loops when finding a valid point
    public float alignSpeed = 5f;          // Smooth rotation speed
    public float stopThreshold = 0.5f;

    [Header("Inner object")]
    public Transform innerObject;
    public Animator animator;

    private NavMeshAgent agent;
    private AnimationHandler animationHandler;
    private float waitTimer;
    private bool justStopped = false;

    [Header("Animator")]
    [SerializeField]
    private string m_VerticalID = "Vert";
    [SerializeField]
    private string m_StateID = "State";

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animationHandler = new AnimationHandler(animator, m_VerticalID, m_StateID);

        agent.updateRotation = false;
        PickNewDestination();
    }

    void FixedUpdate()
    {
        // If agent reached the destination or is idle
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                justStopped = true;
                PickNewDestination();
            }
        }

        // Calculate move axis for the animation handler
        // if the speed is below the stop threshold, make it zero
        float stopThresholdVel = agent.speed * stopThreshold;

        float moveMagnitude = agent.speed;
        if (agent.desiredVelocity.sqrMagnitude < stopThresholdVel * stopThresholdVel)
        {
            moveMagnitude = 0f;
        }
        animationHandler.Animate(moveMagnitude, 0f, Time.fixedDeltaTime);

        // Align inner object's rotation to the agent velocity
        AlignToVelocity();
    }

    void PickNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        bool found = false;
        int tries = 0;

        // Ensure the destination stays on the NavMesh
        while (!found && tries < maxTries)
        {
            Vector3 randPos = transform.position + Random.insideUnitSphere * walkRadius;
            if (NavMesh.SamplePosition(randPos, out hit, walkRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                found = true;
            }
            tries++;
        }

        // Set random wait time before the next movement
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    void AlignToVelocity()
    {
        Vector3 velocity = agent.velocity;

        // Only rotate if we're actually moving
        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            innerObject.rotation = Quaternion.Slerp(innerObject.rotation, targetRotation, Time.deltaTime * alignSpeed);
        }
    }

    #region Handlers
    private class AnimationHandler
    {
        private readonly Animator m_Animator;
        private readonly string m_VerticalID;
        private readonly string m_StateID;

        private float m_Magnitude = 0f;
        private float m_State = 0f;

        public AnimationHandler(Animator animator, string verticalID, string stateID)
        {
            m_Animator = animator;
            m_VerticalID = verticalID;
            m_StateID = stateID;
        }

        public void Animate(in float magnitude, float state, float deltaTime)
        {
            m_Magnitude = Mathf.Lerp(m_Magnitude, Mathf.Clamp01(magnitude), deltaTime * 10f);
            m_State = Mathf.Lerp(m_State, Mathf.Clamp01(state), deltaTime * 10f);

            m_Animator.SetFloat(m_VerticalID, m_Magnitude);
            m_Animator.SetFloat(m_StateID, m_State);
        }
    }
    #endregion

}
