using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasLanded = false;
    private MainSceneHandler _handler;
    public int rollResult;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();   
    }

    public void Roll(MainSceneHandler handler)
    {
        _handler = handler;
        hasLanded = false;
        transform.rotation = Random.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddTorque(Random.onUnitSphere * Random.Range(500, 1000));
    }

    void Update()
    {
        if (!hasLanded && rb.IsSleeping())
        {
            hasLanded = true;
            int value = GetDiceValue();
            rollResult = value;
            _handler.RollFinished(this);
        }
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
