using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IgračBanka : MonoBehaviour
{


    [Header("Generalne varijable")]

    [SerializeField] HNB narodnaBanka;
    [SerializeField] Narod narod;

    [SerializeField] float trenutačniNovac = 100;
    [SerializeField] float trenutačniKrediti = 0;

    [SerializeField] float porezSkala = 10;

    [SerializeField] Transform exitUI;



    [Header("KS Elementi")]

    public TMP_InputField kamateInput;
    public TMP_InputField štednjaInput;
    public TMP_InputField rizikInput;
    public TMP_InputField obvezniceInput;
    public Slider timeSpeed;



    [Header("Interne varijable")]

    public float kamataKredit = 100;
    public float štednjaKoličina = 0;
    public float rizik = 0;
    public float obvezniceUzet = 0;
    // ide od 0 do 100, 0 je vrlo raznovesno, 100 je jedan jednini sektor
    // public float monopolDionica

    public float profit;

    public List<Vector3> kreditiLista = new List<Vector3>();
    public List<Vector3> obvezniceLista = new List<Vector3>();

    Vector4[] povijest;
    float[] profitPovijest;




    public void PromjeniKamateKredit()
    {
        float par = float.Parse(kamateInput.text);
        if (par >= 0 && par <= 100)
        {
            kamataKredit = par;
        }
    }

    public void PromjeniŠtednju()
    {
        float par = float.Parse(štednjaInput.text);
        if (par >= 0)
        {
            štednjaKoličina = par;
        }
    }

    public void PromjeniRizik()
    {
        float par = float.Parse(rizikInput.text);
        if (par >= 0 && par <= 100)
        {
            rizik = par;
        }
    }

    public void PromjeniObveznice()
    {
        float par = float.Parse(obvezniceInput.text);
            obvezniceUzet = par;
    }





    private void Start()
    {

        profitPovijest = new float[FindObjectOfType<Visual>().maxHistory];

        povijest = new Vector4[FindObjectOfType<Visual>().maxHistory];
        for (int i = 0; i < povijest.Length; i++)
        {
            povijest[i] = Vector4.zero;
        }

        if (!narod) { narod = FindObjectOfType<Narod>(); }
        if (!narodnaBanka) { narodnaBanka = FindObjectOfType<HNB>(); }
        štednjaKoličina = 0;
        rizik = 0.05f;

        kamateInput.text = 5.ToString();
        štednjaInput.text = 0.ToString();
        rizikInput.text = 5.ToString();
        obvezniceInput.text = 0.ToString();

    }


    private void Update()
    {


        transform.GetChild(0).GetComponent<TMP_Text>().text =
           "kredit " + (Mathf.Round(kamataKredit * 100)) / 100.0 + "%";
        transform.GetChild(1).GetComponent<TMP_Text>().text =
            "štednja " + (Mathf.Round(štednjaKoličina * 100)) / 100.0 + "€";
        transform.GetChild(2).GetComponent<TMP_Text>().text =
            "rizik " + rizik;
        transform.GetChild(3).GetComponent<TMP_Text>().text =
            "obvezniceUzet " + obvezniceUzet;




        povijest[0] = new Vector4(kamataKredit, štednjaKoličina, rizik, trenutačniNovac);
        profitPovijest[0] = profit;


        narodnaBanka.vrijemeOdluka = timeSpeed.value;



        if (Input.GetKeyDown(KeyCode.Escape)) 
        { exitUI.gameObject.SetActive(!exitUI.gameObject.activeInHierarchy) ; }


    }





    public void RačunajSve()
    {

        AzurirajPovijest();

        profit = Kamate() + DrzavneObveznice() - Štednja();

        profit = profit - (profit * (narodnaBanka.inflacija / 100)) - PorezDividende();

        obvezniceUzet = 0; obvezniceInput.text = 0.ToString();

        trenutačniNovac += profit;


    }





    public bool DigniKredit(float kredit)
    {


        int brojMjeseci = narod.kreditMjeseciOtplate;

        // izracun isplate u eur 
        float isplata = kredit * (kamataKredit / 12) * (1 + (kamataKredit / 12))
            / (1 + (kamataKredit / 12) * brojMjeseci - 1);

        // izracun tereta isplate na placu (%)
        // veca inflacija smanjuje br kredita tj lose za banku
        // više štednje je "marketing" koji kaze vise ljudi ce ic u tu banku -> prominit
        // u dijeljeni sustav svih banaka tj zbrajaju se štednje i najvisa pobjeduje

        float teret = ((isplata) / ((štednjaKoličina / 10) +
            FindObjectOfType<Narod>().prosjecnaPlaca)) * 100;


        // ako je teret veci od rizika, ne uzimam
        if (rizik > (teret) && trenutačniNovac > kredit)
        {
            trenutačniKrediti += kredit; trenutačniNovac -= kredit;
            // novi kredit - mjeseci otplate, količina, mjesecni rizik (PROMINI GA ////////////////)
            kreditiLista.Add(new Vector3(narod.kreditMjeseciOtplate, kredit, Mathf.Sqrt(teret)));
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



    float Kamate()
    {
        // štednja 0% daje 0e, 1% štednje će rasti za ? 

        float otplate = 0;

        for (int i = 0; i < kreditiLista.Count; i++)
        {

            bool nestaje = false;

            // smanjivanje mjeseci otplate kredita
            kreditiLista[i] = new Vector3(kreditiLista[i].x - 1, kreditiLista[i].y, kreditiLista[i].z);

            // rizik ne otplacivanja; veci kredit/kamate, veci rizik
            if (UnityEngine.Random.Range(0f, 100f) < kreditiLista[i].z)
            {
                nestaje = true;
            }
            else
            {
                // otplata kredita
                otplate += kamataKredit * kreditiLista[i].y;
            }


            // mjeseci za otplatu kredita / micanje ako je otplaceon
            if (i < kreditiLista.Count - 1 && kreditiLista[i] != null && kreditiLista[i].x <= 0)
            {
                nestaje = true;
            }



            if (nestaje)
            {
                trenutačniKrediti -= kreditiLista[i].y;
                narod.KreditManje(); kreditiLista.RemoveAt(i);

                if (i < kreditiLista.Count - 1)
                {
                    i--;
                }
                else { i = kreditiLista.Count; }
            }



        }


        return otplate;

    }


    ///promini/______________///_______________________________________/summar y++ me ubija
    float DrzavneObveznice()
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


        float ulogDionice = obvezniceUzet;

        if (trenutačniNovac > ulogDionice && ulogDionice > 0 && trenutačniNovac > 1)
        {
            trenutačniNovac -= ulogDionice;
            obvezniceLista.Add(new Vector3(narodnaBanka.mjeseciZaObveznice, ulogDionice, narodnaBanka.kamate));
        }

        return isplate;

    }


    float Štednja()
    {
        // štednja 0% daje 0e, 1% štednje će rasti za ? 

        if (trenutačniNovac > štednjaKoličina)
        {
            return štednjaKoličina;
        }
        else { return 0; }

    }

    float PorezDividende()
    {

        return ((trenutačniNovac * narodnaBanka.kamate / 100)
            + Mathf.Max(trenutačniNovac - 1000, 0));

    }



    void AzurirajPovijest()
    {

        for (int i = povijest.Length - 1; i > 0; i--)
        {
            if (povijest[i - 1] != null)
            {
                povijest[i] = povijest[i - 1];
            }
            else { povijest[i] = Vector3.zero; }

            profitPovijest[i] = profitPovijest[i - 1];

        }

        povijest[0] = new Vector4(kamataKredit, štednjaKoličina, rizik, trenutačniNovac);
        profitPovijest[0] = profit;

    }

    public Vector4[] DobijPovijest()
    {
        return povijest;
    }
    public float[] DobijProfit() { return profitPovijest; }


    public float SviKrediti()
    {
        return trenutačniKrediti;
    }

    public float BrojKredita()
    {
        return kreditiLista.Count;
    }



    public void ExitApp()
    {
        Application.Quit();
    }




}
