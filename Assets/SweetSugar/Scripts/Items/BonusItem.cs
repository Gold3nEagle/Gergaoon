using System;
using System.Collections.Generic;
using UnityEngine;

namespace SweetSugar.Scripts.Items
{
    public class BonusItem : MonoBehaviour
    {
        public List<BonusItemArray> list = new List<BonusItemArray>();

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }

    [Serializable]
    public class BonusItemArray
    {
        public int[] array = new int[25];

    }
}