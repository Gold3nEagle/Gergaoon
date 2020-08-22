using SweetSugar.Scripts.Items;
using UnityEngine;

namespace SweetSugar.Scripts
{
    public class ItemAnimEvents : MonoBehaviour {


        public Item item;

        public void SetAnimationDestroyingFinished()
        {
            item.SetAnimationDestroyingFinished();
        }
    }
}
