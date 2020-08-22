using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SweetSugar.Scripts.Effects
{
    /// <summary>
    /// Explode animation for package item
    /// </summary>
    public class ExplAround : MonoBehaviour
    {
        public Item item;
        public FieldBoard field;
        public Item[] items;
        public Item[] itemsSecondCirle;
        public Item[] itemsThirdCircle;
        public float duration = 1.2f;
        public AnimationCurve curve;
        public GameObject particle;
        public float startParticle = 0.25f;
        public AudioSource AudioSource;
        private void Play(Item[] array, float delay, float force)
        {
            foreach (Item item1 in array)
            {
                if(item1 == null) continue;
                item1.anim.enabled = false;
                var seq = LeanTween.sequence();
                Transform child = item1.transform.GetChild(0);
                Vector3 transformPosition = child.transform.position;
                Vector2 v = transformPosition + (transformPosition - item.transform.position).normalized * force;
                LeanTween.move(child.gameObject, v, duration / 2).setDelay(delay).setEase(curve).setOnComplete(OnFinished,child.gameObject);
            }

            LeanTween.delayedCall(5, Destr);
        }

        private void Destr()
        {
            if(this != null)
                Destroy(gameObject);
        }


        void OnFinished(object o)
        {
            Item _item = ((GameObject) o).transform.parent.GetComponent<Item>();
            _item.anim.enabled = true;//TODO: set destroying items animation false and true after
            _item.ResetAnimTransform();
//            item.SetActive(false);
        }


        private void Start()
        {

            FillArrays();               
            Play(itemsSecondCirle,0.51f, 0.5f);
            Play(itemsThirdCircle,0.63f, 0.3f);
            LeanTween.delayedCall(startParticle, () => { particle.SetActive(true);});
            AudioSource.PlayDelayed(0.2f);
        }

        private void FillArrays()
        {
            if (item == null) return;
            items = GetItemsAround8(item.square).ToArray();
            itemsSecondCirle = GetItemsAroundSecond(item.square).ToArray();
            itemsThirdCircle = GetItemsAroundThird(item.square).ToArray();
        }

        private List<Item> GetItemsAround8(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<Item>
            {
                GetSquare(col + 0, row - 1, true)?.Item,
                GetSquare(col + 1, row - 1, true)?.Item,
                GetSquare(col + 1, row + 0, true)?.Item,
                GetSquare(col + 1, row + 1, true)?.Item,
                GetSquare(col + 0, row + 1, true)?.Item,
                GetSquare(col - 1, row + 1, true)?.Item,
                GetSquare(col - 1, row + 0, true)?.Item,
                GetSquare(col - 1, row - 1, true)?.Item
            };


            return itemsList;
        }

        private List<Item> GetItemsAroundSecond(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<Item>();

            var r = row - 2;
            var c = col - 2;
            for (c = col - 2; c <= col + 2; c++)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            c = col + 2;
            for (r = row - 1; r <= row + 2; r++)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            r = row + 2;
            for (c = col + 1; c >= col - 2; c--)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            c = col - 2;
            for (r = row + 1; r >= row - 1; r--)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            return itemsList;
        }

        private List<Item> GetItemsAroundThird(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<Item>();

            var r = row - 3;
            var c = col - 3;
            for (c = col - 3; c <= col + 3; c++)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            c = col + 3;
            for (r = row - 2; r <= row + 3; r++)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            r = row + 3;
            for (c = col + 2; c >= col - 3; c--)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            c = col - 3;
            for (r = row + 2; r >= row - 2; r--)
            {
                itemsList.Add(GetSquare(c, r, true)?.Item);
            }

            return itemsList;
        }

        private Square GetSquare(int col, int row, bool safe = false)
        {
            if(!field)
            field = LevelManager.THIS.field;
            return field.GetSquare(col, row);
        }

    }
}