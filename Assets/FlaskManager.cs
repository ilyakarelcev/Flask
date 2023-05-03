using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaskManager : MonoBehaviour
{

    [SerializeField] private Flask _currentFlask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<Flask>() is Flask flask)
                {
                    if (_currentFlask == null)
                    {
                        _currentFlask = flask;
                    }
                    else
                    {
                        Merge(_currentFlask, flask);
                    }
                }
            }
            else
            {
                _currentFlask = null;
            }
        }
    }


    private void Merge(Flask flaskFrom, Flask flaskTo) {

        if (flaskFrom.ColorIndexes.Count == 0) return;
        if (flaskTo.ColorIndexes.Count >= 4) return;
        if (flaskFrom == flaskTo) return;

        int topIndex = flaskFrom.ColorIndexes[flaskFrom.ColorIndexes.Count - 1];

        if (flaskTo.ColorIndexes.Count == 0 || topIndex == flaskTo.ColorIndexes[flaskTo.ColorIndexes.Count - 1]) {
            int sameColorSegments = 0;
            for (int i = flaskFrom.ColorIndexes.Count - 1; i >= 0; i--)
            {
                if (flaskFrom.ColorIndexes[i] == topIndex) {
                    sameColorSegments++;
                }
            }
            int emptySegments = 4 - flaskTo.ColorIndexes.Count;

            int numberToSwitch = Mathf.Min(sameColorSegments, emptySegments);

            flaskFrom.Pour(flaskTo, numberToSwitch);

            //Debug.Log(numberToSwitch);
        }


    }



}
