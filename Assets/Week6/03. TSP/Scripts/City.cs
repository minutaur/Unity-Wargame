using System;
using UnityEngine;

namespace Week6.TSP
{
    [RequireComponent(typeof(LineRenderer))]
    public class City : MonoBehaviour
    {
        private LineRenderer _line;
        private City otherCity;
        private void Start()
        {
            _line = GetComponent<LineRenderer>();
        }

        public float GetDistance(City other)
        {
            return Vector3.Distance(transform.position, other.transform.position);
        }

        public void ShowLinePath(City other)
        {
            otherCity = other;
            Invoke("Show", .01f);
        }

        public void Show()
        {
            _line.SetPosition(0, transform.position);
            _line.SetPosition(1, otherCity.transform.position);
        }
    }
}