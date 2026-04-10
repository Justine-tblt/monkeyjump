using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public GameObject Template;
    public GameObject TemplateEmpty;
    public GameObject SpawnTo;

    [Header("Difficulty")]
    public float BaseSpeed = 5f;
    public float SpeedIncreasePerUnit = 0.03f;
    public float MaxSpeed = 14f;

    private float DistanceTravelled = 0;
    private float StartZ;

    private void Start()
    {
        StartZ = transform.position.z;

        GameObject Spawned = Instantiate(TemplateEmpty, SpawnTo.transform);
        Spawned.transform.parent = transform;
        SpawnTo.transform.position += new Vector3(0, 0, -20);
        GameObject Spawned1 = Instantiate(TemplateEmpty, SpawnTo.transform);
        Spawned1.transform.parent = transform;
        SpawnTo.transform.position += new Vector3(0, 0, -20);
        GameObject Spawned2 = Instantiate(TemplateEmpty, SpawnTo.transform);
        Spawned2.transform.parent = transform;
    }

    public void Update()
    {
        float progression = Mathf.Max(0f, transform.position.z - StartZ);
        float currentSpeed = Mathf.Min(MaxSpeed, BaseSpeed + progression * SpeedIncreasePerUnit);
        transform.position += new Vector3(0, 0, currentSpeed * Time.deltaTime);

        if(transform.position.z - DistanceTravelled >= 20)
        {
            DistanceTravelled = transform.position.z;
            GameObject Spawned = Instantiate(Template, SpawnTo.transform);
            Spawned.transform.parent = transform;
        }
    }
}
