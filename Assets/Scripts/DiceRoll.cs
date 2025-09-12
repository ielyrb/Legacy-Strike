using System.Collections;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasLanded = false;
    private Vector3 initPosition;
    private bool _initiated = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;        
    }

    public void RollDice()
    {
        hasLanded = false;
        transform.position = initPosition + new Vector3(0, 2, 0); // reset above ground
        transform.rotation = Random.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(Vector3.up * Random.Range(100, 300));
        rb.AddTorque(Random.onUnitSphere * Random.Range(500, 1000));
    }

    void Update()
    {
        if (!hasLanded && rb.IsSleeping())
        {
            if (!_initiated)
                return;

            hasLanded = true;
            int value = GetDiceValue();
            GameSceneHandler.OnDiceRolled?.Invoke(value);
            StartCoroutine(ResetPosition());
        }
    }

    private void OnMouseDown()
    {
        if (!hasLanded && _initiated)
            return;

        rb.useGravity = true;
        _initiated = true;

        RollDice();
    }

    IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1f);
        transform.position = initPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    int GetDiceValue()
    {
        Vector3[] directions = {
        transform.up,
        -transform.up,
        transform.forward,
        -transform.forward,
        -transform.right,
        transform.right
    };

        int[] faceValues = {
        1,
        6,
        3,
        4,
        2,
        5
    };

        float maxDot = -Mathf.Infinity;
        int value = -1;

        for (int i = 0; i < directions.Length; i++)
        {
            float dot = Vector3.Dot(directions[i], Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                value = faceValues[i];
            }
        }

        return value;
    }

}
