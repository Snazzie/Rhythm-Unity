using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.TapTapAim
{
    public class Tracker : MonoBehaviour, ITracker
    {
        private int nextObjectID { get; set; }
        private bool SkippedToStart;
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public int Score { get; private set; }
        public List<HitScore> HitHistory { get; set; } = new List<HitScore>();
        private float HealthDrain { get; } = 5;
        private float HealthDamage { get; } = 20;
        public float HealthAddedPerHit { get; } = 7;
        public float HitAccuracy { get; private set; }
        public List<TimeSpan> BreakPeriodQueue { get; private set; } = new List<TimeSpan>();
        public double StartOffsetMs { get; set; }
        public int NextObjToHit { get; set; } = 0;
        public int NextObjToActivateID { get; set; }
        public int Combo { get; private set; }
        public float Health { get; private set; } = 100;
        public bool IsGameReady { get; set; }
        public bool GameFinished { get; private set; }
        private Stopwatch Stopwatch { get; } = new Stopwatch();
        public bool UseMusicTimeline { get; set; }
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
                if (NextObjToActivateID < TapTapAimSetup.ObjActivationQueue.Count)
                    IterateObjectQueue();
            }
            catch
            {
                //Debug.LogError(exception);
            }

            try
            {
                //if (NextObjToHit < TapTapAimSetup.ObjectInteractQueue.Count)
                //    IterateInteractionQueue();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            if (IsGameReady && UseMusicTimeline && !GameFinished)
            {
                if (!TapTapAimSetup.MusicSource.isPlaying)
                    TapTapAimSetup.MusicSource.Play();
                HandleHealth();
            }
            else
            {
                GetTime();
            }

            CalculateAccuracy();

            if (Input.GetKey(KeyCode.Escape))
                SceneManager.LoadScene("MapSelect");
            else if (Input.GetKey(KeyCode.Space))
                if ((TapTapAimSetup.MusicSource.time * 1000) - 5000 <
                    TapTapAimSetup.ObjectInteractQueue[0].Visibility.VisibleStartStartTimeInMs && !SkippedToStart)
                {
                    SkippedToStart = true;
                    TapTapAimSetup.MusicSource.time = (float)TapTapAimSetup.ObjectInteractQueue[0].Visibility.VisibleStartStartTimeInMs - 2000f;
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
        private void IterateObjectQueue()
        {
            //if (Stopwatch.Elapsed + TimeSpan.FromMilliseconds(500) >= ((IObject)TapTapAimSetup.ObjectInteractQueue[nextObjectID]).VisibleStartStart)
            //{
            //    ((MonoBehaviour)TapTapAimSetup.ObjectInteractQueue[nextObjectID]).gameObject.SetActive(true);
            //    nextObjectID++;
            //}
            if (Stopwatch.Elapsed.TotalMilliseconds + 200 >= TapTapAimSetup.ObjActivationQueue[NextObjToActivateID].Visibility.VisibleStartStartTimeInMs)
            {
                ((MonoBehaviour)(TapTapAimSetup).ObjActivationQueue[NextObjToActivateID]).gameObject.SetActive(true);
                NextObjToActivateID++;

            }
            if (nextObjectID == (TapTapAimSetup).ObjActivationQueue.Count && nextObjectID == TapTapAimSetup.ObjectInteractQueue.Count)
                GameFinished = true;
        }

        
        /// <summary>
        /// Time in Ms
        /// Can be negative if offset is applied
        /// </summary>
        /// <returns></returns>
        public double GetTime()
        {
            if (UseMusicTimeline)
            {
                return TapTapAimSetup.MusicSource.time * 1000;
            }

            if (Stopwatch.Elapsed.TotalMilliseconds - StartOffsetMs >= 0)
            {
                UseMusicTimeline = true;
                return TapTapAimSetup.MusicSource.time * 1000;
            }
            else
            {
                return Stopwatch.Elapsed.TotalMilliseconds - StartOffsetMs;
            }

        }
        public void IterateInteractionQueue(int? thisId = null)
        {
            if (thisId != null)
            {
                NextObjToHit = (int)thisId + 1;
                Debug.Log($"set nextHitObj to:{NextObjToHit}");
                return;
            }
            //if (Stopwatch.Elapsed >= (TapTapAimSetup.ObjectInteractQueue[NextObjToHit]).PerfectInteractionTime + TimeSpan.FromMilliseconds(TapTapAimSetup.AccuracyLaybackMs))
            //{
            //    NextObjToHit++;
            //    Debug.Log($"iterate hitQueue to:{NextObjToHit}");
            //}
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