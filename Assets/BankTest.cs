using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankTest : MonoBehaviour
{

    [Header("Generalne varijable")]

    [SerializeField] HNB narodnaBanka;
    [SerializeField] Narod narod;

    [SerializeField] float trenutačniNovac = 100;
    [SerializeField] float trenutačniKrediti = 0;




    [Header("Interne varijable")]

    public float kamataKredit;
    public float štednjaKoličina;
    public float rizik;
    public float obvezniceUzet;

    public float profit;

    public List<Vector3> kreditiLista = new List<Vector3>();
    public List<Vector3> obvezniceLista = new List<Vector3>();




    void Start()
    {
        
    }
    void Update()
    {
        
    }







    public void RačunajSve()
    {

        profit = Kamate() + DrzavneObveznice() - Štednja();
        profit = profit - (profit * (narodnaBanka.inflacija / 100));

        trenutačniNovac += profit - (profit * (narodnaBanka.inflacija / 100));

    }



    public bool DigniKredit(float kredit)
    {

        if (!narod) { narod = FindObjectOfType<Narod>(); }

        int brojMjeseci = narod.kreditMjeseciOtplate;

        // izracun isplate u eur 
        float isplata = kredit * (kamataKredit / 12) * (1 + (kamataKredit / 12))
            / (1 + (kamataKredit / 12) * brojMjeseci - 1);

        // izracun tereta isplate na placu (%)
        // veca inflacija smanjuje br kredita tj lose za banku
        // više štednje je "marketing" koji kaze vise ljudi ce ic u tu banku -> prominit
        // u dijeljeni sustav svih banaka tj zbrajaju se štednje i najvisa pobjeduje

        float teret = ((isplata + (štednjaKoličina / 100)) / (FindObjectOfType<Narod>().prosjecnaPlaca - 
            narodnaBanka.inflacija / 100)) * 100;


        // ako je teret veci od rizika, ne uzimam
        if (rizik < (teret) && trenutačniNovac > kredit)
        {
            trenutačniKrediti += kredit; trenutačniNovac -= kredit;
            // novi kredit - mjeseci otplate, količina, mjesecni rizik (PROMINI GA ////////////////)
            kreditiLista.Add(new Vector3(60, kredit, Mathf.Sqrt(teret)));
            return true;
        }

        return false;

    }
    /*
    Ako želiš usporediti s plaćom:
opterećenje = M / prosječna_mjesečna_plaća
Ako je opterećenje > 0.4, smatra se rizičnim (više od 40% plaće ide na kredit).


Plaća: 1000 
Kredit: 20,000 
Kamatna stopa: 5% godišnje
Trajanje: 5 godina (60 mjeseci)

r = 0.05 / 12 = 0.004166...
n = 60

M = 20000 * 0.004166 * (1 + 0.004166)^60 / ((1 + 0.004166)^60 - 1)
  377.42 

Opterećenje = 377.42 / 1000 = 0.377 → 37.7%
*/



    public float Kamate()
    {
        // štednja 0% daje 0e, 1% štednje će rasti za ? 

        float otplate = 0;

        for (int i = 0; i < kreditiLista.Count; i++)
        {
            // mjeseci za otplatu kredita
            if (kreditiLista[i].x <= 0) { kreditiLista.RemoveAt(i); i--; }

            // smanjivanje mjeseci otplate kredita
            if (kreditiLista[i] != null && i > 0)
            {
                kreditiLista[i] = new Vector3(kreditiLista[i].x - 1, kreditiLista[i].y, kreditiLista[i].z);
            }

            // rizik ne otplacivanja; veci kredit/kamate, veci rizik
            if (UnityEngine.Random.Range(0f, 1f) > kreditiLista[i].z)
            {
                kreditiLista.RemoveAt(i);
                if (i != kreditiLista.Count - 1 && i > 0) { i--; }
            }
            else
            {
                // otplata kredita
                otplate += kamataKredit * kreditiLista[i].y;
            }



        }

        return otplate;

    }

    public float DrzavneObveznice()
    {

        float isplate = 0;

        for (int i = 0; i < obvezniceLista.Count; i++)
        {
            if (obvezniceLista[i].x <= 0) 
            {
                // novac + postotak ekstra od drzave
                isplate+= obvezniceLista[i].y + obvezniceLista[i].y * obvezniceLista[i].z;
                obvezniceLista.RemoveAt(i); i--;
            }
            else if (obvezniceLista[i] != null && i > 0)
            {
                obvezniceLista[i] = new Vector2(obvezniceLista[i].x - 1, 
                    obvezniceLista[i].y);
            }

        }


        float ulogDionice = narodnaBanka.kamate * obvezniceUzet * trenutačniNovac;

        if (trenutačniNovac > ulogDionice && ulogDionice > 0 && trenutačniNovac > 1)
        {
            trenutačniNovac -= ulogDionice;
            obvezniceLista.Add(new Vector3(narodnaBanka.mjeseciZaObveznice, ulogDionice, narodnaBanka.kamate));
        }

        return isplate;
        
    }

    public float Štednja()
    {
        // štednja 0% daje 0e, 1% štednje će rasti za ? 

        return štednjaKoličina;


    }





    public float SviKrediti()
    {

        return trenutačniKrediti;

    }


}
