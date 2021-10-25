using UnityEngine;

public class CannonManager : MonoBehaviour
{
    public GameObject cannonBallPrefab;
    public Transform firePoint;
    public LineRenderer lineRenderer;

    private const int N_TRAJECTORY_POINTS = 10;

    private Camera _cam;
    private bool _pressingMouse = false;

    private Vector3 _initialVelocity;

    void Start()
    {
        _cam = Camera.main;

        lineRenderer.positionCount = N_TRAJECTORY_POINTS;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _pressingMouse = true;
            lineRenderer.enabled = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _pressingMouse = false;
            lineRenderer.enabled = false;
            _Fire();
        }

        if (_pressingMouse)
        {
            // coordinate transform screen > world
            Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // look at
            transform.LookAt(mousePos);

            _initialVelocity = mousePos - firePoint.position;

            _UpdateLineRenderer();
        }
    }

    private void _Fire()
    {
        // instantiate a cannon ball
        GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, Quaternion.identity);
        // apply some force
        Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
        rb.AddForce(_initialVelocity, ForceMode.Impulse);
    }

    private void _UpdateLineRenderer()
    {
        float g = Physics.gravity.magnitude;
        float velocity = _initialVelocity.magnitude;
        float angle = Mathf.Atan2(_initialVelocity.y, _initialVelocity.x);

        Vector3 start = firePoint.position;

        float timeStep = 0.1f;
        float fTime = 0f;
        for (int i = 0; i < N_TRAJECTORY_POINTS; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle);
            float dy = velocity * fTime * Mathf.Sin(angle) - (g * fTime * fTime / 2f);
            Vector3 pos = new Vector3(start.x + dx, start.y + dy, 0);
            lineRenderer.SetPosition(i, pos);
            fTime += timeStep;
        }
    }
}
