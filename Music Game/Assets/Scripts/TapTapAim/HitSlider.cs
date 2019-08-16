using System;
using System.Collections;
using System.Collections.Generic;
using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.UI;
using Slider = Assets.Scripts.TapTapAim.Slider;

namespace Assets.Scripts.TapTapAim
{
    public class HitSlider : MonoBehaviour, IHitSlider
    {

        public int QueueID { get; set; }
        public TimeSpan PerfectHitTime { get; set; }
        public int VisibleStartOffsetMs { get; } = 400;
        public int VisibleEndOffsetMs { get; } = 50;
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public ICircle BlankCircle { get; set; }
        public ISliderHitCircle InitialHitCircle { get; set; }
        public SliderPositionRing SliderPositionRing { get; set; }
        public Slider Slider { get; set; }
        public int Bounces { get; set; }
        public int Number { get; set; }
        public float DurationMs { get; set; }
        public TimeSpan End { get; set; }
        public float Progress { get; set; }
        public bool GoingForward { get; set; }
        private bool Ready { get; set; }
        public bool fadeInTriggered { get; set; }
        private float alpha = 0;
        public int AccuracyLaybackMs { get; set; } = 100;
        private bool positionRingDone;
        private List<(double, double)> directionOfTravelBetweenRange { get; set; }
        [SerializeField]
        [Range(0.0f, 100.0f)]
        private float TParam;
        public void SetUp(ISliderHitCircle initialHitCircle, ISlider slider, ISliderPositionRing sliderPositionRing, TimeSpan perfectHitTime, int bounces, ITapTapAimSetup tapTapAimSetup)
        {
            InitialHitCircle = initialHitCircle;
            ((SliderHitCircle)InitialHitCircle).ParentSlider = this;
            Slider = (Slider)slider;
            SliderPositionRing = (SliderPositionRing)sliderPositionRing;
            PerfectHitTime = perfectHitTime;
            Bounces = bounces;
            TapTapAimSetup = TapTapAimSetup;
            GoingForward = true;
            Ready = true;
            End = perfectHitTime + TimeSpan.FromMilliseconds(DurationMs);
            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            SetAlpha(alpha);
            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 100
            };
            directionOfTravelBetweenRange = new List<(double, double)>();
            var incrementLengthMs = DurationMs / bounces;
            var rangeStart = perfectHitTime.TotalMilliseconds;
            for (int i = 0; i < bounces; i++)
            {
                directionOfTravelBetweenRange.Add((rangeStart, rangeStart += incrementLengthMs));
            }

            Visibility.VisibleStartStart = PerfectHitTime - TimeSpan.FromMilliseconds(VisibleStartOffsetMs);
            Visibility.VisibleEndStart = PerfectHitTime + TimeSpan.FromMilliseconds(DurationMs) - TimeSpan.FromMilliseconds(VisibleEndOffsetMs);
            sliderPositionRing.PerfectInteractionTime = PerfectHitTime;
            sliderPositionRing.InteractionBoundStart = perfectHitTime;
            sliderPositionRing.InteractionBoundEnd = perfectHitTime + TimeSpan.FromMilliseconds(DurationMs);


            gameObject.SetActive(false);
        }

        void Update()
        {

            if (!fadeInTriggered && TapTapAimSetup.Tracker.Stopwatch.Elapsed >= Visibility.VisibleStartStart)
            {
                ((MonoBehaviour)InitialHitCircle).enabled = true;
                ((MonoBehaviour)InitialHitCircle).gameObject.SetActive(true);
                StartCoroutine(FadeIn());

            }
            if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= PerfectHitTime + TimeSpan.FromMilliseconds(DurationMs))
            {
                StartCoroutine(FadeOut());
                Destroy(gameObject, 1);
            }

            if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= PerfectHitTime && !positionRingDone)
            {
                try
                {
                    int indexOf = 0;
                    for (int i = 0; i < directionOfTravelBetweenRange.Count; i++)
                    {
                        if (TapTapAimSetup.Tracker.Stopwatch.Elapsed.TotalMilliseconds >= directionOfTravelBetweenRange[i].Item1 &&
                            TapTapAimSetup.Tracker.Stopwatch.Elapsed.TotalMilliseconds < directionOfTravelBetweenRange[i].Item2)
                        {
                            indexOf = i;
                            break;
                        }
                    }

                    GoingForward = indexOf % 2 == 0;

                    // currently doesnt consider bounces
                    TParam = ((float)(TapTapAimSetup.Tracker.Stopwatch.Elapsed - PerfectHitTime).TotalMilliseconds / (DurationMs / Bounces + 1));// +1 so 1 bounce means, if bounces is 1, duration is halved.

                    if (TParam > 1)
                    {
                        SliderPositionRing.transform.localPosition = Slider.GetPositionAtTime(1);
                    }
                    else
                    {
                        if (GoingForward)
                            SliderPositionRing.transform.localPosition = Slider.GetPositionAtTime(TParam);
                    }
                    // handle Tparam direction using 100 -tparam
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            }


        }



        IEnumerator FadeIn()
        {
            fadeInTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1 != fadeOutTriggered)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(elapsedTime * 4));
            }
        }
        private void Outcome(TimeSpan time, bool hit)
        {

            // gameObject.SetActive(false);
            Destroy(gameObject, 3);
        }
        private void SetAlpha(float alpha)
        {
            List<Image> children = new List<Image>();
            List<Text> textChild = new List<Text>();
            LineRenderer lRenderer = transform.GetComponent<LineRenderer>();

            foreach (Transform child in transform)
            {
                foreach (var ic in child.GetComponentsInChildren<Image>())
                {
                    children.Add(ic);
                }

                foreach (var tc in child.GetComponentsInChildren<Text>())
                {
                    textChild.Add(tc);
                }

            }


            var start = Color.white;
            start.a = alpha;
            var end = Color.white;
            end.a = alpha;
            lRenderer.startColor = start;
            lRenderer.endColor = end;
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

        private YieldInstruction instruction = new YieldInstruction();

        public bool fadeOutTriggered { get; set; }
        public Visibility Visibility { get; set; }

        IEnumerator FadeOut()
        {
            fadeOutTriggered = true;
            float elapsedTime = 0.0f;

            while (elapsedTime < 1)
            {
                yield return instruction;
                elapsedTime += Time.deltaTime;
                SetAlpha(1.0f - Mathf.Clamp01(elapsedTime * 9));
            }

        }
    }
}