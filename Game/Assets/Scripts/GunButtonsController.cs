using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunButtonsController : MonoBehaviour
{
    public Button blueButton;
    public Image blueImage;
    public Text blueText;

    public Button greenButton;
    public Image greenImage;
    public Text greenText;

    public Button redButton;
    public Image redImage;
    public Text redText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setBlueButton(int level)
    {
        blueButton.interactable = level > 0;
        blueImage.gameObject.SetActive(level > 0);
        blueText.text = numToRomanNumeral(level);
    }

    public void setGreenButton(int level)
    {
        greenButton.interactable = level > 0;
        greenImage.gameObject.SetActive(level > 0);
        greenText.text = numToRomanNumeral(level);
    }

    public void setRedButton(int level)
    {
        redButton.interactable = level > 0;
        redImage.gameObject.SetActive(level > 0);
        redText.text = numToRomanNumeral(level);
    }

    private string numToRomanNumeral(int num)
    {
        if(num == 1)
        {
            return "I";
        }else if (num == 2)
        {
            return "II";
        }
        else if (num == 3)
        {
            return "III";
        }
        else if (num == 4)
        {
            return "IV";
        }
        else if (num == 5)
        {
            return "V";
        }
        else if (num == 6)
        {
            return "VI";
        }
        else if (num == 7)
        {
            return "VII";
        }
        else if (num == 8)
        {
            return "VIII";
        }
        else if (num == 9)
        {
            return "IX";
        }
        else if (num == 10)
        {
            return "X";
        }
        else if (num == 11)
        {
            return "XI";
        }
        else if (num == 12)
        {
            return "XII";
        }
        return "";
    }
}
