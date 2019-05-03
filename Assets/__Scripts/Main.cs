using System.Collections;           
using System.Collections.Generic;  
using UnityEngine;              
using UnityEngine.SceneManagement;  

public class Main : MonoBehaviour {
    static public Main S;
	static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Set in Inspector")]
    public GameObject[]     prefabEnemies;             //enemy prefabs
    public float             enemySpawnPerSecond = 0.5f; // # Enemies/second
    public float             enemyDefaultPadding = 1.5f; 
	public WeaponDefinition[]    weaponDefinitions; 
    public GameObject            prefabPowerUp;                             // a 
    public WeaponType[]          powerUpFrequency = new WeaponType[] {      // b 
                                    WeaponType.blaster, WeaponType.blaster, 
                                    WeaponType.spread,  WeaponType.shield }; 

    private BoundsCheck     bndCheck; 

    public void shipDestroyed( Enemy e ) {                                   // c 
        // Potentially generate a PowerUp 
        if (Random.value <= e.powerUpDropChance) {                           // d 
            // Choose which PowerUp to pick 
            // Pick one from the possibilities in powerUpFrequency 
            int ndx = Random.Range(0,powerUpFrequency.Length);               // e 
            WeaponType puType = powerUpFrequency[ndx]; 

            // Spawn a PowerUp 
            GameObject go = Instantiate( prefabPowerUp ) as GameObject; 
            PowerUp pu = go.GetComponent<PowerUp>(); 
            // Set it to the proper WeaponType 
            pu.SetType( puType );                                            // f 

            // Set it to the position of the destroyed ship 
            pu.transform.position = e.transform.position;
        } 
    }


    void Awake() {
        S = this;
   
        bndCheck = GetComponent<BoundsCheck>();       
        Invoke( "SpawnEnemy", 1f/enemySpawnPerSecond );
		// A generic Dictionary with WeaponType as the key 
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();         
        foreach( WeaponDefinition def in weaponDefinitions ) {              
            WEAP_DICT[def.type] = def; 
    }
}
    public void SpawnEnemy() {
        
        int ndx = Random.Range(0, prefabEnemies.Length);                    
        GameObject go = Instantiate<GameObject>( prefabEnemies[ ndx ] );    

        // Position enemy above screen with random x val
        float enemyPadding = enemyDefaultPadding;                           
        if (go.GetComponent<BoundsCheck>() != null) {                        
            enemyPadding = Mathf.Abs( go.GetComponent<BoundsCheck>().radius );
        }

        // init position for enemy                 
        Vector3 pos = Vector3.zero;              
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax =  bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range( xMin, xMax );
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        // spawn another enemy
        Invoke( "SpawnEnemy", 1f/enemySpawnPerSecond );                      
    }

	public void DelayedRestart( float delay ) {
        // Invoke the Restart() method in the variable delay and seconds
        Invoke( "Restart", delay );
    }

    public void Restart() {
        // restarts game based on scene
        SceneManager.LoadScene( "Scene01");
    }

	static public WeaponDefinition GetWeaponDefinition( WeaponType wt ) {          // a 
         // Check to make sure that the key exists in the Dictionary 
         // Attempting to retrieve a key that didn't exist, would throw an error, 
         // so the following if statement is important. 
         if (WEAP_DICT.ContainsKey(wt)) {                                           // b 
             return( WEAP_DICT[wt] ); 
         } 
        // This returns a new WeaponDefinition with a type of WeaponType.none, 
        //   which means it has failed to find the right WeaponDefinition 
         return( new WeaponDefinition() );                                          // c 
     } 
}
