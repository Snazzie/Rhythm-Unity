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
        public double PerfectHitTimeInMs { get; set; }
        public double VisibleStartOffsetMs { get; } = 400;
        public double VisibleEndOffsetMs { get; } = 50;
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public ICircle BlankCircle { get; set; }
        public ISliderHitCircle InitialHitCircle { get; set; }
        public SliderPositionRing SliderPositionRing { get; set; }
        public Slider Slider { get; set; }
        public int SliderTrips { get; set; }
        public int Number { get; set; }
        public double TripMs { get; set; }
        public double EndOfLifeTimeInMs { get; set; }
        public float Progress { get; set; }
        public bool GoingForward { get; set; }
        private bool Ready { get; set; }
        public bool fadeInTriggered { get; set; }
        private float alpha = 0;
        public int AccuracyLaybackMs { get; set; } = 100;
        private bool positionRingDone;
        private List<double> tripTimesInMs { get; set; }
        [SerializeField]
        [Range(0.0f, 100.0f)]
        private float tParam;
        public void SetUp(ISliderHitCircle initialHitCircle, ISlider slider, ISliderPositionRing sliderPositionRing, double perfectHitTimeInMs, int sliderTrips, ITapTapAimSetup tapTapAimSetup)
        {
            InitialHitCircle = initialHitCircle;
            ((SliderHitCircle)InitialHitCircle).ParentSlider = this;
            Slider = (Slider)slider;
            SliderPositionRing = (SliderPositionRing)sliderPositionRing;
            PerfectHitTimeInMs = perfectHitTimeInMs;
            SliderTrips = sliderTrips;
            TapTapAimSetup = TapTapAimSetup;
            GoingForward = true;
            Ready = true;
            EndOfLifeTimeInMs = perfectHitTimeInMs + (TripMs * SliderTrips);
            TapTapAimSetup.Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            SetAlpha(alpha);
            Visibility = new Visibility()
            {
                VisibleStartOffsetMs = 400,
                VisibleEndOffsetMs = 100
            };
            tripTimesInMs = new List<double>();
            var totalSliderLifeTimeMs = TripMs * SliderTrips;

            for (int i = 0; i < SliderTrips; i++)
            {
                tripTimesInMs.Add(perfectHitTimeInMs + ((i + 1) * TripMs ));
            }

            

            Visibility.VisibleStartStartTimeInMs = PerfectHitTimeInMs - VisibleStartOffsetMs;
            Visibility.VisibleEndStartTimeInMs = PerfectHitTimeInMs + totalSliderLifeTimeMs - VisibleEndOffsetMs;
            sliderPositionRing.PerfectInteractionTimeInMs = PerfectHitTimeInMs;
            sliderPositionRing.InteractionBoundStartTimeInMs = perfectHitTimeInMs;
            sliderPositionRing.InteractionBoundEndTimeInMs = perfectHitTimeInMs + totalSliderLifeTimeMs;


            gameObject.SetActive(false);
        }

        void Update()
        {

            if (TapTapAimSetup.Tracker.GetTimeInMs() >= Visibility.VisibleStartStartTimeInMs)
            {
                ((MonoBehaviour)InitialHitCircle).enabled = true;
                ((MonoBehaviour)InitialHitCircle).gameObject.SetActive(true);
                StartCoroutine(FadeIn());

            }
            if (TapTapAimSetup.Tracker.GetTimeInMs() >= EndOfLifeTimeInMs && !fadeOutTriggered)
            {
                StartCoroutine(FadeOut());
                Destroy(gameObject, 1);
            }

        }
        void FixedUpdate()
        {
            if (TapTapAimSetup.Tracker.GetTimeInMs() >= SliderPositionRing.InteractionBoundStartTimeInMs && !StopCalculatingSliderPositionRing)
            {
                StartCoroutine(MoveSliderPositionRing());
            }
        }

        bool previousDirectionIsForward;

        IEnumerator MoveSliderPositionRing()
        {
                       
            var frameTime = TapTapAimSetup.Tracker.GetTimeInMs();
            try
            {
                int indexOf = 0;
                for (int i = 0; i < tripTimesInMs.Count; i++)
                {
                    if (frameTime < tripTimesInMs[i])
                    {
                        indexOf = i;
                        break;
                    }
                }

                //Debug.Log($"id: {QueueID}:  {indexOf + 1}/ {tripTimesInMs.Count}");
                GoingForward = indexOf % 2 != 0;


                var thisTripDestinationTimeInMs = tripTimesInMs[indexOf];


                tParam = GoingForward
                    ? (float)((thisTripDestinationTimeInMs - frameTime) / TripMs)
                    : 1 - (float)((thisTripDestinationTimeInMs - frameTime) / TripMs);


                //Debug.Log($"id:{QueueID} TParam: {TParam}");
                SliderPositionRing.transform.localPosition = Slider.GetPositionAtTime(Mathf.Clamp(tParam, 0f, 1f));

                if (GoingForward != previousDirectionIsForward)
                    TapTapAimSetup.HitSource.Play();
                previousDirectionIsForward = GoingForward;


                if (frameTime > EndOfLifeTimeInMs)
                {
                    TapTapAimSetup.HitSource.Play();
                    StopCalculatingSliderPositionRing = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            yield return null;
        }

        bool fadeInDone;
        IEnumerator FadeIn()
        {
            if (fadeInDone)
                yield return null;

            var fadeDuration = Visibility.VisibleStartOffsetMs * 0.7f;

            var fadeInTParam = (float)((Visibility.VisibleStartStartTimeInMs + fadeDuration - TapTapAimSetup.Tracker.GetTimeInMs()) / fadeDuration);
            if (fadeInTParam >= 1)
            {
                fadeInTParam = 1;
                fadeInDone = true;
            }
            else if (fadeInTParam < 0)
                fadeInTParam = 0;

            var alpha = 1 + fadeInTParam * (0 - 1);

            SetAlpha(alpha);
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
        public bool StopCalculatingSliderPositionRing { get; private set; }

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