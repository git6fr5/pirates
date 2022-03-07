/* --- Libaries --- */
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class NetworkSettings {

    public bool Linear;
    public int Width;
    public int Height;
    public int SecondaryBranchDepth;
    public int TertiaryBranchDepth;
    public bool OverlapSecondaryBranches;
    public bool OverlapTertiaryBranches;

}