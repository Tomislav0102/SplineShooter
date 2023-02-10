using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstCollection
{
    public class HelperScript 
    {
        public Vector3 MousePoz(Camera cam, LayerMask lay)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, lay))
            {
                return hit.point;
            }
            else return Vector3.zero;
        }

        public static T[] GetAllChildernByType<T>(Transform par)
        {
            T[] tip = new T[par.childCount];
            for (int i = 0; i < par.childCount; i++)
            {
                tip[i] = par.GetChild(i).GetComponent<T>();
            }
            return tip;
        }
    }
    public interface ITakeDamage
    {
        void TakeDamage(Faction attackerFaction, int dam);
    }
    public interface IActivation
    {
        bool IsActive { get; set; }
    }
    public enum TileState
    {
        Open,
        Closed,
        Danger
    }
    public enum Faction
    {
        Ally,
        Enemy,
        Neutral
    }
    public enum ProjectileType
    {
        Blue,
        Yellow,
        Red
    }
    public enum MotionType
    {
        Rotation,
        RotateGroup,
        Translation
    }
    public enum ShootingMode
    {
        NoBullets,
        OneBullet,
        TwoBullets,
        ThreeBullets
    }
    public enum PUtype
    {
        Diamond,
        Fuel,
        Ammo,
        Random
    }
    public enum ConditionForActivation //used for special cases
    {
        BossKilled
    }
}
