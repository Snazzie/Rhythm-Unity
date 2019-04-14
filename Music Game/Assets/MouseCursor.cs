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
        private IHittable currentTarget { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            OnObject = null;
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
                if (!(tapTapAimSetup.Tracker.NextObjToHit < tapTapAimSetup.HitObjectQueue.Count()))
                    return;
                var nextObj = ((MonoBehaviour)tapTapAimSetup.HitObjectQueue[tapTapAimSetup.Tracker.NextObjToHit]).transform;
                //Debug.Log("Next object to hit: " + ((IHittable)tapTapAimSetup.HitObjectQueue[tapTapAimSetup.Tracker.NextObjToHit]).HitID);
                if (IsAutoPlay)
                {
                    var objTarget = nextObj.GetComponent<IHittable>();
                    if (currentTarget != objTarget)
                    {
                        Debug.Log($"Cursor target = HitId:{objTarget.HitID}");
                        currentTarget = objTarget;
                    }

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
                if (IsHittable(hits, out var type, out var hittable))
                {
                    if (OnObject != hittable)
                    {
                        Debug.Log($"ontop of {hittable.HitID} {((MonoBehaviour)hittable).name}");
                        OnObject = hittable;
                    }

                    if (IsAutoPlay)
                    {

                        switch (hittable)
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

        private bool IsHittable(RaycastHit2D[] array, out Type type, out IHittable hittableObject)
        {
            var hittables = new List<IHittable>();
            foreach (var raycastHit2D in array)
            {

                if (raycastHit2D.transform.GetComponent<IHittable>() != null)
                    hittables.Add(raycastHit2D.transform.GetComponent<IHittable>());

            }
            if (!hittables.Any())
            {
                type = null;
                hittableObject = null;
                return false;
            }


            int minId = hittables.Select(h => h.HitID).ToList().Min();
            hittableObject = hittables.Single(h => h.HitID == minId);

            if (hittableObject.GetType() == typeof(HitCircle))
            {
                type = typeof(HitCircle);
                return true;
            }
            else if (hittableObject.GetType() == typeof(SliderHitCircle))
            {
                type = typeof(SliderHitCircle);
                return true;
            }
            else if (hittableObject.GetType() == typeof(HitSlider))
            {
                throw new Exception($"HitSlider recognised as IHittable. This is wrong!");
            }
            else if (hittableObject.GetType() == typeof(SliderPositionRing))
            {
                throw new Exception($"HitSlider recognised as IHittable. This is wrong!");
            }



            type = null;
            hittableObject = null;
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
