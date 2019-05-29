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
        public TimeSpan PerfectInteractionTime { get; set; }
        public int AccuracyLaybackMs { get; set; }

        public TimeSpan InteractionBoundStart { get; set; }

        public TimeSpan InteractionBoundEnd { get; set; }
        void Update()
        {
            if (IsPastLifeBound())
            {
                transform.GetComponent<Rigidbody2D>().simulated = false;
                transform.GetComponent<CircleCollider2D>().enabled = false;

                ((Tracker)TapTapAimSetup.Tracker).IterateInteractionQueue(InteractionID);
                //Debug.LogError($" HitId:{InteractionID} Not hit attempted.  next hit id: {TapTapAimSetup.Tracker.NextObjToHit}");
                //Outcome(TapTapAimSetup.Tracker.Stopwatch.Elapsed, false);

            }
        }
        public bool IsPastLifeBound()
        {
            return TapTapAimSetup.Tracker.Stopwatch.Elapsed >= InteractionBoundEnd;
        }
        public void TryInteract()
        {
            throw new NotImplementedException();
        }

        public bool IsInInteractionBound(TimeSpan time)
        {
            if (time >= InteractionBoundStart && time <= InteractionBoundEnd)
            {
                return true;
            }
            return false;
        }
        public bool IsInAutoPlayHitBound(TimeSpan time)
        {
            if (time >= InteractionBoundStart && time <= InteractionBoundEnd)
            {
                return true;
            }
            return false;
        }

    }

}