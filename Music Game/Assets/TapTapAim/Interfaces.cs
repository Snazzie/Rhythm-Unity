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
        List<IHittable> HitObjectQueue { get; }
        List<IQueuable> ObjActivationQueue { get; }
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

        TapTapAimSetup TapTapAimSetup { get; set; }
    }

    public interface IQueuable: IObject
    {
        int QueueID { get; set; }
    }
    public interface IObject
    {
        TapTapAimSetup TapTapAimSetup { get; set; }
        Visibility Visibility { get; set; }
    }
    public interface ICircle : IObject
    {
    }

    public interface IHittable : IObject
    {
        int HitID { get; set; }
        void TryHit();
        TimeSpan PerfectHitTime { get; set; }
        int AccuracyLaybackMs { get; set; }
        bool IsHitAttempted { get; }
    }

    public interface IHitCircle : ICircle, IHittable, IQueuable,IDisplaysGroupNumber{}

    public interface ISliderHitCircle : ICircle, IHittable, IDisplaysGroupNumber
    {
        event EventHandler OnHitOrShowSliderTimingCircleEvent;
    }
    public interface IDisplaysGroupNumber
    {
        int GroupNumberShownOnCircle { get; set; }
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
