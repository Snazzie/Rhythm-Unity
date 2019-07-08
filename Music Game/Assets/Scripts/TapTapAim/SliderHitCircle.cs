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
        public TimeSpan PerfectInteractionTime { get; set; }

        public bool IsHitAttempted { get; private set; } = false;
        public int AccuracyLaybackMs { get; set; } = 100;
        public int GroupNumberShownOnCircle { get; set; }
        public event EventHandler OnInteract;

        private bool StopCalculating;

        public HitSlider ParentSlider { get; set; }
        public Visibility Visibility { get; set; }

        public TimeSpan InteractionBoundStart { get; set; }

        public TimeSpan InteractionBoundEnd { get; set; }

        private YieldInstruction instruction = new YieldInstruction();

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

            InteractionBoundStart = PerfectInteractionTime - TimeSpan.FromMilliseconds(AccuracyLaybackMs);
            InteractionBoundEnd = PerfectInteractionTime + TimeSpan.FromMilliseconds(AccuracyLaybackMs);

            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 50
            };
            Visibility.VisibleStartStart = PerfectInteractionTime - TimeSpan.FromMilliseconds(Visibility.VisibleStartOffsetMs);
            Visibility.VisibleEndStart = PerfectInteractionTime + TimeSpan.FromMilliseconds(Visibility.VisibleEndOffsetMs);
            OnInteract += SliderHitCircle_OnHitOrShowSliderTimingCircleEvent;
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
                if (ParentSlider.fadeInTriggered && TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleStartStart)
                {

                    StartCoroutine(TimingRingShrink());
                }

                if (IsInInteractionBound(TapTapAimSetup.Tracker.Stopwatch.Elapsed))
                {
                    transform.GetComponent<Rigidbody2D>().simulated = true;
                    transform.GetComponent<CircleCollider2D>().enabled = true;
                }


                if (!IsHitAttempted && IsPastLifeBound())
                {
                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    transform.GetComponent<CircleCollider2D>().enabled = false;


                    Debug.LogError($" HitId:{InteractionID} Not hit attempted.  next hit id: {TapTapAimSetup.Tracker.NextObjToHit}");
                    Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
                    Disappear();
                }
            }
        }

        public bool IsInCircleLifeBound()
        {
            var time = TapTapAimSetup.Tracker.Stopwatch.Elapsed;
            if (time >= Visibility.VisibleStartStart
                && time <= PerfectInteractionTime + TimeSpan.FromMilliseconds(Visibility.VisibleEndOffsetMs))
            {
                return true;
            }
            return false;
        }

        public bool IsPastLifeBound()
        {
            return TapTapAimSetup.Tracker.Stopwatch.Elapsed >= PerfectInteractionTime + TimeSpan.FromMilliseconds(Visibility.VisibleEndOffsetMs);
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
            if (time >= PerfectInteractionTime - TimeSpan.FromMilliseconds(20) && time <= PerfectInteractionTime + TimeSpan.FromMilliseconds(AccuracyLaybackMs))
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

        private void Outcome(TimeSpan time, bool hit)
        {
            //TapTapAimSetup.Tracker.NextObjToHit = InteractionID + 1;

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


