using System.Collections;
using System.Collections.Generic;
using LF2.Client;
using UnityEngine;

[CreateAssetMenu(fileName = "SummaryTable", menuName = "SummaryTable")]
public class SummaryTableSO : ScriptableObject {
    public string Name;
    public int NumerKills ; 
    public int DamageAmounts;
    public int HPLost;
    public int MPUsage;
    public WinState Status ;

    public void UpdateSummaryTable(int damage ,int hpLost , int mpUsage , int kill , WinState statusLifeState){
        

    }





}


