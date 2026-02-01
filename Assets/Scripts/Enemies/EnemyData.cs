using UnityEngine;
 
[CreateAssetMenu(menuName = "Game/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string displayName = "Enemy";
    public float maxHP = 5f;
    public float moveSpeed = 2f; //prova modificare
}
