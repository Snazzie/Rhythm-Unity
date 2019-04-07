using System;
using System.Collections.Generic;
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
        private IObject previousOnTopOfObject { get; set; }
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
                //Debug.Log("Next object to hit: " + ((IHittable)tapTapAimSetup.HitObjectQueue[tapTapAimSetup.Tracker.NextObjToHit]).HitID);
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
                    if (previousOnTopOfObject != OnObject)
                    {
                        Debug.Log("ontop of type: " + OnObject.GetType());
                        previousOnTopOfObject = OnObject;
                    }

                    //Debug.Log("On Circle: " + hit.transform.name);
                    if (IsAutoPlay)
                    {

                        switch (OnObject)
                        {
                            case HitCircle hitCircle:
                                if (!hitCircle.IsHitAttempted && hitCircle.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed))
                                {
                                    Debug.Log("Try hit: " + hitCircle.name);

                                    hitCircle.TryHit();
                                    GameObject.FindWithTag("TapCounter").GetComponent<TapTicker>().IncrementButton(1);
                                }
                                break;

                            case SliderHitCircle sliderHitCircle:
                                {
                                    if (!sliderHitCircle.IsHitAttempted && sliderHitCircle.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed))
                                    {
                                        Debug.Log("Try hit: " + sliderHitCircle.name);

                                        sliderHitCircle.TryHit();
                                        GameObject.FindWithTag("TapCounter").GetComponent<TapTicker>().IncrementButton(1);
                                    }

                                    break;
                                }
                            case SliderPositionRing sliderPositionRing:
                                {
                                    break;
                                }
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


            var iDs = new List<int>();
            foreach (var raycastHit2D in array)
            {

                if (raycastHit2D.transform.GetComponent<IHittable>() != null)
                    iDs.Add(raycastHit2D.transform.GetComponent<IHittable>().HitID);

            }
            if (!iDs.Any())
            {
                type = null;
                return false;
            }

            int min = iDs.Min();
            OnObject = array[iDs.ToList().IndexOf(min)].transform.GetComponent<IObject>();

            if (OnObject.GetType() == typeof(HitCircle))
            {
                type = typeof(HitCircle);
                return true;
            }
            else if (OnObject.GetType() == typeof(HitSlider))
            {
                type = typeof(HitSlider);
                return false;
            }
            else if (OnObject.GetType() == typeof(SliderHitCircle))
            {
                type = typeof(SliderHitCircle);
                return true;
            }



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
