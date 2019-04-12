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
            public TapTapAimSetup TapTapAimSetup { get; set; }
            public int QueueID { get; set; }
            public int HitID { get; set; }
            public TimeSpan PerfectHitTime { get; set; }
            public bool IsHitAttempted { get; private set; } = false;
            public int VisibleStartOffsetMs { get; } = 400;
            public int VisibleEndOffsetMs { get; } = 100;
            public int AccuracyLaybackMs { get; set; } = 100;
            public int GroupNumberShownOnCircle { get; set; }
            public event EventHandler OnHitOrShowSliderTimingCircleEvent;
            private bool StopCalculating;
            public Visibility Visibility { get; set; } = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 50
            };

            private YieldInstruction instruction = new YieldInstruction();

            public void Disappear()
            {

                transform.parent.GetComponent<HitSlider>().HideCircle();
            }

            // Use this for initialization
            void Start()
            {
                transform.GetComponent<Rigidbody2D>().simulated = false;
                transform.GetComponent<CircleCollider2D>().enabled = false;
                TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
                transform.GetChild(1).GetComponent<Text>().text = GroupNumberShownOnCircle.ToString();

                Visibility.VisibleStartStart = PerfectHitTime - TimeSpan.FromMilliseconds(VisibleStartOffsetMs);
                Visibility.VisibleEndStart = PerfectHitTime + TimeSpan.FromMilliseconds(VisibleEndOffsetMs);
                OnHitOrShowSliderTimingCircleEvent += SliderHitCircle_OnHitOrShowSliderTimingCircleEvent;  
                gameObject.SetActive(false);
            }

            private void SliderHitCircle_OnHitOrShowSliderTimingCircleEvent(object sender, EventArgs e)
            {
               ((Tracker)TapTapAimSetup.Tracker).IterateHitQueue(HitID);
                //throw new NotImplementedException();
            }

            void Update()
            {
                if (!IsHitAttempted && !StopCalculating)
                {
                    if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleStartStart)
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


                    if (!IsHitAttempted && IsPastLifeBound())
                    {
                        transform.GetComponent<Rigidbody2D>().simulated = false;
                        transform.GetComponent<CircleCollider2D>().enabled = false;
                        //Disappear();

                        Debug.LogError($" HitId:{HitID} Not hit attempted.  next hit id: {TapTapAimSetup.Tracker.NextObjToHit}");
                        Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
                        StopCalculating = true;
                    }
                }
            }
            
            public bool IsInCircleLifeBound()
            {
                var time = TapTapAimSetup.Tracker.Stopwatch.Elapsed;
                if (time >= Visibility.VisibleStartStart
                    && time <= PerfectHitTime + TimeSpan.FromMilliseconds(Visibility.VisibleEndOffsetMs))
                {
                    return true;
                }
                return false;
            }

            public bool IsPastLifeBound()
            {
                return TapTapAimSetup.Tracker.Stopwatch.Elapsed >= PerfectHitTime + TimeSpan.FromMilliseconds(Visibility.VisibleEndOffsetMs);
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
                TimeSpan hitTime = TapTapAimSetup.Tracker.Stopwatch.Elapsed;
                if (!IsHitAttempted)
                {


                    Debug.Log(QueueID + "tryHit Triggered. : " + hitTime + "Perfect time =>" + PerfectHitTime + "   IsInBounds:" +
                              IsInHitBound(hitTime));
                    var nextHitID = TapTapAimSetup.Tracker.NextObjToHit;
                    //if (HitID == nextHitID)
                    //{
                        OnHitOrShowSliderTimingCircleEvent(this, null);
                        IsHitAttempted = true;
                        transform.GetComponent<Rigidbody2D>().simulated = false;
                        transform.GetComponent<CircleCollider2D>().enabled = false;


                        if (IsInHitBound(hitTime))
                        {
                            TapTapAimSetup.HitSource.Play();
                            Outcome(hitTime, true);
                        }
                        else
                        {
                            Debug.LogError($" HitId:{HitID} Hit attempted but missed. Time difference: {hitTime - PerfectHitTime}ms");
                            Outcome(hitTime, false);
                        }
                    //}
                    //else
                    //{
                    //    Debug.LogError($" HitId:{HitID} Hit attempted but it is not next hit id: {nextHitID}. Time difference: {hitTime - PerfectHitTime}ms");
                    //}
                }
                else
                {
                    Debug.LogError($" HitId:{HitID} Hit already attempted. Time difference: {hitTime - PerfectHitTime}ms");
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
