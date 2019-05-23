using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.TapTapAim;
using UnityEngine;

namespace Assets.Scripts.TapTapAim
{
    public interface ITapTapAimSetup
    {
        ITracker Tracker { get; set; }
        AudioSource MusicSource { get; }
        AudioSource HitSource { get; }
        Transform PlayArea { get; set; }
        List<IInteractable> ObjectInteractQueue { get; }
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
        int NextObjToHit { get; }
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
    public interface ICircle : IObject{}
    public interface IHitCircle : ICircle, IHittable, IQueuable, IDisplaysGroupNumber { }
    public interface ISliderHitCircle : ICircle, IHittable, IDisplaysGroupNumber { }
    public interface IInteractable : IObject
    {
        int InteractionID { get; set; }
        void TryInteract();
        TimeSpan PerfectInteractionTime { get; set; }
        int AccuracyLaybackMs { get; set; }

        TimeSpan InteractionBoundStart { get; }
        TimeSpan InteractionBoundEnd { get; }
        bool IsInInteractionBound(TimeSpan time);
        event EventHandler OnInteract;
    }

    public interface IHittable : IInteractable
    {
        bool IsHitAttempted { get; }

    }
    public interface IHoldable : IInteractable { }

    public interface IDisplaysGroupNumber
    {
        int GroupNumberShownOnCircle { get; set; }
    }

    public interface ISlider
    {

        List<Vector3> Points { get; set; }
        SliderType SliderType { get; }
    }

    public interface ISliderPositionRing : IHoldable
    {

    }

    public interface IHitSlider : IObject, IQueuable
    {
        int Number { get; set; }

        ISliderHitCircle InitialHitCircle { get; }
        Slider Slider { get; }
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
