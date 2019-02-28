using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TapTapAim
{
    public class HitCircle : MonoBehaviour, IHitCircle
    {
        public ITapTapAimSetup TapTapAimSetup { get; set; }
        public int QueueID { get; set; }
        public TimeSpan PerfectHitTime { get; set; }
        public TimeSpan VisibleStartStart { get; set; }
        public TimeSpan VisibleEndStart { get; set; }
        public bool IsHitAttempted { get; private set; } = false;
        public int VisibleStartOffsetMs { get; } = 400;
        public int VisibleEndOffsetMs { get; } = 50;
        public int AccuracyLaybackMs { get; set; } = 100;
        public int Number { get; set; }

        private float alpha = 0;
        private bool fadeInTriggered;
        private bool fadeOutTriggered;
        private YieldInstruction instruction = new YieldInstruction();
        public int HitID { get; set; }

        // Use this for initialization
        void Start()
        {
            transform.GetComponent<Rigidbody2D>().simulated = false;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            transform.GetChild(1).GetComponent<Text>().text = Number.ToString();
            SetAlpha(alpha);
            VisibleStartStart = PerfectHitTime - TimeSpan.FromMilliseconds(VisibleStartOffsetMs);
            VisibleEndStart = PerfectHitTime + TimeSpan.FromMilliseconds(VisibleEndOffsetMs);
            gameObject.SetActive(false);
        }
        void Update()
        {
            if (!IsHitAttempted)
            {
                if (!fadeInTriggered && TapTapAimSetup.Tracker.Stopwatch.Elapsed >= VisibleStartStart)
                {
                    StartCoroutine(FadeIn());
                    StartCoroutine(TimingRingShrink());
                }

                if (IsInHitBound(TapTapAimSetup.Tracker.Stopwatch.Elapsed))
                {

                    transform.GetComponent<Rigidbody2D>().simulated = true;
                    transform.GetComponent<CircleCollider2D>().enabled = true;
                }

                if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= VisibleEndStart && !fadeOutTriggered)
                {
                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
                    StartCoroutine(FadeOut());
                }
            }
        }

        public bool IsInCircleLifeBound(TimeSpan time)
        {
            if (time >= VisibleStartStart
                && time <= VisibleEndStart)
            {
                return true;
            }
            return false;
        }
        public bool IsInHitBound(TimeSpan time)
        {
            if (time >= PerfectHitTime - TimeSpan.FromMilliseconds(AccuracyLaybackMs)
                && time <= PerfectHitTime + TimeSpan.FromMilliseconds(AccuracyLaybackMs))
            {
                return true;
            }
            return false;
        }
        public bool IsInAutoPlayHitBound(TimeSpan time)
        {
            if (time >= PerfectHitTime - TimeSpan.FromMilliseconds(10)
                && time <= PerfectHitTime + TimeSpan.FromMilliseconds(50))
            {
                return true;
            }
            return false;
        }

        public void TryHit()
        {
            if (!IsHitAttempted)
            {
                TimeSpan hitTime = TapTapAimSetup.Tracker.Stopwatch.Elapsed;

                Debug.Log(QueueID + "tryHit Triggered. : " + hitTime + "Perfect time =>" + PerfectHitTime + "   IsInBounds:" +
                          IsInHitBound(hitTime));

                if (HitID == TapTapAimSetup.Tracker.NextObjToHit)
                {

                    IsHitAttempted = true;
                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    transform.GetComponent<CircleCollider2D>().enabled = false;
                }

                if (IsInHitBound(hitTime))
                {
                    TapTapAimSetup.HitSource.Play();
                    Outcome(hitTime, true);
                }
                else
                {
                    Outcome(hitTime, false);
                }

            
                StartCoroutine(FadeOut());
            }
        }



        IEnumerator TimingRingShrink()
        {
            fadeInTriggered = true;
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
            fadeInTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1 != fadeOutTriggered)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(elapsedTime * 4));
            }
        }
        IEnumerator FadeOut()
        {
            fadeOutTriggered = true;
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
            //TapTapAimSetup.Tracker.NextObjToHit = HitID + 1;
            if (hit)
            {
                var difference = Math.Abs(time.TotalMilliseconds - PerfectHitTime.TotalMilliseconds);
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
                Debug.LogError(HitID + "Failed to hit");
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
