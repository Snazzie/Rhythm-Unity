using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Assets.TapTapAim
{
    public class Tracker : MonoBehaviour, ITracker
    {
        private int nextObjectID;
        private bool SkippedToStart;
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public int Score { get; private set; }
        public List<HitScore> HitHistory { get; set; } = new List<HitScore>();
        private float HealthDrain { get; } = 5;
        private float HealthDamage { get; } = 20;
        public float HealthAddedPerHit { get; } = 7;
        public float HitAccuracy { get; private set; }
        public List<TimeSpan> BreakPeriodQueue { get; private set; } = new List<TimeSpan>();
        public int StartOffset { get; set; }
        public int NextObjToHit { get; set; } = 0;
        public int NextObjToActivateID { get; set; }
        public int Combo { get; private set; }
        public float Health { get; private set; } = 100;
        public bool IsGameReady { get; set; }
        public bool GameFinished { get; private set; }
        public Stopwatch Stopwatch { get; } = new Stopwatch();

        private void Start()
        {


        }
        public void SetGameReady()
        {
            Stopwatch.Start();

            IsGameReady = true;
        }

        private void Update()
        {
            if (!IsGameReady)
                return;
            try
            {
                if(NextObjToActivateID < TapTapAimSetup.ObjActivationQueue.Count)
                IterateObjectQueue();
            }
            catch 
            {
                //Debug.LogError(exception);
            }

            try
            {
                IterateHitQueue();
            }
            catch
            {

            }
            if (IsGameReady && OffsetOver() && !GameFinished)
            {

                if (!TapTapAimSetup.MusicSource.isPlaying)
                    TapTapAimSetup.MusicSource.Play();
                HandleHealth();
            }

            CalculateAccuracy();

            if (Input.GetKey(KeyCode.Escape))
                SceneManager.LoadScene("MapSelect");
            else if (Input.GetKey(KeyCode.Space))
                if (TimeSpan.FromSeconds(TapTapAimSetup.MusicSource.time) - TimeSpan.FromSeconds(5) <
                    TapTapAimSetup.HitObjectQueue[0].Visibility.VisibleStartStart && !SkippedToStart)
                {
                    SkippedToStart = true;
                    TapTapAimSetup.MusicSource.time = (float)((IObject)TapTapAimSetup.HitObjectQueue[0]).Visibility.VisibleStartStart.TotalSeconds - 5f;
                }

        }

        private void CalculateAccuracy()
        {
            try
            {
                float sum = 0;
                var count = 0;

                foreach (var hit in HitHistory)
                {
                    sum += hit.accuracy;
                    count++;
                }

                HitAccuracy = sum / count;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private bool OffsetOver()
        {
            if (Stopwatch.Elapsed < TimeSpan.FromMilliseconds(StartOffset) && StartOffset != 0) return false;

            return true;
        }

        private TimeSpan TimeNormalized()
        {
          return TapTapAimSetup.Offset != 0 ? Stopwatch.Elapsed - TimeSpan.FromMilliseconds(TapTapAimSetup.Offset) : Stopwatch.Elapsed;
        }
        private void IterateObjectQueue()
        {
            //if (Stopwatch.Elapsed + TimeSpan.FromMilliseconds(500) >= ((IObject)TapTapAimSetup.HitObjectQueue[nextObjectID]).VisibleStartStart)
            //{
            //    ((MonoBehaviour)TapTapAimSetup.HitObjectQueue[nextObjectID]).gameObject.SetActive(true);
            //    nextObjectID++;
            //}
            if (Stopwatch.Elapsed + TimeSpan.FromMilliseconds(1000) >= TapTapAimSetup.ObjActivationQueue[NextObjToActivateID].Visibility.VisibleStartStart)
            {
                ((MonoBehaviour)(TapTapAimSetup).ObjActivationQueue[NextObjToActivateID]).gameObject.SetActive(true);
                NextObjToActivateID++;

            }
            if (nextObjectID == (TapTapAimSetup).ObjActivationQueue.Count && nextObjectID == TapTapAimSetup.HitObjectQueue.Count)
                GameFinished = true;
        }

        public void IterateHitQueue(int? thisId = null)
        {
            if (thisId != null)
            {
                NextObjToHit= (int)thisId + 1;
                return;
            }
            if (Stopwatch.Elapsed >= (TapTapAimSetup.HitObjectQueue[NextObjToHit]).PerfectHitTime + TimeSpan.FromMilliseconds(TapTapAimSetup.AccuracyLaybackMs))
            {
                NextObjToHit++;
            }
        }



        private void HandleHealth()
        {
            Health -= Time.deltaTime * HealthDrain;

            if (Health <= 0)
            {
                // GameFinished = true;
            }
            else if (Health > 100)
            {
                Health = 100;
            }
        }

        public void RecordEvent(bool hit, HitScore hitScore = null)
        {
            if (hit)
            {
                Combo++;
                if (hitScore != null)
                {
                    HitHistory.Add(hitScore);
                    Score += hitScore.score * (Combo + 1);

                    Health += HealthAddedPerHit;
                }
            }
            else
            {
                Combo = 0;
                Health -= HealthDamage;
            }
        }


    }
    public class HitScore
    {
        public float accuracy;
        public int id;
        public int score;
    }
}