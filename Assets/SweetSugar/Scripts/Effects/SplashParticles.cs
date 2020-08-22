using UnityEngine;

namespace SweetSugar.Scripts.Effects
{
    /// <summary>
    /// Simple item explosion effect
    /// </summary>
    [ExecuteInEditMode]
    public class SplashParticles : MonoBehaviour
    {
        float index;
        ParticleSystem ps;
        public GameObject attached;
        public void SetColor(int index_)
        {
            ps = GetComponent<ParticleSystem>();
            index = index_+1;
            var textSheet = ps.textureSheetAnimation;
            textSheet.startFrame = index / 6f;
            ps.Play();
        }

        private void Update()
        {
            if (attached != null)
                transform.position = attached.transform.position;
        }


    }
}
