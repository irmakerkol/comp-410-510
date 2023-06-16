using UnityEngine;

public class MovingObstacle : Obstacle
{
    protected Transform player;
    protected Transform movingObstacle;

    protected virtual void Start()
    {
        this.movingObstacle = transform;
        player = GameObject.FindObjectOfType<Player>().myTransform;
    }

    protected virtual void Update()
    {
        if (movingObstacle.position.z - player.position.z < 50f)
        {
            movingObstacle.Translate(0, 0, -3f * Time.deltaTime);
        }

     
    }
}