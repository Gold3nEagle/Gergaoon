using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using System.Linq;
using UnityEngine;

namespace SweetSugar.Scripts.GUI.Boost
{
    /// <summary>
    /// Boost animation events and effects
    /// </summary>
    public class BoostAnimation : MonoBehaviour
    {
        public Square square;

        public void ShowEffect()
        {
            var partcl = Instantiate(Resources.Load("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
            var main = partcl.GetComponent<ParticleSystem>().main;
//        main.startColor = LevelManager.THIS.scoresColors[square.Item.color];
            if (name.Contains("area_explosion"))
                main.startColor = Color.white;
            Destroy(partcl, 1f);

            if (name.Contains("area_explosion"))
            {
                var p = Instantiate(Resources.Load("Prefabs/Effects/CircleExpl"), transform.position, Quaternion.identity) as GameObject;
                Destroy(p, 0.4f);

            }
        }

        public void OnFinished(BoostType boostType)
        {
            SoundBase.Instance.PlayOneShot(SoundBase.Instance.destroyPackage);

            if (boostType == BoostType.ExplodeArea)
            {
                var list = LevelManager.THIS.GetItemsAround9(square).Where(i=>i.currentType != ItemsTypes.MULTICOLOR);
                foreach (var item in list)
                {
                    if (item != null && item.Explodable) item.DestroyItem(true);
                }
            }
            if (boostType == BoostType.Bomb)
            {
                square.Item.DestroyItem(true);
            }
            LevelManager.THIS.StartCoroutine(LevelManager.THIS.FindMatchDelay());

            Destroy(gameObject);
        }
    }
}
