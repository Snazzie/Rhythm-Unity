using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using Visibility = Assets.TapTapAim.Visibility;

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
        double StartOffsetMs { get; set; }
        int NextObjToHit { get; }
        double GetTimeInMs();
        bool UseMusicTimeline { get; set; }
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
        double PerfectInteractionTimeInMs { get; set; }
        int AccuracyLaybackMs { get; set; }

        double InteractionBoundStartTimeInMs { get; set; }
        double InteractionBoundEndTimeInMs { get; set; }
        bool IsInInteractionBound(double time);
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
        Vector3 GetPositionAtTime(float tParam);
    }

    public interface ISliderPositionRing : IHoldable
    {

    }

    public interface IHitSlider : IObject, IQueuable
    {
        int Number { get; set; }

        ISliderHitCircle InitialHitCircle { get; }
        Slider Slider { get; }
        int SliderTrips { get; set; }
        double TripMs { get; set; }
        bool GoingForward { get; set; }
    }
    public enum SliderType
    {
        LinearLine,
        PerfectCurve,
        BezierCurve
    }
}
