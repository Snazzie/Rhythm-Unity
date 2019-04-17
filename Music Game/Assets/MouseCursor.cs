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
        float t;
        Vector3 startPosition;
        Vector3 target;
        double timeToReachTarget;
        private TapTapAimSetup tapTapAimSetup;
        private IInteractable OnObject { get; set; }
        private bool IsUserControl { get; set; }
        public bool IsGame { get; set; }
        private float Radius { get; set; }
        private IInteractable currentTarget { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            OnObject = null;
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
                if (!(tapTapAimSetup.Tracker.NextObjToHit < tapTapAimSetup.ObjectInteractQueue.Count()))
                    return;
                var nextObj = ((MonoBehaviour)tapTapAimSetup.ObjectInteractQueue[tapTapAimSetup.Tracker.NextObjToHit]).transform;
                //Debug.Log("Next object to hit: " + ((IHittable)tapTapAimSetup.ObjectInteractQueue[tapTapAimSetup.Tracker.NextObjToHit]).InteractionID);
                if (tapTapAimSetup.isAutoPlay)
                {
                    var objTarget = nextObj.GetComponent<IInteractable>();
                    if (currentTarget != objTarget)
                    {
                        Debug.Log($"Cursor target = HitId:{objTarget.InteractionID}");
                        currentTarget = objTarget;
                    }

                    //pos.y += 0.1f;
                    SetDestination(nextObj.position, (nextObj.GetComponent<IInteractable>().PerfectInteractionTime - tapTapAimSetup.Tracker.Stopwatch.Elapsed).TotalSeconds * Speed);
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
                if (IsInteractable(hits, out var interactable))
                {
                    if (OnObject != interactable)
                    {
                        Debug.Log($"ontop of {interactable.InteractionID} {((MonoBehaviour)interactable).name}");
                        OnObject = interactable;
                    }

                    if (tapTapAimSetup.isAutoPlay)
                    {

                        switch (interactable)
                        {
                            case HitCircle hitCircle:
                                if (!hitCircle.IsHitAttempted && hitCircle.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed))
                                {
                                    Debug.Log("Try hit: " + hitCircle.name);

                                    hitCircle.TryInteract();
                                    GameObject.FindWithTag("TapCounter").GetComponent<TapTicker>().IncrementButton(1);
                                }
                                break;

                            case SliderHitCircle sliderHitCircle:
                                {
                                    if (!sliderHitCircle.IsHitAttempted && sliderHitCircle.IsInAutoPlayHitBound(tapTapAimSetup.Tracker.Stopwatch.Elapsed))
                                    {
                                        Debug.Log("Try hit: " + sliderHitCircle.name);

                                        sliderHitCircle.TryInteract();
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

        private bool IsInteractable(RaycastHit2D[] array, out IInteractable interactableObject)
        {
            var interactables = new List<IInteractable>();
            foreach (var raycastHit2D in array)
            {


                if (raycastHit2D.transform.GetComponent<IHitCircle>() != null)
                    interactables.Add(raycastHit2D.transform.GetComponent<IHitCircle>());
                else if( tapTapAimSetup.interactWithSliderPositionRing && raycastHit2D.transform.GetComponent<ISliderPositionRing>() != null)
                    interactables.Add(raycastHit2D.transform.GetComponent<ISliderPositionRing>());

            }
            if (!interactables.Any())
            {
  
                interactableObject = null;
                return false;
            }


            int minId = interactables.Select(h => h.InteractionID).ToList().Min();
            interactableObject = interactables.Single(h => h.InteractionID == minId);

            if (interactableObject.GetType() == typeof(HitCircle))
            {
                return true;
            }
            else if (interactableObject.GetType() == typeof(SliderHitCircle))
            {
                return true;
            }
            else if (interactableObject.GetType() == typeof(SliderPositionRing))
            {
                throw new Exception($"HitSlider recognised as IHittable. This is wrong!");
            }



            interactableObject = null;
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
