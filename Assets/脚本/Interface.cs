using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public interface IDiggable
    {
        void Dig();
        
    }
    public interface IResourcable
    {
        void Collect();
        bool IsCollectable { get; set; }
        int Quantity { get; set; }
    }
    public interface IAttack
    {
        void Attack();
    }
    public interface IMovable
    {
        void Move(Vector2 direction);
    }
    public interface IInteractable
    {
        void Interact();
    }

    public interface ICollectable
    {
        void Collect();
    }
    public interface IHealth
    {
        void Health();
    }
    public interface IDeath
    {
        void Death();

    }
    public interface IDroppable
    {
        void Drop();
    }
    interface IJumpable
    {
        void Jump();
    }
    public interface IDamageable
    {
        void TakeDamage(int damage);
    }

    public interface ITargetable
    {
        void SetTarget(GameObject target);
        GameObject GetTarget();
    }

}
