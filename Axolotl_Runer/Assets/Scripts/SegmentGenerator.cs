using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] Segment;
    [SerializeField] int zPos = 0;
    [SerializeField] int segmentNum;
    [SerializeField] bool creatingSegment = false;

    public bool isGameOver = false;
    void Update()
    {
        if (isGameOver) return;

        if (creatingSegment == false)
        {
            creatingSegment = true;
            StartCoroutine(SegmentGen());
        }
    }

    IEnumerator SegmentGen()
    {
        segmentNum = Random.Range(0, 2);
        Instantiate(Segment[segmentNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 20;
        yield return new WaitForSeconds(5);
        creatingSegment = false;
    }
}
