using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public int brojBanaka = 5;
    public int brojBanakaTraining = 5;
    public int daljina = 5;
    public Transform roditeljBanaka;
    public Transform bankaPrefab;



    void Start()
    {

        for (int i = 1; i < brojBanaka; i++)
        {
            Transform ba = Instantiate(bankaPrefab, roditeljBanaka);
            ba.localPosition += i * Vector3.forward * daljina;
        }

        for (int i = 1; i < brojBanakaTraining; i++)
        {
            Transform ba = Instantiate(bankaPrefab, roditeljBanaka);
            ba.localPosition += - 20 * i * Vector3.up * daljina;
        }

    }


}
