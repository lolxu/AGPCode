using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class RopeMaker : MonoBehaviour
{
    public GameObject startObject; // Assign the start object in the inspector
    public GameObject endObject;   // Assign the end object in the inspector
    
    public float resolution = 0.1f;

    public GameObject ropePrefab;

    private ObiRope rope;

    void Start()
    {
        if (startObject == null || endObject == null || ropePrefab == null)
        {
            Debug.LogError("Please assign all the required objects and the rope prefab.");
            return;
        }

        // CreateAndAttachRope();
        StartCoroutine(CreateRopeBlueprint(startObject.transform.position, endObject.transform.position));
    }

    // private void CreateAndAttachRope()
    // {
    //     // Instantiate the rope prefab
    //     // createdRope = Instantiate(ropePrefab, Vector3.zero, Quaternion.identity);
    //
    //     // Attach the start and end of the rope to the specified GameObjects
    //     AttachRopeEndToGameObject(0, startObject); // Attach first particle
    //     AttachRopeEndToGameObject(rope.activeParticleCount - 1, endObject); // Attach last particle
    // }

    private void AttachRopeEndToGameObject(GameObject obj)
    {
        // int solverIndex = rope.solverIndices[particleIndex];
    
        // rope.solver.positions[solverIndex] = obj.transform.position;
        
        
        
        ObiParticleGroup group;
        ObiParticleAttachment attachment = rope.gameObject.AddComponent<ObiParticleAttachment>();
        attachment.target = obj.transform;
        // attachment.particleGroup = rope.
        // attachment.particleGroup = createdRope.Parti;
    }

    private IEnumerator CreateRopeBlueprint(Vector3 start, Vector3 end)
    {
        // create the blueprint: (ltObiRopeBlueprint, ObiRodBlueprint)
        var blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();

        // Procedurally generate the rope path (a simple straight line):
        int filter = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 0);
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(start, -Vector3.right, Vector3.right, Vector3.up, 0.1f, 0.1f, 1, filter,
            Color.white, "start");
        blueprint.path.AddControlPoint(end, -Vector3.right, Vector3.right, Vector3.up, 0.1f, 0.1f, 1, filter,
            Color.white, "end");
        blueprint.resolution = resolution;
        blueprint.path.FlushEvents();

        // generate the particle representation of the rope (wait until it has finished):
        yield return StartCoroutine(blueprint.Generate());
        
        rope = Instantiate(ropePrefab, Vector3.zero, Quaternion.identity).GetComponent<ObiRope>();
        
        
        // The solver is on this gameobject, parent the rope to it and then add the solver to the updater
        rope.transform.SetParent(transform);
        RopeManager.Instance.AddSolver(GetComponent<ObiSolver>());
        
        rope.gameObject.name = "PROCEDURAL ROPE";
        rope.GetComponent<ObiRope>().ropeBlueprint = blueprint;
        
        // Create start attachment
        ObiParticleAttachment startAttachment = rope.gameObject.AddComponent<ObiParticleAttachment>();
        startAttachment.particleGroup = blueprint.groups[0];
        startAttachment.target = startObject.transform;
        
        // Create end attachment
        ObiParticleAttachment endAttachment = rope.gameObject.AddComponent<ObiParticleAttachment>();
        endAttachment.particleGroup = blueprint.groups[1];
        endAttachment.target = endObject.transform;
    }
}
