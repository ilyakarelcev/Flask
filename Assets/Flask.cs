using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Flask : MonoBehaviour
{

    public List<int> ColorIndexes;
    [SerializeField] private FlaskSettings _flaskSettings;

    [SerializeField] private Material _material;

    [SerializeField] private Material _materialClone;
    [SerializeField] private Renderer _renderer;

    public Transform _pourPoint;

    [SerializeField] private float[] _angles;

    [SerializeField] private Transform _leftCorner;
    [SerializeField] private Transform _rightCorner;

    private void OnValidate()
    {
        UpdateColors();
        SetFill();
    }


    private void UpdateColors()
    {

        if (_materialClone == null)
        {
            _materialClone = Instantiate(_material);
            _renderer.material = _materialClone;
        }

        for (int i = 0; i < 4; i++)
        {
            if (ColorIndexes.Count > i)
            {
                _materialClone.SetColor("_Color" + i, _flaskSettings.Colors[ColorIndexes[i]]);
            }
            else
            {
                if (ColorIndexes.Count > 0)
                {
                    _materialClone.SetColor("_Color" + i, _flaskSettings.Colors[ColorIndexes[ColorIndexes.Count - 1]]);
                }
            }
        }

    }

    private void SetFill()
    {
        _materialClone.SetFloat("_Fill", ColorIndexes.Count / 4f);
    }

    private void SetFill(float value) // 0 - 1 // 0 - 4
    {
        _materialClone.SetFloat("_Fill", value / 4f);
    }

    public void Pour(Flask other, int number)
    {
        StartCoroutine(PourProcess(other, number));
    }

    private IEnumerator PourProcess(Flask other, int number)
    {
        Transform corner;
        float rotationSign;
        if (transform.position.x > other.transform.position.x)
        {
            corner = _leftCorner;
            rotationSign = 1;
        }
        else
        {
            corner = _rightCorner;
            rotationSign = -1;
        }

        Vector3 startPosition = corner.position;

        int colorIndex = ColorIndexes[ColorIndexes.Count - 1];

        int startNumberOfSegments = ColorIndexes.Count;
        int endNumberOfSegments = startNumberOfSegments - number;

        int startNumberOfSegmentsOther = other.ColorIndexes.Count;
        int endNumberOfSegmentsOther = other.ColorIndexes.Count + number;


        for (int i = 0; i < number; i++)
        {
            ColorIndexes.RemoveAt(ColorIndexes.Count - 1);
            other.ColorIndexes.Add(colorIndex);
        }
        other.UpdateColors();
        
        float startAngle = _angles[startNumberOfSegments];
        float endAngle = _angles[endNumberOfSegments];

        for (float t = 0; t < 1f; t += Time.deltaTime / 0.5f)
        {

            float interpolant = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

            Vector3 position = Vector3.Lerp(startPosition, other._pourPoint.position, interpolant);
            corner.position = position;

            float angle = Mathf.Lerp(0, startAngle, interpolant);
            corner.localEulerAngles = new Vector3(0, 0, angle * rotationSign);
            yield return null;
        }
        corner.position = other._pourPoint.position;
        corner.localEulerAngles = new Vector3(0, 0, startAngle * rotationSign);

        for (float t = 0; t < 1f; t += Time.deltaTime / 0.5f)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            corner.localEulerAngles = new Vector3(0, 0, angle * rotationSign);

            float fill = Mathf.Lerp((float)startNumberOfSegments, endNumberOfSegments, t);
            SetFill(fill);

            float otherFill = Mathf.Lerp((float)startNumberOfSegmentsOther, endNumberOfSegmentsOther, t);
            other.SetFill(otherFill);

            yield return null;
        }
        corner.localEulerAngles = new Vector3(0, 0, endAngle * rotationSign);
        SetFill(endNumberOfSegments);
        other.SetFill(endNumberOfSegmentsOther);

        UpdateColors();

        for (float t = 0; t < 1f; t += Time.deltaTime / 0.5f)
        {

            float interpolant = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

            Vector3 position = Vector3.Lerp(other._pourPoint.position, startPosition, interpolant);
            corner.position = position;

            float angle = Mathf.Lerp(endAngle, 0, interpolant);
            corner.localEulerAngles = new Vector3(0, 0, angle * rotationSign);
            yield return null;
        }
        corner.position = startPosition;
        corner.localEulerAngles = new Vector3(0, 0, 0);

    }





}
