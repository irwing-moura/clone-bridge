using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float followSpeed = 2;
    public float offsetPlayer = 3.5f;
    // Start is called before the first frame update

    void Start()
    {
        GetComponent<Camera>().orthographicSize = (4.5f * Screen.height) / Screen.width;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Max(0, GameManager.Instance.Player.transform.position.x + offsetPlayer), transform.position.y, transform.position.z), followSpeed * Time.deltaTime);
    }
}
