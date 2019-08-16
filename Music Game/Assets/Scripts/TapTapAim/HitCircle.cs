using System;
using System.Collections;
using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TapTapAim
{
    public class HitCircle : MonoBehaviour, IHitCircle, IQueuable
    {
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public int InteractionID { get; set; }
        public int QueueID { get; set; }
        public TimeSpan PerfectInteractionTime { get; set; }

        public bool IsHitAttempted { get; private set; } = false;

        public int AccuracyLaybackMs { get; set; } = 100;
        public int GroupNumberShownOnCircle { get; set; }

        private float alpha { get; set; } = 0;

        public Visibility Visibility { get; set; }
        private YieldInstruction instruction = new YieldInstruction();
        public TimeSpan InteractionBoundStart { get; set; }

        public TimeSpan InteractionBoundEnd { get; set; }

        public event EventHandler OnInteract;
        // Use this for initialization
        void Start()
        {
            transform.GetComponent<Rigidbody2D>().simulated = false;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            transform.GetChild(1).GetComponent<Text>().text = GroupNumberShownOnCircle.ToString();
            SetAlpha(alpha);

            InteractionBoundStart = PerfectInteractionTime - TimeSpan.FromMilliseconds(AccuracyLaybackMs);
            InteractionBoundEnd = PerfectInteractionTime + TimeSpan.FromMilliseconds(AccuracyLaybackMs);
            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 50
            };
            Visibility.VisibleStartStart = PerfectInteractionTime - TimeSpan.FromMilliseconds(Visibility.VisibleStartOffsetMs);
            Visibility.VisibleEndStart = PerfectInteractionTime + TimeSpan.FromMilliseconds(Visibility.VisibleEndOffsetMs);
            OnInteract += HitCircle_OnHitEvent;
            gameObject.SetActive(false);
        }
        void Update()
        {
            if (!IsHitAttempted)
            {
                if (!Visibility.fadeInTriggered && TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleStartStart)
                {
                    StartCoroutine(FadeIn());
                    StartCoroutine(TimingRingShrink());
                }

                if (IsInInteractionBound(TapTapAimSetup.Tracker.Stopwatch.Elapsed))
                {

                    transform.GetComponent<Rigidbody2D>().simulated = true;
                    transform.GetComponent<CircleCollider2D>().enabled = true;
                }

                if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleEndStart && !Visibility.fadeOutTriggered)
                {
                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
                    StartCoroutine(FadeOut());
                }
            }
        }
        private void HitCircle_OnHitEvent(object sender, EventArgs e)
        {
            IsHitAttempted = true;
            ((Tracker)TapTapAimSetup.Tracker).IterateInteractionQueue(InteractionID);

            //throw new NotImplementedException();
        }
        public bool IsInCircleLifeBound(TimeSpan time)
        {
            if (time >= Visibility.VisibleStartStart
                && time <= Visibility.VisibleEndStart)
            {
                return true;
            }
            return false;
        }
        public bool IsInInteractionBound(TimeSpan time)
        {
            if (time >= PerfectInteractionTime - TimeSpan.FromMilliseconds(AccuracyLaybackMs)
                && time <= PerfectInteractionTime + TimeSpan.FromMilliseconds(AccuracyLaybackMs))
            {
                return true;
            }
            return false;
        }
        public bool IsInAutoPlayHitBound(TimeSpan time)
        {
            if (time >= PerfectInteractionTime - TimeSpan.FromMilliseconds(10)
                && time <= PerfectInteractionTime + TimeSpan.FromMilliseconds(50))
            {
                return true;
            }
            return false;
        }

        public void TryInteract()
        {
            TimeSpan hitTime = TapTapAimSetup.Tracker.Stopwatch.Elapsed;
            if (!IsHitAttempted)
            {
                Debug.Log(QueueID + "tryHit Triggered. : " + hitTime + "Perfect time =>" + PerfectInteractionTime + "   IsInBounds:" +
                          IsInInteractionBound(hitTime));

                if (InteractionID == TapTapAimSetup.Tracker.NextObjToHit)
                {
                    if (IsInInteractionBound(hitTime))
                    {
                        transform.GetComponent<Rigidbody2D>().simulated = false;
                        transform.GetComponent<CircleCollider2D>().enabled = false;

                        OnInteract.Invoke(this, null);
                        TapTapAimSetup.HitSource.Play();
                        Outcome(hitTime, true);
                        StartCoroutine(FadeOut());
                    }
                    else
                    {
                        Debug.LogError($" HitId:{InteractionID} Hit attempted but missed. Time difference: {hitTime - PerfectInteractionTime}ms");
                        Outcome(hitTime, false);
                    }
                }
            }
            else
            {
                Debug.LogError($" HitId:{InteractionID} Hit already attempted. Time difference: {hitTime - PerfectInteractionTime}ms");
            }
        }



        IEnumerator TimingRingShrink()
        {
            Visibility.fadeInTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                var scale = 2f - Mathf.Clamp01(elapsedTime * 2.4f);
                if (scale >= 1.1f)
                {
                    SetHitRingScale(scale);
                }
                else
                {
                    SetHitRingScale(1.1f);
                }
            }
        }
        IEnumerator FadeIn()
        {
            Visibility.fadeInTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1 != Visibility.fadeOutTriggered)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(elapsedTime * 4));
            }
        }
        IEnumerator FadeOut()
        {
            Visibility.fadeOutTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(1.0f - Mathf.Clamp01(elapsedTime * 9));
            }

        }

        private void Outcome(TimeSpan time, bool hit)
        {
            if (hit)
            {
                var difference = Math.Abs(time.TotalMilliseconds - PerfectInteractionTime.TotalMilliseconds);
                int score;
                if (difference <= 100)
                {
                    score = 100;
                }
                else if (difference <= 150)
                {
                    score = 50;
                }
                else
                {
                    score = 20;
                }

                var cs = new HitScore()
                {
                    id = QueueID,
                    accuracy = GetAccuracy(difference),
                    score = score
                };
                TapTapAimSetup.Tracker.RecordEvent(true, cs);
            }
            else
            {
                var cs = new HitScore()
                {
                    id = QueueID,
                    accuracy = 0,
                    score = 0
                };
                TapTapAimSetup.Tracker.RecordEvent(false, cs);
                Debug.LogError(InteractionID + "Failed to hit");
            }
            // gameObject.SetActive(false);
            Destroy(gameObject, 2);
        }
        //TODO: scale with HasAttemptHit window
        private float GetAccuracy(double difference)
        {
            if (difference <= 200)
                return 100;

            return 100 - ((float)difference) / 10;
        }
        private void SetHitRingScale(float scale)
        {
            var child = transform.GetChild(3).GetComponent<RectTransform>();
            child.localScale = new Vector3(scale, scale, 0);
        }
        private void SetAlpha(float alpha)
        {
            var children = GetComponentsInChildren<Image>();
            var textChild = GetComponentsInChildren<Text>();
            foreach (var child in children)
            {
                var newColor = child.color;
                newColor.a = alpha;
                child.color = newColor;
            }

            foreach (var child in textChild)
            {
                var newColor = child.color;
                newColor.a = alpha;
                child.color = newColor;
            }
        }

        // Update is called once per frame

    }
}
