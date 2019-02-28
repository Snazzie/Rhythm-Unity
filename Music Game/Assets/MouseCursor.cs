using System;
using System.Linq;
using Assets.TapTapAim;
using Assets.TapTapAim.Assets.TapTapAim;
using UnityEngine;
using UnityEngine.SceneManagement;
using HitCircle = Assets.TapTapAim.HitCircle;

namespace Assets
{
    public class MouseCursor : MonoBehaviour
    {
        public float Speed = 0.6f;
        public bool IsAutoPlay { get; set; }
        float t;
        Vector3 startPosition;
        Vector3 target;
        double timeToReachTarget;
        private TapTapAimSetup tapTapAimSetup;
        private IObject OnObject { get; set; }
        private bool IsUserControl { get; set; }
        public bool IsGame { get; set; }
        private float Radius { get; set; }
        // Start is called before the first frame update
        void Start()
        {

            IsAutoPlay = true;
            startPosition = new Vector3(0, 0, 0);
            IsGame = SceneManager.GetActiveScene().name == "TapTapAim";
            if (IsGame)
            {
                tapTapAimSetup = GameObject.Find("Tracker").transform.GetComponent<TapTapAimSetup>();
                Radius = transform.GetComponent<CircleCollider2D>().radius;
            }


        }

        // Update is called once per frame
        void Update()
        {
            if (Cursor.visible)
                Cursor.visible = false;



            if (IsGame)
            {
                var nextObj = ((MonoBehaviour)tapTapAimSetup.HitObjectQueue[tapTapAimSetup.Tracker.NextObjToHit]).transform;
                Debug.Log("Next object to hit: " + ((IHittable)tapTapAimSetup.HitObjectQueue[tapTapAimSetup.Tracker.NextObjToHit]).HitID);
                if (IsAutoPlay)
                {

                    //pos.y += 0.1f;
                    SetDestination(nextObj.position, (nextObj.GetComponent<IHittable>().PerfectHitTime - tapTapAimSetup.Tracker.Stopwatch.Elapsed).TotalSeconds * Speed);
                    t += Time.deltaTime / (float)timeToReachTarget;
                    transform.position = Vector3.Lerp(startPosition, target, t);
                }
                else
                {
                    transform.position = Input.mousePosition;
                }

                RayCast();
            }
            else
            {
                transform.position = Input.mousePosition;
            }



            transform.Rotate(Vector3.forward * -1000 * Time.deltaTime);

            //Debug.DrawLine(transform.position, transform.forward);
            //Debug.DrawRay(transform.position,transform.forward);

        }



        public void RayCast()
        {




            try
            {
                var hits = Physics2D.CircleCastAll(transform.position, Radius, transform.forward, 5);
                if (IsHittable(hits, out var type))
                {
                    Debug.Log("ontop of type: " + type);
                    //Debug.Log("On Circle: " + hit.transform.name);
                    if (IsAutoPlay)
                    {

                        if (type == typeof(HitCircle))
                        {
                            var obj = (HitCircle)OnObject;

                            if (!obj.IsHitAttempted && obj.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed))
                            {
                                Debug.Log("Try hit: " + obj.name);

                                obj.TryHit();
                                GameObject.FindWithTag("TapCounter").GetComponent<TapTicker>().IncrementButton(1);
                            }
                        }
                        else if (type == typeof(SliderHitCircle))
                        {
                            var obj = (SliderHitCircle)OnObject;
                            if (!obj.IsHitAttempted && obj.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed))
                            {
                                Debug.Log("Try hit: " + obj.name);

                                obj.TryHit();
                                GameObject.FindWithTag("TapCounter").GetComponent<TapTicker>().IncrementButton(1);
                            }

                        }
                        else if (type == typeof(SliderPositionRing))
                        {
                            var obj = (SliderPositionRing)OnObject;
                            //obj.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed);


                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


        }

        private bool IsHittable(RaycastHit2D[] array, out Type type)
        {
            var iDs = new int[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                try
                {
                    iDs[index] = int.Parse(array[index].transform.name.Split('-')[0]);
                }
                catch
                {

                }
            }

            try
            {
                int min = iDs.Min();
                int minIndex = iDs.ToList().IndexOf(min);


                try
                {
                    OnObject = array[minIndex].transform.GetComponent<HitCircle>();
                    if (OnObject != null)
                    {
                        type = typeof(HitCircle);
                        return true;
                    }
                }
                catch { }

                try
                {
                    OnObject = array[minIndex].transform.GetComponent<HitSlider>();
                    if (OnObject != null)
                    {
                        type = typeof(HitSlider);
                        return false;
                    }
                }
                catch { }
                try
                {
                    OnObject = array[minIndex].transform.GetComponent<SliderHitCircle>();
                    if (OnObject != null)
                    {
                        type = typeof(SliderHitCircle);
                        return true;
                    }
                }
                catch { }

            }
            catch { }

            type = null;
            return false;
        }

        public void SetDestination(Vector3 destination, double time)
        {
            t = 0;
            startPosition = transform.position;
            timeToReachTarget = time;
            target = destination;
        }
    }
}
