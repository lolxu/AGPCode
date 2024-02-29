using UnityEngine;

namespace __OasisBlitz.Rendering
{
    public class GameplayTexture : MonoBehaviour
    {
        public RenderTexture gameplayRenderTexture;
    
        [SerializeField] private RenderTexture surfaceRenderTexture;
        [SerializeField] private RenderTexture diveRenderTexture;
        [SerializeField] private Material blendMaterial;
        [SerializeField] private float blendDuration = 0.1f;
        
        private BlendTextures blendTexture;

        void Awake()
        {
            blendTexture = new BlendTextures(surfaceRenderTexture, diveRenderTexture, gameplayRenderTexture, blendMaterial, blendDuration);
        }
        
        public void BlendToDive()
        {
            blendTexture.StartBlend();
        }
        
        public void BlendToSurface()
        {
            blendTexture.ReverseBlend();
        }

        public void RenderToGameplayRenderTexture()
        {
            // Uncomment this to skip blending and blit directly
            // if (Diving)
            // {
            //     Graphics.Blit(diveRenderTexture, gameplayRenderTexture);
            // }
            // else
            // {
            //     Graphics.Blit(surfaceRenderTexture, gameplayRenderTexture);
            // }
            
            Graphics.Blit(null as RenderTexture, gameplayRenderTexture, blendMaterial);
        }
    }
}
