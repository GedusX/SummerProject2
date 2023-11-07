using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Handler : MonoBehaviour
{
    // Start is called before the first frame update
    static public int score = 0;
    static public int level = 1;

    
    
    //public static Handler instance;
    

    public abstract void update_score(int sc);
    public abstract void gameover();

    public abstract void update_hex(int nor, int bomb, int laser, int void_hex);
    
    //public abstract void update_stats

    //public abstract void hint();

    public static float multiplier;
    //some config
    public int max_lock_on_board = 15;
    public bool no_move;

    public List<GameObject> holding;

    public int difficulty;

    


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
