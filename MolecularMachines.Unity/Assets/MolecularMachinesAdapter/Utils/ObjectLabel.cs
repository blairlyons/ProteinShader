using UnityEngine;
using System.Collections;

public class ObjectLabel : MonoBehaviour
{
    private GUIStyle style = new GUIStyle();

    public bool considerGlobalScale = true;

    // Use this for initialization
    void Start()
    {
        style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        var cam = Camera.current;
        if (cam == null) { cam = Camera.main; }

        var position = gameObject.transform.position;

        if (considerGlobalScale)
        {
            position = position * PersistantSettings.Instance.Scale;
        }

        var p = cam.WorldToScreenPoint(position);

        float w = 70;
        float h = 25;

        float x = p.x;
        float y = cam.pixelHeight - p.y;

        GUI.Box(new Rect(x - w / 2f, y - h / 2f, w, h), gameObject.name);
    }
}
