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
        public double PerfectInteractionTimeInMs { get; set; }
        public double InteractionBoundStartTimeInMs { get; set; }
        public double InteractionBoundEndTimeInMs { get; set; }
        public bool IsHitAttempted { get; private set; } = false;

        public int AccuracyLaybackMs { get; set; } = 100;
        public int GroupNumberShownOnCircle { get; set; }

        private float alpha { get; set; } = 0;

        public Visibility Visibility { get; set; }
        private YieldInstruction instruction = new YieldInstruction();

        private double shrinkDuration;

        public event EventHandler OnInteract;
        // Use this for initialization
        void Start()
        {
            transform.GetComponent<Rigidbody2D>().simulated = false;
            transform.GetComponent<CircleCollider2D>().enabled = false;
            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            transform.GetChild(1).GetComponent<Text>().text = GroupNumberShownOnCircle.ToString();
            SetAlpha(alpha);

            InteractionBoundStartTimeInMs = PerfectInteractionTimeInMs - AccuracyLaybackMs;
            InteractionBoundEndTimeInMs = PerfectInteractionTimeInMs + AccuracyLaybackMs;
            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = TapTapAimSetup.visibleStartOffsetMs,
                VisibleEndOffsetMs = 50
            };
            Visibility.VisibleStartStartTimeInMs = PerfectInteractionTimeInMs - Visibility.VisibleStartOffsetMs;
            Visibility.VisibleEndStartTimeInMs = PerfectInteractionTimeInMs + Visibility.VisibleEndOffsetMs;
            OnInteract += HitCircle_OnHitEvent;
            shrinkDuration = PerfectInteractionTimeInMs - Visibility.VisibleStartStartTimeInMs;
            ringStartScale = transform.GetChild(3).GetComponent<RectTransform>().localScale.x;
            gameObject.SetActive(false);
        }
        void Update()
        {
            if (!IsHitAttempted)
            {
                if (TapTapAimSetup.Tracker.GetTimeInMs() >= Visibility.VisibleStartStartTimeInMs)
                {
                    FadeIn();
                    TimingRingShrink();
                }

                if (IsInInteractionBound(TapTapAimSetup.Tracker.GetTimeInMs()))
                {

                    transform.GetComponent<Rigidbody2D>().simulated = true;
                    transform.GetComponent<CircleCollider2D>().enabled = true;
                }

                if (TapTapAimSetup.Tracker.GetTimeInMs() >= Visibility.VisibleEndStartTimeInMs && !Visibility.fadeOutTriggered)
                {
                    transform.GetComponent<Rigidbody2D>().simulated = false;
                    Outcome(TapTapAimSetup.Tracker.GetTimeInMs(), false);
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
        public bool IsInCircleLifeBound(double time)
        {
            if (time >= Visibility.VisibleStartStartTimeInMs
                && time <= Visibility.VisibleEndStartTimeInMs)
            {
                return true;
            }
            return false;
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
            if (time >= PerfectInteractionTimeInMs - 10
                && time <= PerfectInteractionTimeInMs + 50)
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
                Debug.Log(QueueID + "tryHit Triggered. : " + hitTime + "Perfect time =>" + PerfectInteractionTimeInMs + "   IsInBounds:" +
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


        bool shrinkDone;
        float ringStartScale;
        float shrinkMinScale = 1.05f;
        void TimingRingShrink()
        {

            if (shrinkDone)
                return;

            var shrinkTParam = (float)((PerfectInteractionTimeInMs - TapTapAimSetup.Tracker.GetTimeInMs()) / shrinkDuration);

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


        bool fadeInDone;
        void FadeIn()
        {
            if (fadeInDone)
                return;
            var fadeDuration = Visibility.VisibleStartOffsetMs * 0.7f;

            var fadeInTParam = (float)((Visibility.VisibleStartStartTimeInMs + fadeDuration  - TapTapAimSetup.Tracker.GetTimeInMs()) / fadeDuration);
            if (fadeInTParam >= 1)
            {
                fadeInTParam = 1;
                fadeInDone = true;
            }
            else if (fadeInTParam < 0)
                fadeInTParam = 0;

            var alpha = 1 + fadeInTParam * (0 - 1);

            SetAlpha(alpha);
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

        private void Outcome(double timeInMs, bool hit)
        {
            if (hit)
            {
                var cs = new HitScore()
                {
                    id = QueueID,
                    accuracy = GetAccuracy(timeInMs)
                };
                Debug.Log($"Accuracy : {cs.accuracy}");
               
                if (cs.accuracy <= 20f)
                {
                    cs.score = 100;
                }
                else if (cs.accuracy <= 60f)
                {
                    cs.score = 50;
                }
                else
                {
                    cs.score = 25;
                }

                
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
            Destroy(gameObject, 1);
        }
        //TODO: scale with HasAttemptHit window
        private float GetAccuracy(double hitTime)
        {
            return Mathf.Abs((float)(( hitTime - PerfectInteractionTimeInMs) / 150)) * 100;

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
