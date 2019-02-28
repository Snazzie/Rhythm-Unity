using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.TapTapAim.Assets.TapTapAim;
using Assets.TapTapAim.LineUtility;
using UnityEngine;

namespace Assets.TapTapAim
{
    public class TapTapAimSetup : MonoBehaviour, ITapTapAimSetup
    {

        public Transform HitCircleTransform;
        public Transform CircleTransform;
        public Transform HitSliderTransform;
        public Transform Slider;
        public Transform SliderPositionRing;
        public Transform SliderHitCircleTransform;
        public static int AccuracyLaybackMs { get; } = 100;
        public Transform PlayArea { get; set; }
        public List<IObject> HitObjectQueue { get; } = new List<IObject>();
        public List<IQueuable> ObjToActivateQueue { get; } = new List<IQueuable>();
        public ITracker Tracker { get; set; }

        private bool ready;
        private Dispatcher _dispatcher;
        private int offset = 4000;
        private bool AddOffset { get; set; }

        public AudioSource MusicSource { get; set; }

        public AudioSource HitSource { get; set; }
        private int prevGroupID { get; set; } = -1;
        private int groupIDCount { get; set; } = 0;
        private bool showSliders = true;
        private bool showCircles = true;
        private int queueID = -1;
        private int HitID { get; set; } = -1;
        private int getObjectNameID = -1;

        void Start()
        {
            PlayArea = GameObject.Find("PlayArea").transform;
            HitSource = GameObject.Find("HitSource").GetComponent<AudioSource>();
            MusicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
            MusicSource.clip = GameStartParameters.MapJson.audioClip;
            Tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
            Tracker.TapTapAimSetup = this;

            if (TimeSpan.FromMilliseconds(int.Parse(GameStartParameters.MapJson.map[0].Split(',')[2])) <
                TimeSpan.FromMilliseconds(offset))
            {
                AddOffset = true;
                Tracker.StartOffset = offset;
            }

            InstantiateObjects();
        }

        private int GetQueueId()
        {
            return queueID += 1;
        }
        private int GetHitID()
        {
            return HitID += 1;
        }
        private string GetObjectNameID()
        {
            return (getObjectNameID += 1).ToString();
        }
        void InstantiateObjects()
        {

            for (var index = 0; index < GameStartParameters.MapJson.map.Count; index++)
            {

                var hitObject = GameStartParameters.MapJson.map[index].Split(',');
                if (hitObject.Length == 7)
                {
                    //spinner
                }
                else if (hitObject.Length == 6)
                {
                    //circle
                    if (showCircles)
                    {
                        var circle = CreateHitCircle(index, hitObject);
                        circle.HitID = GetHitID();
                        ObjToActivateQueue.Add(circle);
                        HitObjectQueue.Add(circle);
                    }
                }
                else
                {
                    //slider
                    if (showSliders)
                    {
                        var slider = CreateHitSlider(index, hitObject);
                        if (slider != null)
                        {
                            //HitObjectQueue.Add(slider);
                            ObjToActivateQueue.Add(slider);
                            slider.InitialHitCircle.HitID = GetHitID();
                            //slider.SliderPositionRing.HitID = GetHitID();
                            HitObjectQueue.Add(slider.InitialHitCircle);

                            //HitObjectQueue.Add(slider.SliderPositionRing);
                        }
                    }
                }
            }

            for (int i = 0; i < ObjToActivateQueue.Count; i++)
            {
                ObjToActivateQueue[i].QueueID = i;
            }

            var count = 0;
            for (int i = ObjToActivateQueue.Count - 1; i >= 0; i--)
            {
                ((MonoBehaviour)ObjToActivateQueue[i]).transform.SetSiblingIndex(count);
                count++;
            }
            Tracker.SetGameReady();
        }

        private HitSlider CreateHitSlider(int index, string[] hitObject)
        {

            var format = new SliderFormat(hitObject);
            if (format.type == SliderType.PerfectCurve)
                return null; // not implemented yet


            var instance = Instantiate(HitSliderTransform, PlayArea).GetComponent<HitSlider>();

            instance.TapTapAimSetup = this;
            instance.name = GetObjectNameID() + "-HitSlider";
            instance.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            instance.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, -0.1f);
            instance.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            instance.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0f);

            instance.transform.localScale = new Vector2(1f, 1f);

            if (format.group == prevGroupID)
            {
                instance.Number = groupIDCount += 1;
            }
            else
            {
                prevGroupID = format.group;
                instance.Number = groupIDCount = 1;
            }

            var circleFormat = new CircleFormat
            {
                x = format.x,
                y = format.y,
                time = format.time,
                group = format.group
            };

            var sliderHitcircleInstance = CreateSliderHitCircle(circleFormat);
            sliderHitcircleInstance.transform.parent = instance.transform;
            sliderHitcircleInstance.name = GetObjectNameID() + "-SliderHitCircle";

            var sliderPositionRingInstance = Instantiate(SliderPositionRing, instance.transform).GetComponent<SliderPositionRing>();
            sliderPositionRingInstance.GetComponent<RectTransform>().position = sliderHitcircleInstance.GetComponent<RectTransform>().position;

            sliderPositionRingInstance.name = GetObjectNameID() + "-SliderPositionRing";

            var sliderInstance = Instantiate(
                    Slider,
                    transform
                    ).GetComponent<Slider>();
            sliderInstance.transform.parent = instance.transform;
            sliderInstance.LineRenderer = instance.GetComponent<LineRenderer>();
            sliderInstance.SliderPositionRing = sliderPositionRingInstance;
            sliderInstance.Points = format.points;
            sliderInstance.SliderType = format.type;
            sliderInstance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            sliderInstance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            sliderInstance.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
            sliderInstance.DrawSlider();

            instance.SetUp(
                sliderHitcircleInstance,
                sliderInstance,
                sliderPositionRingInstance,
                GetPerfectTime(format),
                format.reverseTimes, this);

            return instance;

        }

        private TimeSpan GetPerfectTime(Format format)
        {
            return TimeSpan.FromMilliseconds(format.time + (AddOffset ? offset : 0));
        }

        private HitCircle CreateHitCircle(int index, string[] hitObject)
        {
            var instance = Instantiate(HitCircleTransform, PlayArea).GetComponent<HitCircle>();
            instance.name = GetObjectNameID() + "-HitCircle";
            instance.TapTapAimSetup = this;

            var format = new CircleFormat(hitObject);
            if (format.group == prevGroupID)
            {
                instance.Number = groupIDCount += 1;
            }
            else
            {
                prevGroupID = format.group;
                instance.Number = groupIDCount = 1;
            }
            instance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            instance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);

            instance.GetComponent<RectTransform>().anchoredPosition = new Vector3(format.x, format.y, 0);
            instance.transform.localScale = new Vector2(1f, 1f);

            instance.PerfectHitTime = GetPerfectTime(format);
            return instance;
        }

        private SliderHitCircle CreateSliderHitCircle(CircleFormat circleFormat)
        {
            var instance = Instantiate(SliderHitCircleTransform, PlayArea).GetComponent<SliderHitCircle>();
            instance.TapTapAimSetup = this;


            instance.name = "Hit Circle";
            instance.Number = groupIDCount;

            instance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            instance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);

            instance.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(circleFormat.x, circleFormat.y, 0);
            instance.transform.localScale = new Vector2(1f, 1f);

            instance.PerfectHitTime = GetPerfectTime(circleFormat);
            return instance;
        }

        private class Format
        {
            public float x;
            public float y;
            public int time;
            public int group { get; set; }
        }

        private class SpinnerFormat : Format
        {
            public SpinnerFormat() { }

        }
        private class CircleFormat : Format
        {
            public CircleFormat() { }
            public CircleFormat(string[] split)
            {

                x = float.Parse(split[0]);
                y = float.Parse(split[1]);
                time = int.Parse(split[2]);
                if (split.Length > 3)
                    group = int.Parse(split[3]);
            }
        }
        private class SliderFormat : Format
        {

            private List<Vector3> vectors = new List<Vector3>();

            public int reverseTimes = 0;
            public double length;
            public SliderType type;
            public SliderFormat() { }
            public List<Vector3> points = new List<Vector3>();
            public SliderFormat(string[] split)
            {
                x = float.Parse(split[0]);
                y = float.Parse(split[1]);
                time = int.Parse(split[2]);
                group = int.Parse(split[3]);
                var typeAndAnchorSplit = split[5].Split('|');


                var anchors = typeAndAnchorSplit.Skip(1).ToArray();
                foreach (var anchor in anchors)
                {
                    var xy = anchor.Split(':').Select(float.Parse).ToArray();
                    vectors.Add(new Vector2(xy[0], xy[1]));
                }
                switch (typeAndAnchorSplit[0])
                {
                    case "L":
                        type = SliderType.LinearLine;
                        points = LinearLine.GetPoints(new Vector2(x, y), vectors[0]);
                        break;
                    case "P":
                    //type = SliderType.PerfectCurve;
                    ////
                    //break;
                    case "B":
                        type = SliderType.BezierCurve;
                        var list = new List<Vector3>(vectors);
                        list.Insert(0, new Vector3(x, y, 0));
                        points = BezierCurve.GetPoints(list);
                        break;
                }
                reverseTimes = int.Parse(split[6]);
                length = double.Parse(split[7]);
            }

            private void SetPoints()
            {

            }
        }


    }
}
