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
        public float Duration { get; set; }
        public float Progress { get; set; }
        public bool GoingForward { get; set; }
        public bool LookForward { get; set; }
        private bool Ready { get; set; }
        public bool fadeInTriggered { get; set; }
        private float alpha = 0;
        public int AccuracyLaybackMs { get; set; } = 100;
        private bool positionRingDone;
        private bool isGoingForward { get; set; } = true;

        float t;

        private int pointToFollow = 0;
        double timeToReachTarget;
        private float speed = 2f;
        private float tParam = 0f;

        public void SetUp(ISliderHitCircle initialHitCircle, ISlider slider, ISliderPositionRing sliderPositionRing, TimeSpan perfectHitTime, int bounces, ITapTapAimSetup tapTapAimSetup)
        {
            InitialHitCircle = initialHitCircle;
            ((SliderHitCircle)InitialHitCircle).ParentSlider = this;
            Slider = (Slider)slider;
            SliderPositionRing = (SliderPositionRing)sliderPositionRing;
            PerfectHitTime = perfectHitTime;
            Bounces = bounces;
            TapTapAimSetup = TapTapAimSetup;
            Ready = true;

            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            SetAlpha(alpha);
            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 100
            };
            Visibility.VisibleStartStart = PerfectHitTime - TimeSpan.FromMilliseconds(VisibleStartOffsetMs);
            Visibility.VisibleEndStart = PerfectHitTime + TimeSpan.FromMilliseconds(Duration) - TimeSpan.FromMilliseconds(VisibleEndOffsetMs);
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
            if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= PerfectHitTime + TimeSpan.FromMilliseconds(Duration))
            {
                StartCoroutine(FadeOut());
                Destroy(gameObject, 1);
            }

            if (TapTapAimSetup.Tracker.Stopwatch.Elapsed >= PerfectHitTime && !positionRingDone)
            {
                try
                {
                    tParam += Time.fixedDeltaTime * speed;
                    SliderPositionRing.transform.localPosition =Vector3.Lerp(SliderPositionRing.transform.localPosition, Slider.Points[pointToFollow], tParam);

                    if (SliderPositionRing.transform.localPosition == Slider.Points[pointToFollow])
                    {
                        if (isGoingForward)
                            pointToFollow++;
                        else
                            pointToFollow--;
                    }

                    if (pointToFollow == Slider.Points.Count || pointToFollow == -1)
                    {
                        if (Bounces == 0)
                        {
                            positionRingDone = true;
                        }
                        else
                        {
                            isGoingForward = !isGoingForward;
                            if (!isGoingForward)
                            {
                                pointToFollow -= 2;
                            }
                            else
                            {
                                pointToFollow += 2;
                            }
                        }
                    }
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