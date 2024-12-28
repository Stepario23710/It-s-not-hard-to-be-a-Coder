using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlatformDollyCart : MonoBehaviour
{
    private GameObject trackParentObj;
    private LineRenderer line;
    private CinemachinePathBase path;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        path = GetComponent<CinemachineDollyCart>().m_Path;
        trackParentObj = path.transform.parent.gameObject;
        transform.rotation = Quaternion.identity;
        DrawTrajectory();
    }
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
    private void DrawTrajectory(){
        path.transform.parent = null;
        Vector3[] points = new Vector3[1000];
        for (int i = 0; i < 1000; i++){
            float posAtTrack = (path.PathLength/1000f) * (i + 1);
            Vector3 newPoint = path.EvaluateLocalPosition(posAtTrack);
            points[i] = new Vector2(newPoint.x + path.transform.position.x, newPoint.y + path.transform.position.y);
        }
        line.positionCount = points.Length;
        line.SetPositions(points);
        path.transform.parent = trackParentObj.transform;
    }
}
