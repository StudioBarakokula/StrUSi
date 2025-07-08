using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Visual : MonoBehaviour
{

    [Header("1-hnb  2-sve  3-jedna+broj<5  4-graf")]

    public float maxHeight = 2;
    public float minHeight = 0.4f;
    public float width;
    public float moveRight;
    public float visuaUpdateTime = 1;
    public int maxHistory = 16;

    public Transform bankaParent;

    public Transform emptyTran;
    public Transform choiceUIElement;

    public Transform textOneBank;
    public Transform textInflation;

    public Transform buttonsNormal;
    public Transform buttonsChoice;
    public Transform buttonsIgrac;

    public Transform visualParent;

    public Transform grafElement;
    public Transform grafStartMid;

    bool inflation = true;
    bool allBanks;
    bool oneBank;
    int bankCurr;
    bool igrac;
    bool profit;

    bool single;
    bool one, two, tri, cet;


    HNB narodnaBanka;




    public void HNBVisual() { KillAll(); inflation = true; VisualUp(); }
    public void BankeVisual() { KillAll(); allBanks = true; }
    public void JednaVisual() { KillAll(); bankCurr = -1; oneBank = true; VisualUp(); }
    public void IgraèVisual() { KillAll(); bankCurr = -1; oneBank = true; VisualUp(); }
    public void ProfitVisual() { KillAll(); profit = true; VisualUp(); }


    // nemoj
    // potakaj ovo sve 
    /*
   public void SingleInflacija() { single = true; one = true; }
   public void SingleKamate() { single = true; two = true; }


   public void SingleKredit() { single = true; one = true; }
   public void SingleŠtednja() { single = true; two = true; }
   public void SingleRizik() { single = true; one = true; }
   public void SingleIObveznice() { single = true; two = true; }
      */

    public void SingleProfitJedan() { single = true; one = true; }
   public void SingleProfitDva() { single = true; two = true; }
   public void SingleProfitTri() { single = true; tri = true; }
   public void SingleProfitCet() { single = true; cet = true; }








    void Start()
    {


        narodnaBanka = FindObjectOfType<HNB>();
        visuaUpdateTime = narodnaBanka.vrijemeOdluka;


        StartCoroutine(VizualnoPetlja());
        KillAll();
    }
    void Update()
    {

        choiceUIElement.gameObject.SetActive(false); 


        if (oneBank && bankCurr == -1) 
        {
            KillAll(); Choice(); 
            if (Input.GetKeyDown(KeyCode.Alpha1) || one) { UnChoice(); bankCurr = 0; }
            if (Input.GetKeyDown(KeyCode.Alpha2) || two) { UnChoice(); bankCurr = 1; }
            if (Input.GetKeyDown(KeyCode.Alpha3) || tri) { UnChoice(); bankCurr = 2; }
            if (Input.GetKeyDown(KeyCode.Alpha4) || cet) { UnChoice(); bankCurr = 3; }
            //if (Input.GetKeyDown(KeyCode.Alpha5) || single) { UnChoice(); bankCurr = 4; }
            oneBank = true;
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Alpha1)) { KillAll(); inflation = true; VisualUp(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { KillAll(); allBanks = true; }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { KillAll(); bankCurr = -1; oneBank = true; VisualUp(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { KillAll(); profit = true; VisualUp(); }

        }

        

    }


   


    IEnumerator VizualnoPetlja()
    {

        yield return new WaitForSeconds(visuaUpdateTime);
        Vizualno();
        StartCoroutine(VizualnoPetlja());
    }


    void Vizualno()
    {

        NeCrtaj();

        if (inflation)
        {

            Vector3[] narPov = narodnaBanka.DobijPovijest();

            Crtaj(grafStartMid.position + Vector3.up * 2, narPov, 1);
            Crtaj(grafStartMid.position - Vector3.up * 2, narPov, 2);

            // daje trenutacne brojeve
            textInflation.GetChild(0).GetComponent<TMP_Text>().text = "inflacija(%) " +
                (Mathf.Round(narPov[0].x * 100) / 100).ToString();
            textInflation.GetChild(1).GetComponent<TMP_Text>().text = "kamate(%) " +
                (Mathf.Round(narPov[0].y * 100) / 100).ToString();

            textInflation.gameObject.SetActive(true);
        }
        else if (allBanks)
        {
            bankaParent.position = new Vector3(bankaParent.position.x, 0, bankaParent.position.z);
        }
        else if (oneBank && bankCurr != -1)
        {

            Vector4[] vecfo = null;
            if (bankaParent.GetChild(bankCurr).GetComponent<BankAgent>())
            {
                vecfo = bankaParent.GetChild(bankCurr).GetComponent<BankAgent>().DobijPovijest();
            }
            else if(bankaParent.GetChild(bankCurr).GetComponent<IgraèBanka>())
            {
                vecfo = bankaParent.GetChild(bankCurr).GetComponent<IgraèBanka>().DobijPovijest();
            }

            Vector3[] firstv = new Vector3[maxHistory];
            Vector3[] secondv = new Vector3[maxHistory];

            for (int i = 0; i < vecfo.Length; i++)
            {
                firstv[i] = new Vector3(vecfo[i].x, vecfo[i].y, vecfo[i].z);
                secondv[i] = new Vector3(vecfo[i].w, 0, 0);
            }


            Crtaj(grafStartMid.position + Vector3.up * 3, firstv, 1);
            Crtaj(grafStartMid.position + Vector3.up * 1, firstv, 2);
            Crtaj(grafStartMid.position + Vector3.up * -1, firstv, 3);
            Crtaj(grafStartMid.position + Vector3.up * -3, secondv, 1);

            textOneBank.GetChild(0).GetComponent<TMP_Text>().text = "kamate(%) " +
                (Mathf.Round(firstv[0].x * 100) / 100).ToString();
            textOneBank.GetChild(1).GetComponent<TMP_Text>().text = "štednja(€) " +
                (Mathf.Round(firstv[0].y * 100) / 100).ToString();
            textOneBank.GetChild(2).GetComponent<TMP_Text>().text = "rizik(%) " +
                (Mathf.Round(firstv[0].z * 100) / 100).ToString();
            textOneBank.GetChild(3).GetComponent<TMP_Text>().text = "novac(€) " +
                (Mathf.Round(secondv[0].x * 100) / 100).ToString();



            textOneBank.gameObject.SetActive(true);
        }
        else if (profit)
        {

            for (int i = 0; i < 4; i++)
            {
                
                float[] vecfo = null;

                if (bankaParent.GetChild(i).GetComponent<BankAgent>())
                {
                    vecfo = bankaParent.GetChild(i).GetComponent<BankAgent>().DobijProfit();
                }
                else if (bankaParent.GetChild(i).GetComponent<IgraèBanka>())
                {
                    vecfo = bankaParent.GetChild(i).GetComponent<IgraèBanka>().DobijProfit();
                }

                Vector3[] vec = new Vector3[vecfo.Length];

                for (int y = 0; y < vecfo.Length; y++)
                {
                    vec[y] = new Vector3(vecfo[y], 0, 0);
                }



                Crtaj(grafStartMid.position + Vector3.up * (3 - (i * 2)), vec, 1);



                string ay = "";
                string bay = "";

                if (i == 0)
                {
                    ay = "igraè profit(€) ";

                    if (bankaParent.GetChild(i).GetComponent<IgraèBanka>())
                    {
                        bay = " s " +
                            bankaParent.GetChild(i).GetComponent<IgraèBanka>().BrojKredita() + " kredita";
                    }
                }
                else if (i == 1)
                {
                    ay = "banka 1 profit(€) ";

                }
                else if (i == 2)
                {
                    ay = "banka 2 profit(€) ";
                }
                else if (i == 3)
                {
                    ay = "banka 3 profit(€) ";
                }


                textOneBank.GetChild(i).GetComponent<TMP_Text>().text = ay +
                    (Mathf.Round(vecfo[0] * 100) / 100).ToString() + bay;

            }


            textOneBank.gameObject.SetActive(true);
        }
        
    }


    // dijeljeno maxHeight, i pozicija / 2 da svi budu na istoj y na dnu
    void Crtaj(Vector3 pos, Vector3[] povijest, int xyz)
    {
        //// na poziciji iz poviesti uzet x, y ili z i napraviti graf
        ///

        Transform parelem = Instantiate(emptyTran,
                new Vector3(pos.x, pos.y, pos.z), transform.rotation, visualParent);


        float minH = 100, maxH = 0;
        int minHindex = 0, maxHindex = 0;


        for (int i = 0; i < maxHistory; i++)
        {



            Transform elem = parelem;
            float visina = 0;


            if (parelem.childCount < maxHistory)
            {

                elem = Instantiate(grafElement, new Vector3(pos.x,
                    pos.y, pos.z + moveRight * i), transform.rotation, parelem);

            }
            else
            {
                elem = parelem.GetChild(i);
            }


            switch (xyz)
            {
                case 1: visina = povijest[i].x; break;
                case 2: visina = povijest[i].y; break;
                case 3: visina = povijest[i].z; break;
            }


            elem.localScale = new Vector3(width, MinMax(visina), width);

            // da svima dno bude na istoj tocki
            elem.localPosition = new Vector3(elem.localPosition.x,
                -2 + elem.localScale.y / 2, elem.localPosition.z);


            if (visina < minH) { minH = visina; minHindex = i; }
            if (visina > maxH) { maxH = visina; maxHindex = i; }


        }



        // gledamo da li je najmanja vrijednost manja od nase minHeight
        if (minH < minHeight) 
        {

            float visina = 0;

            switch (xyz)
            {
                case 1: visina = povijest[minHindex].x; break;
                case 2: visina = povijest[minHindex].y; break;
                case 3: visina = povijest[minHindex].z; break;
            }

            // uzimamo tu najmanju visinu i dobijamo vrijednost za promijeniti visinu
            // npr minHeight = -2, visina = -4, dijeli visina = 2;
            // tj. -4 / 2 = -2 stavljamo visinu na min vrijednost 
            float dijeliVisina = visina / minHeight;



            // zatim prolazimo sve elemente i mijenjamo visine
            for (int i = 0; i < maxHistory; i++)
            {
                switch (xyz)
                {
                    case 1: visina = povijest[i].x; break;
                    case 2: visina = povijest[i].y; break;
                    case 3: visina = povijest[i].z; break;
                }

                parelem.GetChild(i).localScale = new Vector3(1, MinMax(visina / dijeliVisina), 1);
                parelem.GetChild(i).localPosition = new Vector3(parelem.GetChild(i).localPosition.x,
                -2 + parelem.GetChild(i).localScale.y / 2, parelem.GetChild(i).localPosition.z);

            }

        }
        if(maxH > maxHeight)
        {
            float visina = 0;

            switch (xyz)
            {
                case 1: visina = povijest[maxHindex].x; break;
                case 2: visina = povijest[maxHindex].y; break;
                case 3: visina = povijest[maxHindex].z; break;
            }


            float dijeliVisina = visina / maxHeight;


            for (int i = 0; i < maxHistory; i++)
            {
                switch (xyz)
                {
                    case 1: visina = povijest[i].x; break;
                    case 2: visina = povijest[i].y; break;
                    case 3: visina = povijest[i].z; break;
                }

                parelem.GetChild(i).localScale = new Vector3(1, MinMax(visina / dijeliVisina), 1);
                parelem.GetChild(i).localPosition = new Vector3(parelem.GetChild(i).localPosition.x,
                -2 + parelem.GetChild(i).localScale.y / 2, parelem.GetChild(i).localPosition.z);

            }

        }

    }

    void NeCrtaj()
    {

        for (int i = 0; i < visualParent.childCount; i++)
        {
            Destroy(visualParent.GetChild(i).gameObject);
        }

    }




    void VisualUp()
    {
        visualParent.position = new Vector3(visualParent.position.x, 0, visualParent.position.z);
        Vizualno();
    }

    void KillAll()
    {

        inflation = false;
        allBanks = false;
        igrac = false;
        profit = false;
        if(oneBank && bankCurr > -1 && bankCurr < 6) { bankCurr = -1; oneBank = false; }
        bankaParent.position = new Vector3(bankaParent.position.x, -40, bankaParent.position.z);
        visualParent.position = new Vector3(visualParent.position.x, -40, visualParent.position.z);

        textOneBank.gameObject.SetActive(false);
        textInflation.gameObject.SetActive(false);

        buttonsNormal.gameObject.SetActive(true);
        buttonsChoice.gameObject.SetActive(false);
        buttonsIgrac.gameObject.SetActive(false);

        single = false;
        one = false; two = false; tri = false; cet = false;

    }


    void Choice()
    {
        choiceUIElement.gameObject.SetActive(true);
        buttonsNormal.gameObject.SetActive(false);
        buttonsChoice.gameObject.SetActive(true);
    }
    void UnChoice()
    {
        choiceUIElement.gameObject.SetActive(false);
        buttonsNormal.gameObject.SetActive(true);
        buttonsChoice.gameObject.SetActive(false);

        single = false;
        one = false; two = false; tri = false; cet = false;
    }



    float MinMax(float broj)
    {

        return Mathf.Min(maxHeight, Mathf.Max(broj, 0.01f));

    }



}
