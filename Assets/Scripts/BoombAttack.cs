using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BoombAttack : MonoBehaviour
{
    public GameObject airplane;
    public GameObject bomb;
    public GameObject spawnPoint;
    float x_airplane;
    float x_0_airplane;
    public float v_0_airplane;
    float y_airplane;
    float y_0_airplane;
    float angle_airplane;

    bool Go;
    float startTime;
    ShootBomb shootBomb;
    List<ShootBomb> PoolShootBomb;
    public GameObject point;
    public List<GameObject> BigPool;
    public List<MountlyLine> MountlyLines;
    public List<GameObject> PoolPoint_smoll;
    public ParticleSystem explosion;

    void Start()
    {
        Go = false;
        x_0_airplane = airplane.transform.position.x;
        y_0_airplane = airplane.transform.position.y;
        PoolShootBomb = new List<ShootBomb>();
        BigPool = new List<GameObject>();
        MountlyLines = new List<MountlyLine>();

        for (int i = 0; i < PoolPoint_smoll.Count - 1; i++)
        {
            MountlyLines.Add(new MountlyLine(PoolPoint_smoll[i].transform.position.x, PoolPoint_smoll[i].transform.position.y,
            PoolPoint_smoll[i + 1].transform.position.x, PoolPoint_smoll[i + 1].transform.position.y, point));
        }

        for (int k = 0; k < MountlyLines.Count; k++)
        {
            for (int i = 0; i < MountlyLines[k].PoolPoint.Count; i++)
            {
                BigPool.Add(MountlyLines[k].PoolPoint[i]);
            }
        }
    
    }

    void Update()
    {

        x_airplane = x_0_airplane + v_0_airplane * (Time.time - startTime) * Mathf.Cos(angle_airplane * Mathf.PI / 180);
        y_airplane = y_0_airplane + v_0_airplane * (Time.time - startTime) * Mathf.Sin(angle_airplane * Mathf.PI / 180);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            angle_airplane += 2f;
            startTime = Time.time;
            x_0_airplane = x_airplane;
            y_0_airplane = y_airplane;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            angle_airplane -= 2f;
            startTime = Time.time;
            x_0_airplane = x_airplane;
            y_0_airplane = y_airplane;
        }
 

        airplane.transform.localEulerAngles = new Vector3(0, 0, angle_airplane);
        airplane.transform.position = new Vector3(x_airplane, y_airplane, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Go = true;
            shootBomb = new ShootBomb(angle_airplane, v_0_airplane, spawnPoint.transform.position.x, spawnPoint.transform.position.y, bomb,explosion);
            PoolShootBomb.Add(shootBomb);
        }

        if (Go)
        {
            for (int i = 0; i < PoolShootBomb.Count; i++)
            {
                PoolShootBomb[i].Bomb.transform.position = new Vector3(PoolShootBomb[i].X_0 + PoolShootBomb[i].V_0 * (Time.time - PoolShootBomb[i].StartTime) * Mathf.Cos(PoolShootBomb[i].start_angle_bomb * Mathf.PI / 180),
                PoolShootBomb[i].Y_0 + PoolShootBomb[i].V_0 * (Time.time - PoolShootBomb[i].StartTime) * Mathf.Sin(PoolShootBomb[i].start_angle_bomb * Mathf.PI / 180) - 1f * ((Time.time - PoolShootBomb[i].StartTime) * (Time.time - PoolShootBomb[i].StartTime)) / 2, 0);

                for (int k = 0; k < BigPool.Count; k++)
                {
                    if (PoolShootBomb[i].Bomb.transform.position.y <= BigPool[k].transform.position.y + 0.5f &&
                        PoolShootBomb[i].Bomb.transform.position.y >= BigPool[k].transform.position.y - 0.5f
                        && (PoolShootBomb[i].Bomb.transform.position.x <= BigPool[k].transform.position.x + 0.5f &&
                        PoolShootBomb[i].Bomb.transform.position.x >= BigPool[k].transform.position.x - 0.5f)
                        )
                    {
                        PoolShootBomb[i].Explosion.Play();
                        Destroy(PoolShootBomb[i].Bomb,0.5f);
                        PoolShootBomb.Remove(PoolShootBomb[i]);
                        break;
                    }
                }
            }
        }

    }
    public class ShootBomb
    {
        public float start_angle_bomb;
        public float V_0;
        public float X_0;
        public float Y_0;
        public float StartTime;
        public GameObject Bomb;
        public ParticleSystem Explosion;
        public ShootBomb(float _angle_airplane, float _V_0, float _X_0, float _Y_0, GameObject _Bomb, ParticleSystem _Explosion)
        {
            start_angle_bomb = _angle_airplane;
            V_0 = _V_0;
            X_0 = _X_0;
            Y_0 = _Y_0;
            Bomb = Instantiate(_Bomb);
            Explosion = Instantiate(_Explosion);            
            StartTime = Time.time;
            Bomb.transform.localEulerAngles = new Vector3(0, 0, start_angle_bomb);
            Explosion.transform.SetParent(Bomb.transform);
        }
    }
    public class MountlyLine
    {
        public List<GameObject> PoolPoint;
        GameObject point;

        public MountlyLine(float _x_1, float _y_1, float _x_2, float _y_2, GameObject _point)
        {

            PoolPoint = new List<GameObject>();//
            float x_1 = _x_1;
            float y_1 = _y_1;
            float x_2 = _x_2;
            float y_2 = _y_2;
            float d_x;
            if (x_2 - x_1 == 0)
            {
                d_x = 0.00001f;
            }
            else
            {
                d_x = x_2 - x_1;
            }
            float k = (y_2 - y_1) / (d_x);
            float b = (y_1 * x_2 - y_2 * x_1) / (d_x);
            for (float x = x_1; x <= x_2; x += 0.1f)
            {
                point = Instantiate(_point);
                PoolPoint.Add(point);
                point.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            int i = 0;
            for (float x = x_1; x <= x_2; x += 0.1f)
            {
                float y = k * x + b;
                {
                    PoolPoint[i].transform.position = new Vector3(x, y, 0);
                }
                i++;
            }


        }
    }

}

