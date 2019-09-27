using System;
using Assets.TapTapAim;
using UnityEngine;

namespace Assets.Scripts.TapTapAim
{
    public class SliderPositionRing : MonoBehaviour, ISliderPositionRing
    {
        public event EventHandler OnInteract;
        public Visibility Visibility { get; set; }
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public int InteractionID { get; set; }
        public double PerfectInteractionTimeInMs { get; set; }
        public int AccuracyLaybackMs { get; set; }

        public double InteractionBoundStartTimeInMs { get; set; }

        public double InteractionBoundEndTimeInMs { get; set; }
        private bool done { get; set; }
        void Update()
        {
            if (IsPastLifeBound() && !done)
            {
                transform.GetComponent<Rigidbody2D>().simulated = false;
                transform.GetComponent<CircleCollider2D>().enabled = false;

                ((Tracker)TapTapAimSetup.Tracker).IterateInteractionQueue(InteractionID);
                //Debug.LogError($" HitId:{InteractionID} Not hit attempted.  next hit id: {TapTapAimSetup.Tracker.NextObjToHit}");
                //Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);
                done = true;
            }
        }
        public bool IsPastLifeBound()
        {
            return TapTapAimSetup.Tracker.GetTimeInMs() >= InteractionBoundEndTimeInMs;
        }
        public void TryInteract()
        {
            // do nothing for now
        }

        public bool IsInInteractionBound(double timeInMs)
        {
            if (timeInMs >= InteractionBoundStartTimeInMs && timeInMs <= InteractionBoundEndTimeInMs)
            {
                return true;
            }
            return false;
        }
        public bool IsInAutoPlayHitBound(double timeInMs)
        {
            if (timeInMs >= InteractionBoundStartTimeInMs && timeInMs <= InteractionBoundEndTimeInMs)
            {
                return true;
            }
            return false;
        }

    }

}