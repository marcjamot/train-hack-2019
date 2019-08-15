using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class TrafikverketText : MonoBehaviour
{
    private TrafikverketClient client = new TrafikverketClient();
    // Start is called before the first frame update
    async void Start()
    {
        var instruction = GetComponent<Text>();
        var tripInformations = await client.GetTripInformation("10420");

        var message = JsonConvert.SerializeObject(tripInformations);


        instruction.text = message;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
