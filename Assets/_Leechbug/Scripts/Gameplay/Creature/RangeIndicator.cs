using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    private const int NUM_CAP_VERTICES = 6;

    [Tooltip("The radius of the circle perimeter")]
    public float radius = 1f;

    [Tooltip("The width of the line for the circle perimeter")]
    public float lineWidth = 0.05f;

    private LineRenderer _lineRenderer;
    private bool IsAnimating = false;
    private bool IsDecrRange = false;
    private float _targetSize;
    private float _initSize;
    private float _totalSizeChange;
    private float _duration = 0.5f; 
    //particle effect duration is around 0.2s, but the range indicator moves so fast at that speed that it's not really clear what's happening
    //so I just hardcoded 0.5 for now

    void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.numCapVertices = NUM_CAP_VERTICES;
        SetRadius(radius);
    }

    private void Update() {
        //While testing in-editor, refresh the circle each frame so we can test the circle by changing the fields in the inspector.
        if (Application.isEditor) SetRadius(radius);
		if (IsAnimating) {
            Vector3 inc = Vector3.Lerp(Vector3.zero, new Vector3(_totalSizeChange, _totalSizeChange, _totalSizeChange), Time.deltaTime/_duration);
            if (IsDecrRange) {
                //indicator is going from large scale to small
                if (transform.localScale.x > _targetSize) {
                    transform.localScale -= inc;
                } else {
                    Destroy(_lineRenderer.gameObject);
                }
            } else {
                if (transform.localScale.x < _targetSize) {
                    transform.localScale += inc;
                } else {
                    Destroy(_lineRenderer.gameObject);
                }
            }
        }
	}

    //Call this method from other scripts to adjust the radius at runtime
    public void SetRadius(float pRadius)
    {
        radius = pRadius;

        if (radius <= 0.1f)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        float tickness = lineWidth;
        _lineRenderer.startWidth = tickness;
        _lineRenderer.endWidth = tickness;

        //Calculate the number of vertices needed depending on radius so it always looks round.
        //For instance, huge circles need proportionaly less vertices than smaller ones to look good.
        //Source : http://stackoverflow.com/questions/11774038/how-to-render-a-circle-with-as-few-vertices-as-possible
        float e = 0.01f; //constant ratio to adjust, reduce this value for more vertices
        float th = Mathf.Acos(2 * Mathf.Pow(1 - e / radius, 2) - 1); //this is in radian
        int numberOfVertices = Mathf.CeilToInt(2 * Mathf.PI / th);

        _lineRenderer.positionCount = numberOfVertices + 1;
        for (int i = 0; i < numberOfVertices + 1; i++)
        {
            float angle = (360f / (float)numberOfVertices) * (float)i;
            _lineRenderer.SetPosition(i, radius * new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), 0, Mathf.Sin(Mathf.Deg2Rad * angle)));
        }
    }

    public void SetCircleAnimating(float startSize, float endSize) {
        transform.localScale = new Vector3(startSize, startSize, startSize);
        _targetSize = endSize;
        _initSize = startSize;
        _totalSizeChange = Mathf.Abs(_targetSize - startSize);
        IsAnimating = true;
        if(startSize > endSize) {
            //means we're descreasing the indicator
            IsDecrRange = true;
        }
    }

    //Call this method from other scripts to adjust the width of the line at runtime
    public void SetWidth(float pWidth)
    {
        lineWidth = pWidth;
        SetRadius(radius);
    }  
}
