using System.Linq;
// using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.Utility
{
    public class SetupBackfaces : MonoBehaviour
    {
    
        // TODO: This logic could be baked so that meshes don't need to be calculated at runtime
        [SerializeField] private Material backfaceMaterial;
        [SerializeField] private Material stencilBackfaceMaterial;
        [SerializeField] private Material stencilFrontfaceMaterial;

        public void CreateBackfaceMeshes()
        {
            // Find all gameobjects on layer "PenetrableGround" or "PenetrableObject"
            // (This is insanely inefficient, but it's only done once at the start of the game and we will remove it later)
        
            // Get a list of all gameobjects
            GameObject[] allGameObjects = FindObjectsOfType<GameObject>();

            // Use a LINQ query to filter down to only the gameobjects on the "LargePenetrable" layer
            GameObject[] largePenetrables = allGameObjects.Where(
                go => go.layer == Constants.LargePenetrableLayer).ToArray();

            // For each penetrable, create a gameobject with the same transform and mesh, but with the normals flipped
            foreach (GameObject penetrable in largePenetrables)
            {
                MeshFilter meshFilter = penetrable.GetComponent<MeshFilter>();
                
                // Needed for checking if this is set to render (meshes that get unioned will not be set to render)
                MeshRenderer meshRenderer = penetrable.GetComponent<MeshRenderer>();

                if (meshFilter == null) continue;
                if (!meshRenderer.enabled) continue;

                Mesh mesh = meshFilter.mesh;

                Mesh invertedMesh = MeshInverter.InvertMesh(mesh);

                // Create a visible backface, which will be used in dive view
                GameObject visibleBackface = new GameObject();
                visibleBackface.name = "VisibleBackface";
                visibleBackface.transform.parent = penetrable.transform;
                visibleBackface.transform.localPosition = Vector3.zero;
                visibleBackface.transform.localRotation = Quaternion.identity;
                visibleBackface.transform.localScale = Vector3.one;
                visibleBackface.AddComponent<MeshFilter>().mesh = invertedMesh;
                // backface.AddComponent<MeshRenderer>().material = penetrable.GetComponent<MeshRenderer>().material;
                
                // TODO: This is a legacy solution so we can have backfaces on sand without a custom texture.
                Material materialForThisObject = penetrable.GetComponent<BackfaceMaterial>()?.backfaceMaterial;
                if (materialForThisObject == null)
                {
                    materialForThisObject = backfaceMaterial;
                }
                
                visibleBackface.AddComponent<MeshRenderer>().material = materialForThisObject;
                
                visibleBackface.layer = LayerMask.NameToLayer("SandBackface");

                // Create an invisible backface that will be used by the stencil renderer to determine when to draw
                // dive view
                GameObject stencilBackface = new GameObject();
                stencilBackface.name = "StencilBackface";
                stencilBackface.transform.parent = penetrable.transform;
                stencilBackface.transform.localPosition = Vector3.zero;
                stencilBackface.transform.localRotation = Quaternion.identity;
                stencilBackface.transform.localScale = Vector3.one;
                stencilBackface.AddComponent<MeshFilter>().mesh = invertedMesh;
                // backface.AddComponent<MeshRenderer>().material = penetrable.GetComponent<MeshRenderer>().material;
                stencilBackface.AddComponent<MeshRenderer>().material = stencilBackfaceMaterial;
                stencilBackface.layer = LayerMask.NameToLayer("SandBackfaceStencil");

                // Create an invisible frontface that will be used by the stencil renderer for the same purpose
                GameObject stencilFrontface = new GameObject();
                stencilFrontface.name = "StencilFrontface";
                stencilFrontface.transform.parent = penetrable.transform;
                stencilFrontface.transform.localPosition = Vector3.zero;
                stencilFrontface.transform.localRotation = Quaternion.identity;
                stencilFrontface.transform.localScale = Vector3.one;
                stencilFrontface.AddComponent<MeshFilter>().mesh = mesh;
                // backface.AddComponent<MeshRenderer>().material = penetrable.GetComponent<MeshRenderer>().material;
                stencilFrontface.AddComponent<MeshRenderer>().material = stencilFrontfaceMaterial;
                stencilFrontface.layer = LayerMask.NameToLayer("SandFrontfaceStencil");

            }
        }
    }
}
