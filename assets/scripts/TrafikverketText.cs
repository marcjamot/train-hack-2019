using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrafikverketText : MonoBehaviour
{
    private TrafikverketClient client = new TrafikverketClient();
    // Start is called before the first frame update
    async void Start()
    {
        var instruction = GetComponent<Text>();
        var trainMessages = await client.GetTrainMessages();

        var message = trainMessages.First();

        instruction.text = message.Header;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
