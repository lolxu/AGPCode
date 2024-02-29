using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Rendering
{
    public class BlendTextures 
    {
        private readonly RenderTexture texture1;
        private readonly RenderTexture texture2;
        private readonly RenderTexture resultTexture;
        private readonly Material blendMaterial;
        private readonly float blendDuration;

        private Tweener blendTween;
        private float blendValue;

        // A constructor that requires all of the necessary parameters
        public BlendTextures(RenderTexture texture1, RenderTexture texture2, RenderTexture resultTexture, Material blendMaterial, float blendDuration = 1f)
        {
            this.texture1 = texture1;
            this.texture2 = texture2;
            this.resultTexture = resultTexture;
            this.blendMaterial = blendMaterial;
            this.blendDuration = blendDuration;
            
            SetupTween();
        }
        
        void SetupTween()
        {
            blendTween = DOTween.To(() => blendValue, x => blendValue = x, 1f, blendDuration)
                .OnUpdate(UpdateBlend)
                .SetAutoKill(false)
                .Pause();
        }
        

        void UpdateBlend()
        {
            blendMaterial.SetFloat("_DissolveThreshold", blendValue);
            Graphics.Blit(texture1, resultTexture, blendMaterial);
            Graphics.SetRenderTarget(resultTexture);
            Graphics.Blit(texture2, resultTexture, blendMaterial);
        }

        // Call this function to start blending
        public void StartBlend()
        {
            blendTween.PlayForward();
        }

        // Call this function to reverse blending
        public void ReverseBlend()
        {
            blendTween.PlayBackwards();
        }

        // Call this function to stop blending
        public void StopBlend()
        {
            blendTween.Pause();
        }

        // Call this function to restart blending from the beginning
        public void RestartBlend()
        {
            blendTween.Restart();
        }
    }
}
