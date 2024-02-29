using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class PathPredictor : MonoBehaviour
{
    private PlayerStateMachine ctx;
    private LineRenderer _line;

    public LayerMask largePenetrableLayerMask;
    public GameObject exitPoint;

    public Material SilhouetteMaterial;
    public Gradient silhouetteGradient;
    
    private Renderer exitPointRenderer;

    private float timeStep = 1.0f / 15.0f;   // should vary this based on distance... from last frame...
    private int numSteps = 25;
    private void Awake()
    {
        ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        _line = GetComponent<LineRenderer>();

        exitPointRenderer = exitPoint.GetComponent<Renderer>();
    }

    private void Update()
    {
        if (ctx.IsSubmerged)
        {
            DrawPredictedPath();
        }
    }
    
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

    private void DrawPredictedPath()
    {
        // gather initial values before doing a bunch of expensive stuff
        Vector3 position = ctx.transform.position;
        Vector3 acceleration = new Vector3(0, 100.0f, 0);
        acceleration += ctx.MovementInput * 100.0f;
        
        Vector3 velocity = ctx.PlayerPhysics.Velocity;

        acceleration /= 1.3f;   // account for drag lol!
        
        // simulate
        List<Vector3> simulationPoints = new List<Vector3>();
        
        simulationPoints.Add(position);

        for (int i = 0; i < numSteps; i++)
        {
            // iterate by time
            position += velocity * timeStep;
            simulationPoints.Add(position);

            velocity += acceleration * timeStep;
        }

        // now raycast backwards until we hit a penetrable
        int startIndex = simulationPoints.Count - 1;
        int endIndex = simulationPoints.Count - 2;
        bool bFoundHit = false;
        RaycastHit[] m_Results = new RaycastHit[1];
        Vector3 ExitLocation = Vector3.zero;
        while (!bFoundHit && endIndex >= 0)
        {
            Vector3 start = simulationPoints[startIndex];
            Vector3 end = simulationPoints[endIndex];
            Vector3 diff = end - start;
            int hits = Physics.RaycastNonAlloc(start,
                diff, m_Results, diff.magnitude, largePenetrableLayerMask);
            if (hits == 1)
            {
                bFoundHit = true;
                ExitLocation = m_Results[0].point;
            }

            startIndex--;
            endIndex--;
        }
        
        // get depth to ground
        float yVelocity = Mathf.Abs(ctx.PlayerPhysics.Velocity.y);

        float t = map(yVelocity, 0.0f, 45.0f, 0.0f, 1.0f);
        Color c = silhouetteGradient.Evaluate(t);
        SilhouetteMaterial.SetColor("_Color", c);
        
        // set visual to be at exit point for now!
        if (bFoundHit)
        {
            exitPoint.transform.position = ExitLocation;
            float dist = (ExitLocation - ctx.transform.position).magnitude;
            dist = Mathf.Clamp(dist, 0.0f, 15.0f);
            float radius = map(dist, 0.0f, 15.0f, 0.0f, .4f);
            
            exitPointRenderer.sharedMaterial.SetFloat("_radius", radius);
            
            // cut off
            // draw simulation line (will be raycasting eventually)
            //_line.positionCount = simulationPoints.Count;
            _line.positionCount = startIndex;
            _line.SetPositions(simulationPoints.ToArray());
        }
        else
        {
            _line.positionCount = simulationPoints.Count;
            _line.SetPositions(simulationPoints.ToArray());
            // exitPoint.SetActive(false);
        }
    }
}
