using System;
using System.Collections;
using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TapTapAim
{
    public class SliderHitCircle : MonoBehaviour, ISliderHitCircle
    {
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public int QueueID { get; set; }
        public int InteractionID { get; set; }
        public double PerfectInteractionTimeInMs { get; set; }

        public bool IsHitAttempted { get; private set; } = false;
        public int AccuracyLaybackMs { get; set; } = 100;
        public int GroupNumberShownOnCircle { get; set; }
        public event EventHandler OnInteract;

        private bool StopCalculating;

        public HitSlider ParentSlider { get; set; }
        public Visibility Visibility { get; set; }

        public double InteractionBoundStartTimeInMs { get; set; }

        public double InteractionBoundEndTimeInMs { get; set; }

        private double shrinkDuration;
        public void Disappear()
        {
            gameObject.SetActive(false);
        }

        // Use this for initialization
        void Start()
        {
            transform.GetComponent<Rigidbody2D>().simulated = false;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            transform.GetChild(1).GetComponent<Text>().text = GroupNumberShownOnCircle.ToString();

            InteractionBoundStartTimeInMs = PerfectInteractionTimeInMs - AccuracyLaybackMs;
            InteractionBoundEndTimeInMs = PerfectInteractionTimeInMs + AccuracyLaybackMs;

            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = TapTapAimSetup.visibleStartOffsetMs,
                VisibleEndOffsetMs = 50
            };
            Visibility.VisibleStartStartTimeInMs = PerfectInteractionTimeInMs - Visibility.VisibleStartOffsetMs;
            Visibility.VisibleEndStartTimeInMs = PerfectInteractionTimeInMs + Visibility.VisibleEndOffsetMs;
            OnInteract += SliderHitCircle_OnHitOrShowSliderTimingCircleEvent;
            ringStartScale = transform.GetChild(3).GetComponent<RectTransform>().localScale.x;
            shrinkDuration = PerfectInteractionTimeInMs - Visibility.VisibleStartStartTimeInMs;
            gameObject.SetActive(false);
        }

        private void SliderHitCircle_OnHitOrShowSliderTimingCircleEvent(object sender, EventArgs e)
        {
            IsHitAttempted = true;
            ((Tracker)TapTapAimSetup.Tracker).IterateInteractionQueue(InteractionID);
            Disappear();
            //throw new NotImplementedException();
        }

        void Update()
        {
            if (!IsHitAttempted)
            {
                if (TapTapAimSetup.Tracker.GetTimeInMs() >= Visibility.VisibleStartStartTimeInMs)
                {
                    StartCoroutine(TimingRingShrink());
                }

                if (IsInInteractionBound(TapTapAimSetup.Tracker.GetTimeInMs()))
                {
                    transform.GetComponent<Rigidbody2D>().simulated = true;
                    transform.GetComponent<CircleCollider2D>().enabled = true;
                }


                if (!IsHitAttempted && IsPastLifeBound())
                {
                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    transform.GetComponent<CircleCollider2D>().enabled = false;


                    Debug.LogError($" HitId:{InteractionID} Not hit attempted.  next hit id: {TapTapAimSetup.Tracker.NextObjToHit}");
                    Outcome(TapTapAimSetup.Tracker.GetTimeInMs(), false);
                    Disappear();
                }
            }
        }

        public bool IsInCircleLifeBound()
        {
            var time = TapTapAimSetup.Tracker.GetTimeInMs();
            if (time >= Visibility.VisibleStartStartTimeInMs
                && time <= PerfectInteractionTimeInMs + Visibility.VisibleEndOffsetMs)
            {
                return true;
            }
            return false;
        }

        public bool IsPastLifeBound()
        {
            return TapTapAimSetup.Tracker.GetTimeInMs() >= PerfectInteractionTimeInMs + Visibility.VisibleEndOffsetMs;
        }

        public bool IsInInteractionBound(double time)
        {
            if (time >= PerfectInteractionTimeInMs - AccuracyLaybackMs
                && time <= PerfectInteractionTimeInMs + AccuracyLaybackMs)
            {
                return true;
            }
            return false;
        }
        public bool IsInAutoPlayHitBound(double time)
        {
            if (time >= PerfectInteractionTimeInMs - 10 && time <= PerfectInteractionTimeInMs + 50)
            {
                return true;
            }
            return false;
        }

        public void TryInteract()
        {
            double hitTime = TapTapAimSetup.Tracker.GetTimeInMs();
            if (!IsHitAttempted)
            {
                //Debug.Log(QueueID + "tryHit Triggered. : " + hitTime + "Perfect time =>" + PerfectInteractionTimeInMs + "   IsInBounds:" + IsInInteractionBound(hitTime));
                if (InteractionID == TapTapAimSetup.Tracker.NextObjToHit)
                {

                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    transform.GetComponent<CircleCollider2D>().enabled = false;


                    if (IsInInteractionBound(hitTime))
                    {
                        OnInteract.Invoke(this, null);
                        TapTapAimSetup.HitSource.Play();
                        Outcome(hitTime, true);
                    }
                    else
                    {
                        Debug.LogError($" HitId:{InteractionID} Hit attempted but missed. Time difference: {hitTime - PerfectInteractionTimeInMs}ms");
                        Outcome(hitTime, false);
                    }
                }

            }
            else
            {
                Debug.LogError($" HitId:{InteractionID} Hit already attempted. Time difference: {hitTime - PerfectInteractionTimeInMs}ms");
            }
        }


        float shrinkTParam = 0;
        bool shrinkDone;
        float ringStartScale;
        float shrinkMinScale = 1.05f;
        IEnumerator TimingRingShrink()
        {

            if (shrinkDone)
                yield return null;

            shrinkTParam = Math.Abs((float)((TapTapAimSetup.Tracker.GetTimeInMs() - PerfectInteractionTimeInMs) / shrinkDuration));

            if (shrinkTParam >= 1)
            {
                shrinkTParam = 1;
                shrinkDone = true;
            }
            else if (shrinkTParam < 0)
                shrinkTParam = 0;

            var scale = shrinkMinScale + shrinkTParam * (ringStartScale - shrinkMinScale);
            SetHitRingScale(scale);
        }

        private void Outcome(double timeInMs, bool hit)
        {
            //TapTapAimSetup.Tracker.NextObjToHit = InteractionID + 1;

            if (hit)
            {
                var difference = Math.Abs(timeInMs - PerfectInteractionTimeInMs);
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


