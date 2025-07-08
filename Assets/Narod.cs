using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narod : MonoBehaviour
{

    [Header("Generalne varijable")]

    [SerializeField] float kreditMin = 1;
    [SerializeField] float kreditMax = 100;

    public int kreditMjeseciOtplate = 10;

    [SerializeField] float kreditCiklusVrijeme = 0.4f;

    [SerializeField] int brojDiza�aKredita = 3;
    [SerializeField] int brojPoku�ajaDizanjaKredita = 3;

    [SerializeField] int maxBrojKredita = 100;
    int currBrojKredita = 0;

    [SerializeField] Transform roditeljBanaka;


    public float prosjecnaPlaca = 1;




    private void Start()
    {
        StartCoroutine(DigniKredit());
    }


    bool dizanje = false;
    IEnumerator DigniKredit()
    {

        if (!dizanje)
        {
            dizanje = true;

            for (int i = 0; i < brojDiza�aKredita; i++)
            {

                int rand = Random.Range(0, roditeljBanaka.childCount);

                for (int y = 0; y < brojPoku�ajaDizanjaKredita && maxBrojKredita > currBrojKredita; y++)
                {

                    float kredit = Random.Range(kreditMin, kreditMax);


                    if (roditeljBanaka.GetChild((y + rand) %
                        roditeljBanaka.childCount).GetComponent<BankAgent>())
                    {
                        // gledamo da li je odobren kredit
                        if(roditeljBanaka.GetChild((y + rand) % roditeljBanaka.childCount
                        ).GetComponent<BankAgent>().DigniKredit(kredit))
                        {
                            currBrojKredita++;
                            y = brojPoku�ajaDizanjaKredita;
                        }
                        
                    }
                    else if (roditeljBanaka.GetChild((y + rand) %
                            roditeljBanaka.childCount).GetComponent<Igra�Banka>())
                    {
                        
                        if(roditeljBanaka.GetChild((y + rand) % roditeljBanaka.childCount
                        ).GetComponent<Igra�Banka>().DigniKredit(kredit))
                        {
                            currBrojKredita++;
                            y = brojPoku�ajaDizanjaKredita;
                        }
                        
                    }

                }
                
            }


            yield return new WaitForSeconds(kreditCiklusVrijeme);


            dizanje = false;
            StartCoroutine(DigniKredit());
        }

    }



    public void KreditManje()
    {
        currBrojKredita--;
    }




}
