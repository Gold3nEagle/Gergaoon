using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SweetSugar.Scripts.Items
{
    /// <summary>
    /// Time bomb item
    /// </summary>
    public class itemTimeBomb :Item, IItemInterface
    {
        [SerializeField] private bool ActivateByExplosion;
        [SerializeField] private bool StaticOnStart;
        public int timer;
        public int startTimer = 5;
        public TextMeshProUGUI timerLabel;
        [SerializeField] public GameObject sprite;
        public GameObject explosion;

        public void Destroy(Item item1, Item item2)
        {
                GetItem.square.DestroyBlock();
            if (GetItem.square.type == SquareTypes.WireBlock)
            {
                GetItem.StopDestroy();
            }
            SoundBase.Instance
                .PlayOneShot(SoundBase.Instance.explosion);
            Instantiate(explosion, transform.position, Quaternion.identity);
            DestroyBehaviour();
        }
        
        private Item GetItem => GetComponentInParent<Item>();

        public void OnEnable()
        {
            startTimer = LevelManager.THIS.field.fieldData.bombTimer;
            timer = startTimer;
            LevelManager.OnTurnEnd += OnTurnEnd;
            InitItem();
        }

        public override void InitItem()
        {
            timerLabel.color = new Color(73f/255f,73f/255f,73f/255f);
            sprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
            GetComponentInChildren<Canvas>().enabled = true;
            sprite.SetActive(true);
            base.InitItem();
            UpdateTimer();
        }

        private void OnDisable()
        {
            LevelManager.OnTurnEnd -= OnTurnEnd;
        }

        private void OnTurnEnd()
        {
            timer--;
            // if(timer == 1) Warning();
            if (timer <= 0)
            {
                timer = 0;
                // LevelManager.THIS.gameStatus = GameState.BombFailed;
           
            }
            UpdateTimer();
        }

        private void Warning()
        {
            timerLabel.color = Color.red;
            var seq = LeanTween.sequence();
            float t = 0.3f;
            seq.append(LeanTween.scale(timerLabel.gameObject, Vector3.one * 1.2f, t));
            seq.append(LeanTween.scale(timerLabel.gameObject, Vector3.one, t));
            seq.append(LeanTween.scale(timerLabel.gameObject, Vector3.one * 1.2f, t));
            seq.append(LeanTween.scale(timerLabel.gameObject, Vector3.one, t));
            SoundBase.Instance.PlayLimitSound(SoundBase.Instance.timeOut);
        }

        public UnityAction OnExlodeAnimationFinished;
        public void ExlodeAnimation(bool hide, UnityAction callback)
        {
            LevelManager.THIS.levelData.limit = 0;

            if(callback == null)
                callback = OnExlodeAnimationFinished;
            anim.enabled = false;
            GetComponentInChildren<Canvas>().enabled = false;
            GetComponent<BombFailedAnimation>().BombFailed(Vector3.zero, 10, hide, callback);
        }

        void UpdateTimer()
        {
            timerLabel.text = timer.ToString();
        }

        public GameObject GetGameobject()
        {
            return gameObject;
        }

        public bool IsCombinable()
        {
            return Combinable;
        }

        public bool IsExplodable()
        {
            return ActivateByExplosion;
        }

        public void SetExplodable(bool setExplodable)
        {
            ActivateByExplosion = setExplodable;
        }

        public bool IsStaticOnStart()
        {
            return StaticOnStart;
        }

        public void SetOrder(int i)
        {
            GetComponent<SpriteRenderer>().sortingOrder = i;
        }

        public Item GetParentItem()
        {
            return transform.GetComponentInParent<Item>();
        }
        
        
    }
}