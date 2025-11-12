using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cutscenescript : MonoBehaviour
{

    float timebycutscene;

    public List<GameObject> Images;

    private int currentimageindex;

    public float timeperimage;

    private int imagetimecounter;

    private bool fadingtoblack;

    private int buttonlagcnt;

    private void Start()
    {
        ActivateCorrectImage();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(buttonlagcnt>0)
        {
            buttonlagcnt--;
        }

        if(currentimageindex< Images.Count)
        {
            if (Images[currentimageindex].transform.localPosition.x > -90)
            {
                Images[currentimageindex].transform.localPosition -= new Vector3(10f * Time.fixedDeltaTime, 0f, 0f);
            }
            else
            {
                Images[currentimageindex].transform.localPosition = new Vector3(-90, 0f, 0f);
            }
        }

        if(imagetimecounter > 0)
        {
            imagetimecounter--;
        }
        else if(fadingtoblack)
        {
            Color newcolor = Images[currentimageindex].GetComponent<Image>().color;
            Color colortoapply = new Color(newcolor.r - Time.fixedDeltaTime, newcolor.g - Time.fixedDeltaTime, newcolor.b - Time.fixedDeltaTime);
            Images[currentimageindex].GetComponent<Image>().color = colortoapply;
            if(colortoapply.r<=0f)
            {
                if(currentimageindex== Images.Count-1)
                {
                    SceneManager.LoadScene("MainScene");
                }
                else
                {
                    PressButton();
                    fadingtoblack = false;
                }
                    
            }
        }
        else
        {
            Color newcolor = Images[currentimageindex].GetComponent<Image>().color;
            Color colortoapply = new Color(newcolor.r + Time.fixedDeltaTime, newcolor.g + Time.fixedDeltaTime, newcolor.b + Time.fixedDeltaTime);
            Images[currentimageindex].GetComponent<Image>().color = colortoapply;
            if (colortoapply.r >= 1f)
            {
                imagetimecounter = (int)(timeperimage/Time.fixedDeltaTime);
                fadingtoblack = true;
            }
        }
        
    }

    public void PressButton()
    {

        if(buttonlagcnt==0)
        {
            buttonlagcnt = (int)(0.5f / Time.fixedDeltaTime);

            currentimageindex++;
            if (currentimageindex >= Images.Count)
            {
                SceneManager.LoadScene("MainScene");
            }
            ActivateCorrectImage();
            fadingtoblack = false;
        }
        
    }

    private void ActivateCorrectImage()
    {
        foreach (GameObject Go in Images)
        {
            if(Images.IndexOf(Go)==currentimageindex)
            {
                if(!Go.activeSelf)
                {
                    Go.SetActive(true);
                }
            }
            else
            {
                if (Go.activeSelf)
                {
                    Go.SetActive(false);
                }
            }
        }
    }

}
