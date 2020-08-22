using System;
using System.Collections;
using UnityEngine;

namespace SweetSugar.Scripts.MapScripts
{
    public class WaypointsMover : MonoBehaviour
    {
        private int _nextInd;
        private int _finishInd;
        private Action _finishedAction;

        private SplineCurve _splineCurve;
        private float _splineT;
        private float _partTime;
        private Vector3 _precisePosition;
        private bool _isRunning;
        private bool _isForwardDirection;

        [HideInInspector]
        public Path Path;

        [HideInInspector]
        public float Speed;

        public void Awake()
        {
            if (Path.IsCurved)
            {
                _splineCurve = new SplineCurve();
                UpdateCurvePoints();
            }
        }

        public void Move(Transform from, Transform to, Action finishedAction)
        {
//        if (_isRunning)
//            return;

            _finishedAction = finishedAction;
            _nextInd = Path.Waypoints.IndexOf(from);
            _finishInd = Path.Waypoints.IndexOf(to);
            _isForwardDirection = _finishInd > _nextInd;
            transform.position = from.position;
            _isRunning = true;
            StartCoroutine(Anim(from, to));
            // TakeNextWaypoint();
        }

        IEnumerator Anim(Transform from, Transform to)
        {
            float startTime = Time.time;
            float counter = 0;
            while (startTime + 1 > Time.time)
            {
                transform.position = Vector2.Lerp(from.position, to.position, counter);
                counter += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            _isRunning = false;
            _finishedAction?.Invoke();
        }

        // public void Update()
        // {
        //     if (_isRunning)
        //     {
        //         if (Path.IsCurved)
        //             UpdateCurved();
        //         else
        //             UpdateLinear();
        //     }
        // }

        private void TakeNextWaypoint()
        {
            if (_nextInd == _finishInd)
            {
                _isRunning = false;
                _finishedAction();
            }
            else
            {
                _nextInd += _isForwardDirection ? 1 : -1;
            }

            if (Path.IsCurved)
                UpdateCurvePoints();
        }

        #region Linear
        private void UpdateLinear()
        {
            Transform waypoint = Path.Waypoints[_nextInd];
            Vector3 direction = (waypoint.position - transform.position).normalized;
            Vector3 nextPosition = transform.position + direction * Speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, waypoint.position) <=
                Vector3.Distance(transform.position, nextPosition))
            {
                transform.position = waypoint.position;
                TakeNextWaypoint();
            }
            else
            {
                transform.position = nextPosition;
            }
        }

        #endregion

        #region Curved
        private void UpdateCurved()
        {
            _splineT += Time.deltaTime / _partTime;
            if (_splineT > 1.0f)
            {
                _splineT = 0.0f;
                TakeNextWaypoint();
                UpdateCurvePoints();
            }
            else
            {
                Vector2 point = _splineCurve.GetPoint(_splineT);
                transform.position = point;
            }
        }

        private void UpdateCurvePoints()
        {
            int dInd = _isForwardDirection ? 1 : -1;
            int[] indexes = Path.GetSplinePointIndexes((_nextInd - dInd + Path.Waypoints.Count) % Path.Waypoints.Count, _isForwardDirection);
            _splineCurve.ApplyPoints(
                Path.Waypoints[indexes[0]].transform.position,
                Path.Waypoints[indexes[1]].transform.position,
                Path.Waypoints[indexes[2]].transform.position,
                Path.Waypoints[indexes[3]].transform.position);
            _partTime = GetPartPassTime(_nextInd);
        }

        private float GetPartPassTime(int targetInd)
        {
            int dInd = _isForwardDirection ? 1 : 0;
            return Path.GetLength((targetInd - dInd + Path.Waypoints.Count) % Path.Waypoints.Count) / Speed;
        }

        #endregion

    }
}
