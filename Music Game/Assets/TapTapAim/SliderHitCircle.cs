using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.TapTapAim
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    namespace Assets.TapTapAim
    {
        public class SliderHitCircle : MonoBehaviour, ISliderHitCircle
        {
            public ITapTapAimSetup TapTapAimSetup { get; set; }
            public int QueueID { get; set; }
            public int HitID { get; set; }
            public TimeSpan PerfectHitTime { get; set; }
            public TimeSpan VisibleStartStart { get; set; }
            public TimeSpan VisibleEndStart { get; set; }
            public bool IsHitAttempted { get; private set; } = false;
            public int VisibleStartOffsetMs { get; } = 400;
            public int VisibleEndOffsetMs { get; } = 50;
            public int AccuracyLaybackMs { get; set; } = 100;
            public int Number { get; set; }

            private float alpha = 0;
            public bool fadeInTriggered;
            public bool fadeOutTriggered;
            private YieldInstruction instruction = new YieldInstruction();

            public void Disappear() => transform.parent.GetComponent<HitSlider>().HideCircle();
            // Use this for initialization
            void Start()
            {
                transform.GetComponent<Rigidbody2D>().simulated = false;
                transform.GetComponent<CircleCollider2D>().enabled = false;
                TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
                transform.GetChild(1).GetComponent<Text>().text = Number.ToString();

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

                        StartCoroutine(TimingRingShrink());
                    }

                    if (IsInHitBound(TapTapAimSetup.Tracker.Stopwatch.Elapsed))
                    {
                        //if (TapTapAimSetup.Tracker.NextObjToHit < HitID)
                        //    TapTapAimSetup.Tracker.NextObjToHit = HitID;
                        transform.GetComponent<Rigidbody2D>().simulated = true;
                        transform.GetComponent<CircleCollider2D>().enabled = true;

                    }

                    if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= VisibleEndStart)
                    {
                        Disappear();
                        transform.GetComponent<Rigidbody2D>().simulated = false;
                        transform.GetComponent<CircleCollider2D>().enabled = false;
                        Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
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
                if (time >= PerfectHitTime - TimeSpan.FromMilliseconds(20)
                    && time <= PerfectHitTime + TimeSpan.FromMilliseconds(20))
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

            // Update is called once per frame

        }
    }

}
