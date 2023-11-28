using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Position : MonoBehaviour
{
    SaveData data;
    [SerializeField] GameObject LUO;
    [SerializeField] GameObject LDO;
    [SerializeField] GameObject RUO;
    [SerializeField] GameObject RDO;

    private void Start()
    {
        data = GetComponent<DataManager>().data;

        LUO.transform.position = data.LU;
        LDO.transform.position = data.LD;
        RUO.transform.position = data.RU;
        RDO.transform.position = data.RD;
    }


    public void SetData()
    {
        data.LU = LUO.transform.position;
        data.LD = LDO.transform.position;
        data.RU = RUO.transform.position;
        data.RD = RDO.transform.position;
    }

    public void DelData()
    {
        data.LU = new Vector2(-2,2);
        data.LD = new Vector2(-2,-2);
        data.RU = new Vector2(2,2);
        data.RD = new Vector2(2, -2);
    }
}
