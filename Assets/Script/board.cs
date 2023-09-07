using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class board : MonoBehaviour
{
    
    // The width of the board in cells
    [SerializeField] int boardWidth;
    // The height of the board in cells
    [SerializeField] int boardHeight;
    // The prefab of an object
    [SerializeField] GameObject hexagon;

    [SerializeField] GameObject hexagon_unit;

    public bool idle;

    float dim_timer;

    // The 2D array to store the objects, while i represents for col (x), and j represents for row (y);
    [SerializeField] private GameObject[,] hexagon_slots;
    

    [SerializeField] public GameObject[,] hexagons;

    public GameObject pointstext;

    //public int difficulty;

    // The width of an object
    public float hexWidth = 1.5f;
    // The height of an object
    public float hexHeight = 1.5f;

    public List<GameObject> invicible;

    

    
    
    public List<GameObject> exploded;

    public List<GameObject> dimmed;
    private void Awake() {
        invicible = new List<GameObject>();
        exploded = new List<GameObject>();
        dimmed = new List<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // Create the board
        CreateBoard();
        
        
        //Time.timeScale = 0;
    }

    // Update is called once per frame
    public bool legal_move = false;

    
    void Update()
    {
        //Debug.Log(Random.Range(0,-5));
        dim_timer-=Time.deltaTime;
        if (dim_timer<0)
            dim_timer = 0;
        
        if (Input.GetMouseButton(0)){
            Debug.Log(pix_to_grid(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            Vector2Int grid = pix_to_grid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (is_legit(grid)){
                hexagons[grid.x,grid.y].GetComponent<Hexa>().ClickOn();
            }
        }
        
        //Debug.Log(get_hint());
        for (int i = 0;i<boardWidth;i++){
            for (int j = 0; j<boardHeight;j++){
                if (hexagons[i,j])
                    hexagons[i,j].GetComponent<Hexa>().pos = new Vector2Int(i,j);
            }
        }
        //legal_move = (drag_list.Count>=3||(drag_list.Count==2&&drag_list[0].GetComponent<Hexa>().type == Hexa_Type.WILD));
        if (Input.GetMouseButtonUp(0) && drag_list.Count>0 && idle){
            invicible.Clear();
            exploded.Clear();
            idle = false;
            
            if (drag_list.Count==2 && drag_list[0].GetComponent<Hexa>().type==Hexa_Type.WILD){
                update_lock();
                lock_create();
                if (drag_list[1].GetComponent<Hexa>().type==Hexa_Type.WILD){
                    clear_all(drag_list[0],drag_list[1]);
                }
                else{
                    //Debug.Log("hehe");
                    StartCoroutine(color_clear(drag_list[0],drag_list[1].GetComponent<Hexa>().color));
                    dim_timer+=0.3f;
                    StartCoroutine(hex_falling());
                }
            }
            else if (drag_list.Count>=3){
                idle = false;
                update_lock();
                lock_create();
                //Debug.Log(drag_list.Count);
                Hexa_Type got = Hexa_Type.NORMAL;
                if (drag_list.Count>=7){
                    got = Hexa_Type.WILD;
                    AudioManager.instance.Play("voidhex");
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                    //score_text.GetComponent<Score_text>().update_points(500);
                    GameObject.Find("Level_handler").GetComponent<Handler>().update_score(500);
                }
                else if (drag_list.Count>=6){
                    got = Hexa_Type.LINE;
                    AudioManager.instance.Play("starhex");
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                   // score_text.GetComponent<Score_text>().update_points(200);
                    GameObject.Find("Level_handler").GetComponent<Handler>().update_score(200);
                }
                else if (drag_list.Count>=5){
                    got = Hexa_Type.BOMB;
                    AudioManager.instance.Play("powerhex");
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                    //score_text.GetComponent<Score_text>().update_points(150);
                    GameObject.Find("Level_handler").GetComponent<Handler>().update_score(150);
                }
                if (got!=Hexa_Type.NORMAL){
                    invicible.Add(drag_list[0]);
                    for (int k = drag_list.Count-1;k>=0;k--){
                        if (drag_list[k].GetComponent<Hexa>().type == Hexa_Type.BOMB){
                            detonate(drag_list[k]);
                        }
                        else if (drag_list[k].GetComponent<Hexa>().type == Hexa_Type.LINE){
                            StartCoroutine(lasering(drag_list[k]));
                        }
                        else if (drag_list[k].GetComponent<Hexa>().type == Hexa_Type.WILD){
                            detonate(drag_list[k]);
                        }
                        else{
                            if (k>0)
                                drag_list[k].GetComponent<Hexa>().combining();
                        }

                        
                    }
                    drag_list[0].GetComponent<Hexa>().change_to(got);
                }
                else{
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                    //score_text.GetComponent<Score_text>().update_points(50+50*(drag_list.Count-3));
                    GameObject.Find("Level_handler").GetComponent<Handler>().update_score(50+50*(drag_list.Count-3));
                    AudioManager.instance.Play("gotcha");
                    foreach (GameObject i in drag_list){
                        if (i.GetComponent<Hexa>().type == Hexa_Type.BOMB){
                            detonate(i);
                        }
                        else if (i.GetComponent<Hexa>().type == Hexa_Type.LINE){
                            StartCoroutine(lasering(i));
                        }
                        else{
                            //dimmed.Add(i);
                            if (!exploded.Contains(i))
                                i.GetComponent<Hexa>().dimming();
                        }
                    }

                }
                
                dim_timer+=0.4f;
                StartCoroutine(hex_falling());
            }
            else{
                AudioManager.instance.Play("badmove");
                idle = true;
                drag_list.Clear();
            }
        }
    }
    
    
    // Create the board with objects

    public Vector2 grid_to_pix(int i, int j){
        float xOffset = 0.8743f;
        float yOffset = 0.4722f;
        float x = i * xOffset;
        float y = j * 0.9425f + (i % 2) * yOffset;
        return transform.position + new Vector3(x,y);
    }
    public Vector2Int pix_to_grid(Vector3 pos){
        float xOffset = 0.8743f;
        float yOffset = 0.4722f;
        
        Vector2 inner_pos = pos-transform.position;
        int i = Mathf.RoundToInt(inner_pos.x/xOffset);
        int j = Mathf.RoundToInt((inner_pos.y-(i % 2)*yOffset)/0.9425f);
        return new Vector2Int(i,j);
    }
    void CreateBoard()
    {
        // Initialize the array
        hexagon_slots = new GameObject[boardWidth, boardHeight];
        hexagons = new GameObject[boardWidth, boardHeight];
        

        // Calculate the offset between adjacent objects
        

        // Loop through the rows and columns of the board
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                // Calculate the position of the object based on its row and column
                // Instantiate a new object at the position
                GameObject newHexagon = Instantiate(hexagon, grid_to_pix(i,j), Quaternion.identity);

                // Set the parent of the object to be this board object
                newHexagon.transform.parent = this.transform;
                
                newHexagon.transform.Find("board_border").Find("board_border_1").gameObject.SetActive(j == boardHeight-1);
                newHexagon.transform.Find("board_border").Find("board_border_2").gameObject.SetActive((j == boardHeight-1 && i%2!=0)||(i == boardWidth-1));
                newHexagon.transform.Find("board_border").Find("board_border_3").gameObject.SetActive((j == 0 && i%2==0)||(i == boardWidth-1));
                newHexagon.transform.Find("board_border").Find("board_border_4").gameObject.SetActive(j == 0);
                newHexagon.transform.Find("board_border").Find("board_border_5").gameObject.SetActive((j == 0 && i%2==0)||(i == 0));
                newHexagon.transform.Find("board_border").Find("board_border_6").gameObject.SetActive((j == boardHeight-1 && i%2!=0)||(i==0));
                
                // Store the object in the array
                hexagon_slots[i, j] = newHexagon;
                hexagons[i,j] = null;
            }
        }
        
        StartCoroutine(fill_up());
    }
    IEnumerator fill_up(){
        idle = false;
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (hexagons[i,j]==null){
                    GameObject newHexagon = Instantiate(hexagon_unit, grid_to_pix(i,j), Quaternion.identity);

                    newHexagon.transform.parent = this.transform;
                    
                    hexagons[i,j] = newHexagon;

                }
                
            }
        }
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (hexagons[i,j]!=null){
                    hexagons[i,j].GetComponent<Hexa>().create_hex();
                }
                
            }
            yield return new WaitForSeconds(0.15f);
        }
        idle = true;
    }


    //drag and drop system

    public bool is_next_to(GameObject obj1, GameObject obj2){
        Vector2Int pos1 = obj1.GetComponent<Hexa>().pos;
        Vector2Int pos2 = obj2.GetComponent<Hexa>().pos;
        //Debug.Log((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude);
        return (((grid_to_pix(pos2.x, pos2.y)-grid_to_pix(pos1.x,pos1.y)).magnitude)<=hexWidth*1.1f);

    }

    public List<GameObject> drag_list = new List<GameObject>();
    //public int list_max = 99;

    public void checking(GameObject hex){
        
        
        if (!drag_list.Contains(hex)){
            if (drag_list.Count == 0){
                drag_list.Add(hex);
                AudioManager.instance.Play("select");
            }
            else{
                if (!is_next_to(drag_list[drag_list.Count-1],hex)){
                    return;
                }
                if (drag_list[0].GetComponent<Hexa>().type==Hexa_Type.WILD){
                    if (drag_list.Count>=2){
                        legal_move = (drag_list.Count>=3||(drag_list.Count==2&&drag_list[0].GetComponent<Hexa>().type == Hexa_Type.WILD));
                        return;
                    }
                    
                    drag_list.Add(hex);
                    AudioManager.instance.Play("select");
                }
                else if (drag_list[drag_list.Count-1].GetComponent<Hexa>().color==hex.GetComponent<Hexa>().color && is_next_to(drag_list[drag_list.Count-1], hex)){
                    drag_list.Add(hex);
                    AudioManager.instance.Play("select");
                }
            }
        }
        else{
            //if (drag_list.IndexOf(hex)==0){
            //    return;
            //}
            while (drag_list[drag_list.Count-1]!=hex){
                drag_list.RemoveAt(drag_list.Count-1);
                AudioManager.instance.Play("select");
            }
            
        }
        legal_move = (drag_list.Count>=3||(drag_list.Count==2&&drag_list[0].GetComponent<Hexa>().type == Hexa_Type.WILD));

    }
    //falling
    
    IEnumerator hex_falling(){
        
        yield return new WaitUntil(() => (dim_timer<=0));
        check_lock();
        float max_fall_duration = 0;
        drag_list.Clear();
        for (int i =0;i<boardWidth;i++){
            for (int j = 0; j<boardHeight;j++){
                if (hexagons[i,j]){
                    
                    Vector2Int target_pos = new Vector2Int(i,j);
                    for (int k = 1;j-k>=0;k++){
                        if (!hexagons[i,j-k]){
                            target_pos = new Vector2Int(i,j-k);
                        }
                        else{
                            break;
                        }
                    }
                    if (target_pos != hexagons[i,j].GetComponent<Hexa>().pos){
                        //Debug.Log("jello");
                        hexagons[target_pos.x,target_pos.y] = hexagons[i,j];
                        hexagons[i,j] = null;
                        max_fall_duration = Mathf.Max(max_fall_duration,(grid_to_pix(target_pos.x,target_pos.y)-grid_to_pix(i,j)).magnitude/8);
                        hexagons[target_pos.x,target_pos.y].transform.DOMove(grid_to_pix(target_pos.x,target_pos.y),(grid_to_pix(target_pos.x,target_pos.y)-grid_to_pix(i,j)).magnitude/8).SetEase(Ease.InOutSine);
                    
                    }
                }
            }
        }
        yield return new WaitForSeconds(max_fall_duration);
        StartCoroutine(filling());
    }
    int num_of_hex(){
        int res = 0;
         for (int i =0;i<boardWidth;i++)
            for (int j = 0; j<boardHeight;j++)
                if (is_legit(new Vector2Int(i,j))){
                    res+=1;
                }
        return res;
    }
    
    IEnumerator filling(){
        List<GameObject> slot_avail = new List<GameObject>();
        //add some specials thing later
        for (int i =0;i<boardWidth;i++){
            for (int j = 0; j<boardHeight;j++){
                if (!hexagons[i,j]){
                    GameObject newHexagon = Instantiate(hexagon_unit, grid_to_pix(i,j), Quaternion.identity);
                    newHexagon.transform.parent = this.transform;
                    slot_avail.Add(newHexagon);
                    hexagons[i,j] = newHexagon;
                }
            }
        }
        yield return new WaitUntil(() => (num_of_hex() >= boardHeight*boardWidth));
        if (GameObject.Find("Level_handler").GetComponent<Handler>().no_move){
            for (int i = 0;i<=(100-GameObject.Find("Level_handler").GetComponent<Handler>().difficulty)/10;i++){
                if (get_hint()){
                    break;
                }
                else {
                    foreach (GameObject k in slot_avail){
                        k.GetComponent<Hexa>().color_random();
                }
            }

            }
        }
        else{
            while (!get_hint()){
                foreach (GameObject k in slot_avail){
                    k.GetComponent<Hexa>().color_random();
                }
            }
        }

        
        foreach (GameObject i in slot_avail){
            i.GetComponent<Hexa>().create_hex();
        }

        yield return new WaitForSeconds(0.3f);
        if (get_hint())
            idle = true;
        else{
            yield return new WaitForSeconds(0.6f);
            GameObject.Find("Level_handler").GetComponent<Handler>().gameover();
        }
           
    }
    
    public bool is_legit(Vector2Int pos){
        if (pos.x<0||pos.x>=boardWidth)
            return false;
        if (pos.y<0||pos.y>=boardHeight)
            return false;
        if (!hexagons[pos.x,pos.y])
            return false;
        return true;
    }
    //special hex trigger
    public GameObject[] effects;
    IEnumerator delay_bomb(float time, GameObject go){
            dim_timer+=time;
            yield return new WaitForSeconds(time);
            detonate(go);
            
    }
    public void detonate(GameObject hex){
        if (!exploded.Contains(hex)){
            exploded.Add(hex);
            if (!invicible.Contains(hex)){
                hex.GetComponent<Hexa>().explode();
                Instantiate(effects[0],hex.transform.position,Quaternion.identity);
            }
        }
        AudioManager.instance.Play("explode");
        dim_timer+=0.4f;
        //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
        //score_text.GetComponent<Score_text>().update_points(20);
        GameObject.Find("Level_handler").GetComponent<Handler>().update_score(20);
        foreach (var k in hex.GetComponent<Hexa>().get_all_surrounded()){
            if (!is_legit(k.GetComponent<Hexa>().pos))
                    continue;
            if (exploded.Contains(k))
                    continue;
            if (invicible.Contains(k))
                    continue;
            if (k.GetComponent<Hexa>().type==Hexa_Type.BOMB){
                StartCoroutine(delay_bomb(0.1f,k));
            }
            else if (k.GetComponent<Hexa>().type==Hexa_Type.LINE){
                    StartCoroutine(lasering(k));
            }
            else if (k.GetComponent<Hexa>().type==Hexa_Type.WILD){
                    detonate(k);
            }
            else{
                    //Debug.Log("hehe");
                    exploded.Add(k);
                    foreach (var item in k.GetComponent<Hexa>().get_all_surrounded()){
                        if (!exploded.Contains(item)){
                            StartCoroutine(item.GetComponent<Hexa>().knockback(
                                (grid_to_pix(item.GetComponent<Hexa>().pos.x,item.GetComponent<Hexa>().pos.y)
                                - grid_to_pix(k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y)).normalized,0.5f));
                        }
                    }
                    //dimmed.Remove(k);
                    k.GetComponent<Hexa>().explode();
                    Instantiate(effects[0],grid_to_pix(k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y),Quaternion.identity);
            }
            //score_text.GetComponent<Score_text>().update_points(20);
            GameObject.Find("Level_handler").GetComponent<Handler>().update_score(20);
        }
    }

    IEnumerator lasering(GameObject hex){
        dim_timer+=0.3f;
        
        StartCoroutine(hex.GetComponent<Hexa>().bursting());
        AudioManager.instance.Play("electro_start");
        yield return new WaitForSeconds(0.4f);
        GameObject t = Instantiate(effects[1],hex.transform.position,Quaternion.identity);
        //yield return new WaitForSeconds(0.2f);
        if (!exploded.Contains(hex)){
            exploded.Add(hex);
            
        }
        AudioManager.instance.Play("electro");
        Vector2Int pos = hex.GetComponent<Hexa>().pos;
        List<GameObject> target = new List<GameObject>();
        //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
        //score_text.GetComponent<Score_text>().update_points(50);
        Vector2Int grid = hex.GetComponent<Hexa>().pos;
        for (int k = 0;k<20;k++){
            for (int angle = 30;angle<360;angle += 60){
                Vector2Int newgrid = pix_to_grid(grid_to_pix(grid.x,grid.y)+ new Vector2(Mathf.Cos(Mathf.Deg2Rad*angle),Mathf.Sin(Mathf.Deg2Rad*angle))*0.94f*k);
                if (is_legit(newgrid)){
                    if (!target.Contains(hexagons[newgrid.x,newgrid.y])){
                        target.Add(hexagons[newgrid.x,newgrid.y]);
                    }
                }
            }
            
            
        }
        GameObject.Find("Level_handler").GetComponent<Handler>().update_score(50);
        
        dim_timer+=0.075f*target.Count;
        foreach (GameObject i in target){
            if (exploded.Contains(i))
                continue;
            if (invicible.Contains(i))
                continue;
            if (i.GetComponent<Hexa>().type == Hexa_Type.BOMB){
                detonate(i);
            }
            else if (i.GetComponent<Hexa>().type == Hexa_Type.LINE){
                StartCoroutine(lasering(i));
            }
            else if (i.GetComponent<Hexa>().type==Hexa_Type.WILD){
                detonate(i);
            }
            else{
                exploded.Add(i);
                //dimmed.Remove(i);
                i.GetComponent<Hexa>().explode();
                //GameObject t = Instantiate(effects[1],i.transform.position,Quaternion.identity);
            }
            
            //score_text.GetComponent<Score_text>().update_points(50);
            GameObject.Find("Level_handler").GetComponent<Handler>().update_score(25);
            AudioManager.instance.Play("gem_explode");
            
            yield return new WaitForSeconds(0.075f);
        }
        if (!invicible.Contains(hex)){
                hex.GetComponent<Hexa>().explode();
        }
    }
    IEnumerator color_clear(GameObject bomb, Sprite color){
        
        dim_timer+=0.3f;
        if (!exploded.Contains(bomb)){
            exploded.Add(bomb);
            if (!invicible.Contains(bomb)){
                bomb.GetComponent<Hexa>().explode();
                Instantiate(effects[2],bomb.transform.position,Quaternion.identity);
            }
        }
        dim_timer+=0.3f;
        //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
        //score_text.GetComponent<Score_text>().update_points(50);
        GameObject.Find("Level_handler").GetComponent<Handler>().update_score(50);
        yield return new WaitForSeconds(0.3f);
        //Vector2Int pos = bomb.GetComponent<Hexa>().pos;
        List<GameObject> target = new List<GameObject>();
        AudioManager.instance.Play("void");
        for (int i =0;i<boardWidth;i++){
            for (int j = 0; j<boardHeight;j++){
                if (!is_legit(new Vector2Int(i,j))){
                    continue;
                }
                if (exploded.Contains(hexagons[i,j])){
                    continue;
                }
                if (invicible.Contains(hexagons[i,j])){
                    continue;
                }
                if (hexagons[i,j].GetComponent<Hexa>().color != color){
                    continue;
                }
                if (hexagons[i,j].GetComponent<Hexa>().type == Hexa_Type.BOMB){
                    detonate(hexagons[i,j]);
                }
                else if (hexagons[i,j].GetComponent<Hexa>().type == Hexa_Type.LINE){
                    StartCoroutine(lasering(hexagons[i,j]));
                }
                else{
                    exploded.Add(hexagons[i,j]);
                    hexagons[i,j].GetComponent<Hexa>().explode();
                   // GameObject t = Instantiate(effects[1],hexagons[i,j].transform.position,Quaternion.identity);
                }
            //score_text.GetComponent<Score_text>().update_points(50);
            GameObject.Find("Level_handler").GetComponent<Handler>().update_score(50);
            AudioManager.instance.Play("gem_explode");
            }
        }
        dim_timer+=0.5f;
        //yield return new WaitForSeconds(3.0f);
    }
    
    IEnumerator clear_all(GameObject bomb, GameObject bomb2){
        
        dim_timer+=0.3f;
        if (!exploded.Contains(bomb)){
            exploded.Add(bomb);
            if (!invicible.Contains(bomb)){
                bomb.GetComponent<Hexa>().explode();
                Instantiate(effects[2],bomb.transform.position,Quaternion.identity);
            }
        }
        if (!exploded.Contains(bomb2)){
            exploded.Add(bomb2);
            if (!invicible.Contains(bomb2)){
                bomb.GetComponent<Hexa>().explode();
                Instantiate(effects[2],bomb2.transform.position,Quaternion.identity);
            }
        }
        dim_timer+=0.3f;
        //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
        //score_text.GetComponent<Score_text>().update_points(50);
        GameObject.Find("Level_handler").GetComponent<Handler>().update_score(50);
        AudioManager.instance.Play("void");
        yield return new WaitForSeconds(0.3f);
        //Vector2Int pos = bomb.GetComponent<Hexa>().pos;
        List<GameObject> target = new List<GameObject>();
        for (int i =0;i<boardWidth;i++){
            for (int j = 0; j<boardHeight;j++){
                if (!is_legit(new Vector2Int(i,j))){
                    continue;
                }
                if (exploded.Contains(hexagons[i,j])){
                    continue;
                }
                if (invicible.Contains(hexagons[i,j])){
                    continue;
                }
                if (hexagons[i,j].GetComponent<Hexa>().type == Hexa_Type.BOMB){
                    detonate(hexagons[i,j]);
                }
                else if (hexagons[i,j].GetComponent<Hexa>().type == Hexa_Type.LINE){
                    StartCoroutine(lasering(hexagons[i,j]));
                }
                else{
                    exploded.Add(hexagons[i,j]);
                    hexagons[i,j].GetComponent<Hexa>().explode();
                    //GameObject t = Instantiate(effects[1],hexagons[i,j].transform.position,Quaternion.identity);
                }
                //score_text.GetComponent<Score_text>().update_points(50);
                GameObject.Find("Level_handler").GetComponent<Handler>().update_score(50);
                AudioManager.instance.Play("gem_explode");
            }
        }
        dim_timer+=0.5f;
        yield return new WaitForSeconds(3.0f);
    }


    //hint and checking available matches
    public int[,] temp;
    public bool[,] visited;

    public void dfs(GameObject current, GameObject prev = null){
        if (!prev){
            temp[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y] = 1;
            visited[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y] = true;
        } else{
            temp[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y] = Mathf.Max(temp[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y],temp[prev.GetComponent<Hexa>().pos.x,prev.GetComponent<Hexa>().pos.y]+1);
            visited[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y] = true;
        }
        foreach (var k in current.GetComponent<Hexa>().get_all_surrounded()){
            if ((!visited[k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y])&&current.GetComponent<Hexa>().color==k.GetComponent<Hexa>().color){
                dfs(k,current);
            }
        }

        
    }
    int match_id = 0;
    public void bfs(GameObject obj){
        List<GameObject> query = new List<GameObject>();
        GameObject current;
        temp[obj.GetComponent<Hexa>().pos.x,obj.GetComponent<Hexa>().pos.y] = match_id++;
        matches.Add(new List<GameObject>());
        
        //GameObject prev;
        query.Add(obj);
        while (query.Count>0){
            current = query[0];
            query.RemoveAt(0);
            visited[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y] = true;
            matches[temp[obj.GetComponent<Hexa>().pos.x,obj.GetComponent<Hexa>().pos.y]].Add(current);
            foreach (var k in current.GetComponent<Hexa>().get_all_surrounded()){
            if ((!visited[k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y])&&current.GetComponent<Hexa>().color==k.GetComponent<Hexa>().color&&k.GetComponent<Hexa>().status!=STATE.LOCKED){
                temp[k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y] = temp[current.GetComponent<Hexa>().pos.x,current.GetComponent<Hexa>().pos.y];
                query.Add(k);
            }
        }
        }
    }
    bool void_locked(GameObject voidhex){
        foreach (var k in voidhex.GetComponent<Hexa>().get_all_surrounded()){
            if (k.GetComponent<Hexa>().status==STATE.FREE){
                return false;
            }
        }
        return true;
    }
    List<List<GameObject>> matches;
    public GameObject get_hint(){
        //return null;
        temp = new int[boardWidth, boardHeight];
        visited = new bool[boardWidth, boardHeight];
        match_id = 0;
        matches = new List<List<GameObject>>();
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                visited[i,j] = false;
                temp[i,j] = -1;
            }
        }
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                if (!visited[j,i]&&hexagons[j,i].GetComponent<Hexa>().type!=Hexa_Type.WILD&&hexagons[j,i].GetComponent<Hexa>().status!=STATE.LOCKED)
                    //dfs(hexagons[j,i]);
                    bfs(hexagons[j,i]);
            }
        }

        

        List<GameObject> available = new List<GameObject>();
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                //hexagons[i,j].GetComponentInChildren<TextMeshPro>().text = temp[i,j].ToString();
                if (hexagons[i,j].GetComponent<Hexa>().type==Hexa_Type.WILD&&!void_locked(hexagons[i,j])){
                    available.Add(hexagons[i,j]);
                }
            }
        }
        foreach (var i in matches){
            if (i.Count>=3){
                available.Add(i[0]);
            }
        }
        Debug.Log(available.Count);
        if (available.Count==0){
            //Debug.Log("no more moves");
            return null;
        }
        //Debug.Log(available.Count);
        return available[Mathf.FloorToInt(Random.Range(0,available.Count))];
    }
    int count_locks(){
        int res = 0;
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (is_legit(new Vector2Int(i,j))&&hexagons[i,j].GetComponent<Hexa>().lock_ins){
                    res++;
                }
            }
        }
        return res;
    }
    public void lock_create(){
        //int max_locks = Random.Range(0,(Handler.level-3)/(Handler.level-3));

        if (Random.Range(0,105-GameObject.Find("Level_handler").GetComponent<Handler>().difficulty)<=0){
            for (int t = 0; t <= boardHeight*boardWidth;t++){
                int i = Random.Range(0,boardWidth);
                int j = Random.Range(0,boardHeight);
                if (!is_legit(new Vector2Int(i,j))){
                    continue;
                }
                if (hexagons[i,j].GetComponent<Hexa>().lock_ins==null && hexagons[i,j].GetComponent<Hexa>().type == Hexa_Type.NORMAL){
                    //Debug.Log(121);
                    hexagons[i,j].GetComponent<Hexa>().lock_create();
                    return;
                }

            }
        }
        }

    public void update_lock(){
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (is_legit(new Vector2Int(i,j))&&hexagons[i,j].GetComponent<Hexa>().lock_ins&&hexagons[i,j].GetComponent<Hexa>().status== STATE.FREE){
                    hexagons[i,j].GetComponentInChildren<HexLock>().lock_counter();
                }
            }
        }
    }
    public void check_lock(){
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (is_legit(new Vector2Int(i,j))&&hexagons[i,j].GetComponent<Hexa>().lock_ins&&hexagons[i,j].GetComponent<Hexa>().status== STATE.FREE){
                    hexagons[i,j].GetComponentInChildren<HexLock>().lock_check();
                }
            }
        }
    }
}

