using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class world_generator : MonoBehaviour
{
    const float kTileWidth = 2f;
    Vector3 kPlaneScale = Vector3.one * 0.2f;

    public class Stop {
        public string name = "stop ";
        public Vector3 position;
  }

    List<Stop> stops = new List<Stop>();

    public GameObject tile;

    public int tiles = 1;

    Random r = new Random();

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < tiles; ++i) {
            var stop = new Stop();
            stop.name += i;
            stop.position = new Vector3(0, 0, i * kTileWidth);
            stops.Add(stop);
        }
        foreach (var stop in stops) {
            var clone = Object.Instantiate(tile, stop.position, tile.transform.rotation);
            clone.transform.SetParent(transform);
            var renderer = clone.GetComponent<MeshRenderer>();
            renderer.material.color = Random.ColorHSV();
        }
        // Right plane
        var rightPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        rightPlane.transform.position = new Vector3(kTileWidth * (tiles + 1) / 2, 0, kTileWidth * (tiles - 1f) / 2);
        rightPlane.transform.localScale = tiles * kPlaneScale;
        rightPlane.transform.SetParent(transform);
        // Left plane
        var leftPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        leftPlane.transform.position = new Vector3(kTileWidth * (-tiles - 1) / 2, 0, kTileWidth * (tiles - 1f) / 2);
        leftPlane.transform.localScale = tiles * kPlaneScale;
        leftPlane.transform.SetParent(transform);
        // ...and beyond
        var beyondPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        beyondPlane.transform.position = new Vector3(0, 0, 2 * kTileWidth * tiles);
        beyondPlane.transform.localScale = 2 * (tiles + 0.5f) * kPlaneScale;
        beyondPlane.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
