using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int cardNumber;
    public int cardColor;
    public GameObject cardObject;

    /*  1- rot
    2- blau
    3- gruen
    4- gelb
    5- special*/
    public Card(int number, int color){
        this.cardNumber = number;
        this.cardColor = color;
        this.cardObject = null;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
