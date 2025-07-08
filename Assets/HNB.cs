using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HNB : MonoBehaviour
{

    [Header("Generalne informacije")]

    [SerializeField] public float vrijemeOdluka = 2;

    [SerializeField] float pocetnaKoličinaNovca = 5000000;

    [SerializeField] Transform roditeljBanaka;



    [Header("Interne varijable")]

    public float inflacija = 2.0f;

    public float kamate = 2.5f;

    public int mjeseciZaObveznice = 20;

    Vector3[] hnbPolje;




    private void Start()
    {
        hnbPolje = new Vector3[FindObjectOfType<Visual>().maxHistory];
        for (int i = 0; i < hnbPolje.Length; i++)
        {
            hnbPolje[i] = Vector3.zero;
        }

        StartCoroutine(PregledSituacije());
    }

    private void Update()
    {

        hnbPolje[0] = new Vector3(inflacija, kamate);
    }

    IEnumerator PregledSituacije()
    {

        AzurirajPovijest();
        PregledInflacije();
        PromjenaKamate();


        for (int i = 0; i < roditeljBanaka.childCount; i++)
        {
            yield return new WaitForSeconds(0.01f);
            if (roditeljBanaka.GetChild(i).GetComponent<BankAgent>() && 
                roditeljBanaka.GetChild(i).gameObject.activeInHierarchy)
            {
                roditeljBanaka.GetChild(i).GetComponent<BankAgent>().RačunajSve();
            }
            else if (roditeljBanaka.GetChild(i).GetComponent<IgračBanka>())
            {
                roditeljBanaka.GetChild(i).GetComponent<IgračBanka>().RačunajSve();
            }
        }

        yield return new WaitForSeconds(vrijemeOdluka);

        StartCoroutine(PregledSituacije());
    }




    void PregledInflacije()
    {

        float sviKrediti = 0;

        for (int i = 0; i < roditeljBanaka.childCount; i++)
        {
            if (roditeljBanaka.GetChild(i).GetComponent<BankAgent>())
            {
                sviKrediti += roditeljBanaka.GetChild(i).GetComponent<BankAgent>().SviKrediti();
            }
            else if (roditeljBanaka.GetChild(i).GetComponent<IgračBanka>())
            {
                sviKrediti += roditeljBanaka.GetChild(i).GetComponent<IgračBanka>().SviKrediti();
            }

        }

        

        // inflacija ~ (M2_nova - M2_stara) / M2_stara
        // tj svi krediti + pocetna kol - pocetna kol / pocetna kol
        inflacija = (sviKrediti / pocetnaKoličinaNovca);

    }

    void PromjenaKamate()
    {

        kamate = 2f + inflacija + (inflacija - 2) / 2;

    }
    /* 
     i = r* + π + 0.5(π - π*) + 0.5(y - y*)
Gdje je:

i – nominalna kamatna stopa (koju postavlja centralna banka)
r* – realna neutralna kamatna stopa (često se uzima ~ 2%)
π – trenutačna inflacija
π* – ciljana inflacija (npr. 2%)
y - y* – output gap (% razlike između stvarnog i potencijalnog BDP-a)
     */






    void AzurirajPovijest()
    {

        for (int i = hnbPolje.Length - 1; i > 0 ; i--)
        {
            if (hnbPolje[i - 1] != null)
            {
                hnbPolje[i] = hnbPolje[i - 1];
            }
            else { hnbPolje[i] = Vector3.zero; }
        }

        hnbPolje[0] = new Vector3(inflacija, kamate);

    }

    public Vector3[] DobijPovijest()
    {
        return hnbPolje;
    }



}
