using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

using UnityEngine;

public class UITimePlayCountDown : MonoBehaviour
{
    public int countdownTime;
    public TextMeshProUGUI countdownDisplay;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (countdownTime>0){
            countdownDisplay.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }    
    

    countdownDisplay.text = "GO!";

    yield return new WaitForSeconds(1f);
    countdownDisplay.gameObject.SetActive(false);
    }

}
