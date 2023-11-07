using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzStat:Stat
{
    // Start is called before the first frame update
    public int score;
    public List<int> score_timeline;
    public List<float> time_timeline;

    public List<int> multiplier_timeline;

    public int multiplier;
    public BlitzStat(){
        score = 0;
        score_timeline = new List<int>();
        time_timeline = new List<float>();
        multiplier_timeline = new List<int>();
    }
}
