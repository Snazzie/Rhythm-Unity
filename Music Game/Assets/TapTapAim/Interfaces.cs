using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.TapTapAim.LineUtility;
using UnityEngine;

namespace Assets.TapTapAim
{
    public interface ITapTapAimSetup
    {
        ITracker Tracker { get; set; }
        AudioSource MusicSource { get; }
        AudioSource HitSource { get; }
        Transform PlayArea { get; set; }
        List<IObject> HitObjectQueue { get; }
        List<IQueuable> ObjToActivateQueue { get; }
    }
    public interface ITracker
    {
        int Combo { get; }
        float Health { get; }
        bool IsGameReady { get; }
        bool GameFinished { get; }
        Stopwatch Stopwatch { get; }
        int StartOffset { get; set; }
        int NextObjToHit { get; set; }
        void RecordEvent(bool hit, HitScore hitScore = null);
        void SetGameReady();

        ITapTapAimSetup TapTapAimSetup { get; set; }
    }

    public interface IQueuable
    {
        int QueueID { get; set; }
    }
    public interface IObject
    {
        ITapTapAimSetup TapTapAimSetup { get; set; }
        int VisibleStartOffsetMs { get; }
        int VisibleEndOffsetMs { get; }
        TimeSpan VisibleStartStart { get; }
        TimeSpan VisibleEndStart { get; }
    }

    public interface ICircle : IObject
    {
    }

    public interface IHittable
    {
        int HitID { get; set; }
        TimeSpan PerfectHitTime { get; set; }


        int AccuracyLaybackMs { get; set; }

    }

    public interface IHitCircle : ICircle, IHittable, IQueuable
    {
        void TryHit();
        int Number { get; set; }
        bool IsHitAttempted { get; }
    }

    public interface ISliderHitCircle : ICircle, IHittable
    {

    }
    public interface ISlider
    {

        List<Vector3> Points { get; set; }
        SliderType SliderType { get; }
    }

    public interface ISliderPositionRing : IObject, IFollow
    {

    }

    public interface IFollow
    {
    }

    public interface IHitSlider : IObject, IQueuable
    {
        int Number { get; set; }

        ISliderHitCircle InitialHitCircle { get; }
        ISlider Slider { get; }
        List<Vector2> Points { get; set; }
        int Bounces { get; set; }
        float Duration { get; set; }
        float Progress { get; set; }
        bool GoingForward { get; set; }
        bool LookForward { get; set; }
    }
    public enum SliderType
    {
        LinearLine,
        PerfectCurve,
        BezierCurve
    }
}
