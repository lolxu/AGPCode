using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace __OasisBlitz.Rendering
{
    /// <summary>
    /// This no longer does an actual blit, but rather calls the correct function at the correct time to render the final
    /// gameplay texture, which will then be seen by the main camera as a raw image.
    /// </summary>
    public class FinalScreenBlit : MonoBehaviour
    {
        [SerializeField] private GameplayTexture gameplayTexture;
    
        void Start()
        {
            RenderPipelineManager.endContextRendering += OnEndContextRendering;
        }

        void OnEndContextRendering(ScriptableRenderContext context, List<UnityEngine.Camera> cameras)
        {
            // Put the code that you want to execute at the end of RenderPipeline.Render here
            gameplayTexture.RenderToGameplayRenderTexture();
            // Graphics.Blit(gameplayTexture.gameplayRenderTexture, );

        }

        void OnDestroy()
        {
            RenderPipelineManager.endContextRendering -= OnEndContextRendering;
        }
    }
}