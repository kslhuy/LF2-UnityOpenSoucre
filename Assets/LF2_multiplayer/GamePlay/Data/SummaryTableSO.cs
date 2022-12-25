using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummaryTable", menuName = "SummaryTable")]
public class SummaryTableSO : ScriptableObject {
    public string Name;
    public int NumerKills ; 
    public int DamageAmounts;
    public int HPLost;
    public int MPUsage;
    public LF2.WinState Status ;

    public void UpdateSummaryTable(int damage ,int hpLost , int mpUsage , int kill , LF2.WinState statusLifeState){
        

    }





}


