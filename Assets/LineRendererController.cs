using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererController : MonoBehaviour
{
    public GameObject linePrefab;

    public void DrawLineBetweenTransforms(RectTransform startTransform, RectTransform endTransform)
    {
        GameObject lineGO = Instantiate(linePrefab, transform);
        lineGO.transform.SetParent(transform, false);
        Image lineImage = lineGO.GetComponent<Image>();

        Vector3 startPos = startTransform.position;
        Vector3 endPos = endTransform.position;

        // Calculate position and size of the line
        Vector3 midpoint = (startPos + endPos) / 2f;
        lineGO.transform.position = midpoint;

        float distance = Vector3.Distance(startPos, endPos);
        lineImage.rectTransform.sizeDelta = new Vector2(distance, lineImage.rectTransform.sizeDelta.y);

        // Calculate rotation angle
        Vector3 direction = endPos - startPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineGO.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


}
