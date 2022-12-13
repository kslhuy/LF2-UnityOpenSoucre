public enum ProjectileBoxType{
    Projecile_Normal = 1<< 0 ,
    Projecile_NoRebound,
    Projecile_Air,
    Shield = 1 << 1 ,// Ice frezzen
    BlockAll =  1 << 2, // Shield off John
    Arrow,


}

[System.Flags]
public enum ProjectileBoxMask{
    None = 0,
    Normal = 1<< 0 ,

    Blockable = 1 << 1 ,// Ice frezzen
    BlockAll =  1 << 2, // Shield off John

}