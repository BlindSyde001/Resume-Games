using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //VARIABLES
    public enum EnemyType { LINE, CIRCULAR};
    public EnemyType _Type;
    public List<Transform> _Anchors;
    public float moveSpeed;
    private bool _lock = true;
    
    public int x = 0;
    public int xLimit;
    public List<Transform> _AddedAnchors;

    // UPDATES
    private void Start()
    {
        Destroy(this.gameObject, 10f);
        GameObject a = GameObject.Find("ANCHORS");
        for(int i = 0; i < a.transform.childCount; i++)
        {
            _Anchors.Add(a.transform.GetChild(i));
        }
        StartCoroutine(LoadUp());
    }
    private void Update()
    {
        if (_lock)
            return;
        if (_Type == EnemyType.LINE)
        {
            MovingInLine();
        } 
        else if(_Type == EnemyType.CIRCULAR)
        {
            MovingInCircularMotion();
        }
    }
    //METHODS
    private IEnumerator LoadUp()
    {
        yield return new WaitForSeconds(0.5f);
        _Type = (EnemyType)Random.Range(0, 2);

        if (_Type == EnemyType.LINE)
        {
            // Set how fast target moves, how many anchors then picks random anchors
            moveSpeed = Random.Range(0.01f, 0.05f);
            xLimit = Random.Range(1, _Anchors.Count);

            for (int i = 0; i < xLimit; i++)
            {
                _AddedAnchors.Add(_Anchors[Random.Range(0, _Anchors.Count)]);
            }
            print(_AddedAnchors.Count + " " + xLimit + " " + x + " " + _AddedAnchors[x].transform.name);
        }
        if (_Type == EnemyType.CIRCULAR)
        {
            _AddedAnchors.Add(_Anchors[0]);
            xLimit = Random.Range(1, _Anchors.Count);
            moveSpeed = Random.Range(0.1f, 0.5f);
            // Sets distance apart from an anchor so it does circular motion
            transform.position = new Vector3(_AddedAnchors[0].position.x,
                                             _AddedAnchors[0].position.y + (Random.Range(-10, 11)),
                                             _AddedAnchors[0].position.z);
            // Limits so that it doesn't go through the floor or above the wall height, respectively
            if (transform.position.y < 1)
                transform.position = new Vector3(transform.position.x,
                                                 1,
                                                 transform.position.z);
            if (transform.position.y > 20)
                transform.position = new Vector3(transform.position.x,
                                                 20,
                                                 transform.position.z);
        }
        _lock = false;
    }
    private void MovingInLine()
    {
        // Straight line movement between anchor points
        transform.position = Vector3.Lerp(transform.position, _AddedAnchors[x].position, moveSpeed);

        // Switch to other Anchor point when you reach one
        if(Vector3.Distance(transform.position, _AddedAnchors[x].position) <= 0.5f)
        {
            x++;
            // to reset and go in a loop
            if (x >= xLimit)
            {
                x = 0;
            }
        }
    }
    private void MovingInCircularMotion()
    {
        // Move in a circle between anchor points
        transform.RotateAround(_AddedAnchors[0].position, Vector3.forward, moveSpeed);
    }

    public void HasBeenShot()
    {
        Destroy(this.gameObject);
        FindObjectOfType<CoverShooterPlayer>().hitCounter ++;
    }
}
