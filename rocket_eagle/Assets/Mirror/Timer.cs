using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;//to get math funcions

[RequireComponent(typeof(Text))]
public class Timer : MonoBehaviour
{

    Text time;
    public float timeElapsed = 0.0f;
    GameObject player;
    BirdController bird;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        time = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("LocalBird");
        bird = player.GetComponent<BirdController>();
        if (!bird.finished)
        {
            timeElapsed += Time.deltaTime;
            time.text = "T i m e : " + timeElapsed.ToString("F2");
        }
        else
        {
            Debug.Log("Time it took to for match:" + timeElapsed);
            float logVal = Mathf.Log(timeElapsed);

            //so C# is studip and doesn't handle a conversion from double to uint
            //so I have to go from double to int to uint
            double bc = Math.Pow(1.0f / 2.0f, logVal) * 400;

            int bcValue = (int)bc;

            uint bcValueActual = (uint)bcValue;

            //uint bc = 100;
            //now that we have the amount of birdcoin the user has earned, update the file that stores the amount
            SaveGameData.UpdatePlayerCoin(bcValueActual);
        }
    }
}
