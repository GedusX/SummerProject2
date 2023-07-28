using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class board : MonoBehaviour
{
    
    // The width of the board in cells
    public int boardWidth = 8;
    // The height of the board in cells
    public int boardHeight = 8;
    // The prefab of an object
    public GameObject hexagon;

    public GameObject hexagon_unit;

    public bool idle;

    public float dim_timer;

    // The 2D array to store the objects, while i represents for col (x), and j represents for row (y);
    private GameObject[,] hexagon_slots;
    

    public GameObject[,] hexagons;

    public GameObject pointstext;

    public int difficulty;

    // The width of an object
    public float hexWidth = 1.5f;
    // The height of an object
    public float hexHeight = 1.5f;

    public List<GameObject> invicible;
    
    public List<GameObject> exploded;
    // Start is called before the first frame update
    void Start()
    {
        // Create the board
        CreateBoard();
        invicible = new List<GameObject>();
        exploded = new List<GameObject>();
    }

    // Update is called once per frame
    public bool legal_move = false;
    void Update()
    {
        dim_timer-=Time.deltaTime;
        if (dim_timer<0)
            dim_timer = 0;
        //Debug.Log(get_hint());
        for (int i = 0;i<boardWidth;i++){
            for (int j = 0; j<boardHeight;j++){
                if (hexagons[i,j])
                    hexagons[i,j].GetComponent<Hexa>().pos = new Vector2Int(i,j);
            }
        }
        legal_move = (drag_list.Count>=3||(drag_list.Count==2&&drag_list[0].GetComponent<Hexa>().type == Hexa_Type.WILD));
        if (Input.GetMouseButtonUp(0) && drag_list.Count>0 ){
            invicible.Clear();
            exploded.Clear();
            if (drag_list.Count==2 && drag_list[0].GetComponent<Hexa>().type==Hexa_Type.WILD){
                if (drag_list[1].GetComponent<Hexa>().type==Hexa_Type.WILD){
                    clear_all(drag_list[0]);
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
                //Debug.Log(drag_list.Count);
                Hexa_Type got = Hexa_Type.NORMAL;
                if (drag_list.Count>=7){
                    got = Hexa_Type.WILD;
                    AudioManager.instance.Play("voidhex");
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                    //score_text.GetComponent<Score_text>().update_points(500);
                    GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(500);
                }
                else if (drag_list.Count>=6){
                    got = Hexa_Type.LINE;
                    AudioManager.instance.Play("starhex");
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                   // score_text.GetComponent<Score_text>().update_points(200);
                    GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(200);
                }
                else if (drag_list.Count>=5){
                    got = Hexa_Type.BOMB;
                    AudioManager.instance.Play("powerhex");
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                    //score_text.GetComponent<Score_text>().update_points(150);
                    GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(150);
                }
                if (got!=Hexa_Type.NORMAL){
                    invicible.Add(drag_list[0]);
                    for (int k = drag_list.Count-1;k>0;k--){
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
                            drag_list[k].GetComponent<Hexa>().combining();
                        }

                        
                    }
                    drag_list[0].GetComponent<Hexa>().change_to(got);
                }
                else{
                    //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
                    //score_text.GetComponent<Score_text>().update_points(50+50*(drag_list.Count-3));
                    GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50+50*(drag_list.Count-3));
                    AudioManager.instance.Play("gotcha");
                    foreach (GameObject i in drag_list){
                        if (i.GetComponent<Hexa>().type == Hexa_Type.BOMB){
                            detonate(i);
                        }
                        else if (i.GetComponent<Hexa>().type == Hexa_Type.LINE){
                            StartCoroutine(lasering(i));
                        }
                        else{
                            i.GetComponent<Hexa>().dimming();
                        }
                    }
                }
                dim_timer+=0.3f;
                StartCoroutine(hex_falling());
            }
            else{
                AudioManager.instance.Play("badmove");
                drag_list.Clear();
            }
        }
    }
    
    
    // Create the board with objects

    Vector3 grid_to_pix(int i, int j){
        float xOffset = hexWidth * 0.85f;
        float yOffset = hexHeight * 0.45f;
        float x = i * xOffset;
        float y = j * hexHeight + (i % 2) * yOffset;
        return transform.position + new Vector3(x,y,0);
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
        
        return ((Mathf.Round((obj2.transform.position-obj1.transform.position).magnitude))<=hexHeight);

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
                if (drag_list[0].GetComponent<Hexa>().type==Hexa_Type.WILD){
                    if (drag_list.Count>=2){
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
    }
    //falling
    
    IEnumerator hex_falling(){
        
        yield return new WaitUntil(() => (dim_timer<=0));
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
                        hexagons[target_pos.x,target_pos.y].transform.DOMove(grid_to_pix(target_pos.x,target_pos.y),0.2f);
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
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
        for (int i = 0;i<=9999-difficulty;i++){
            if (get_hint()){
                break;
            }
            else{
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
            GameObject.Find("Level_handler").GetComponent<ClassicHandler>().gameover();
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
        GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(20);
        for (int i = hex.GetComponent<Hexa>().pos.x-1;i<hex.GetComponent<Hexa>().pos.x+2;i++){
            for (int j = hex.GetComponent<Hexa>().pos.y-1;j<hex.GetComponent<Hexa>().pos.y+2;j++){
                
                if (!is_legit(new Vector2Int(i,j)))
                    continue;
                //Debug.Log(exploded.Count);
                if (exploded.Contains(hexagons[i,j]))
                    continue;
                if (invicible.Contains(hexagons[i,j]))
                    continue;
                //Debug.Log("hehe");
                if (hexagons[i,j].GetComponent<Hexa>().type==Hexa_Type.BOMB){
                    StartCoroutine(delay_bomb(0.1f,hexagons[i,j]));
                }
                else if (hexagons[i,j].GetComponent<Hexa>().type==Hexa_Type.LINE){
                    StartCoroutine(lasering(hexagons[i,j]));
                }
                else if (hexagons[i,j].GetComponent<Hexa>().type==Hexa_Type.WILD){
                    detonate(hexagons[i,j]);
                }
                else{
                    //Debug.Log("hehe");
                    exploded.Add(hexagons[i,j]);
                    hexagons[i,j].GetComponent<Hexa>().explode();
                    Instantiate(effects[0],hexagons[i,j].transform.position,Quaternion.identity);
                }
                //score_text.GetComponent<Score_text>().update_points(20);
                GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(20);
            }
        }
    }

    IEnumerator lasering(GameObject hex){
        dim_timer+=0.3f;
        if (!exploded.Contains(hex)){
            exploded.Add(hex);
            if (!invicible.Contains(hex)){
                hex.GetComponent<Hexa>().explode();
                Instantiate(effects[1],hex.transform.position,Quaternion.identity);
            }
        }
        AudioManager.instance.Play("electro");
        Vector2Int pos = hex.GetComponent<Hexa>().pos;
        List<GameObject> target = new List<GameObject>();
        //GameObject score_text = Instantiate(pointstext,drag_list[0].transform.position,Quaternion.identity);
        //score_text.GetComponent<Score_text>().update_points(50);
        GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50);
        for (int k = 0;k<Mathf.Max(boardHeight,boardWidth);k++){
            if (is_legit(pos+new Vector2Int(0,k))){
                target.Add(hexagons[pos.x,pos.y+k]);
                
            }
            if (is_legit(pos+new Vector2Int(k,0))){
                target.Add(hexagons[pos.x+k,pos.y]);
                
            }
            if (is_legit(pos+new Vector2Int(0,-k))){
                target.Add(hexagons[pos.x,pos.y-k]);
               
            }
            if (is_legit(pos+new Vector2Int(-k,0))){
                target.Add(hexagons[pos.x-k,pos.y]);
                
            }
        }
        dim_timer+=0.6f;
        foreach (GameObject i in target){
            if (exploded.Contains(i))
                continue;
            if (invicible.Contains(i))
                continue;
            if (i.GetComponent<Hexa>().type == Hexa_Type.BOMB){
                detonate(i);
            }
            else if (i.GetComponent<Hexa>().type == Hexa_Type.LINE){
                lasering(i);
            }
            else if (i.GetComponent<Hexa>().type==Hexa_Type.WILD){
                detonate(i);
            }
            else{
                exploded.Add(i);
                i.GetComponent<Hexa>().explode();
                GameObject t = Instantiate(effects[1],i.transform.position,Quaternion.identity);
            }
            
            //score_text.GetComponent<Score_text>().update_points(50);
            GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50);
            AudioManager.instance.Play("gem_explode");
            yield return new WaitForSeconds(0.02f);
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
        GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50);
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
                    lasering(hexagons[i,j]);
                }
                else{
                    hexagons[i,j].GetComponent<Hexa>().explode();
                   // GameObject t = Instantiate(effects[1],hexagons[i,j].transform.position,Quaternion.identity);
                }
            //score_text.GetComponent<Score_text>().update_points(50);
            GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50);
            AudioManager.instance.Play("gem_explode");
            }
        }
        dim_timer+=0.5f;
        yield return new WaitForSeconds(3.0f);
    }
    public GameObject get_hint(){
        int[,] temp = new int[boardWidth, boardHeight];
        bool[,] visited = new bool[boardWidth, boardHeight];
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                temp[i,j] = 0;
                visited[i,j] = false;
            }
        }
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (temp[i,j]==0){
                    temp[i,j] = 1;
                    visited[i,j] = true;
                }
                foreach (var k in hexagons[i,j].GetComponent<Hexa>().get_all_surrounded()){
                    if (!visited[k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y]&&hexagons[i,j].GetComponent<Hexa>().color==k.GetComponent<Hexa>().color){
                        temp[k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y] = temp[i,j]+1;
                        visited[k.GetComponent<Hexa>().pos.x,k.GetComponent<Hexa>().pos.y] = true;
                    }
                }
            }
        }
        
        List<GameObject> available = new List<GameObject>();
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (hexagons[i,j].GetComponent<Hexa>().type==Hexa_Type.WILD){
                    available.Add(hexagons[i,j]);
                }
                else if (temp[i,j]>=3) {
                    available.Add(hexagons[i,j]);
                }
            }
        }
        if (available.Count==0){
            //Debug.Log("no more moves");
            return null;
        }
        //Debug.Log(available.Count);
        return available[Mathf.FloorToInt(Random.Range(0,available.Count))];
    }
    IEnumerator clear_all(GameObject bomb){
        
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
        GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50);
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
                    lasering(hexagons[i,j]);
                }
                else{
                    hexagons[i,j].GetComponent<Hexa>().explode();
                    GameObject t = Instantiate(effects[1],hexagons[i,j].transform.position,Quaternion.identity);
                }
                //score_text.GetComponent<Score_text>().update_points(50);
                GameObject.Find("Level_handler").GetComponent<ClassicHandler>().update_score(50);
                AudioManager.instance.Play("gem_explode");
            }
        }
        dim_timer+=0.5f;
        yield return new WaitForSeconds(3.0f);
    }
}

