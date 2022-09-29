using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// Numerical information about a pawn
    /// </summary>
    public class Statistic
    {
        public enum Type
        {
            Strength,
            Vitality,
            Speed,
        }
        public Statistic(Type type, int value)
        {
            this.type = type;
            this.Value = value;
        }
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = Mathf.Max(value, 1);  // Value cannot be less than one
            }
        }
        public Type type;
        int value;
        public string GetName()
        {
            switch (type)
            {
                case Type.Strength:
                    return "Strength";
                case Type.Vitality:
                    return "Vitality";
            }

            return type.ToString();
        }
    }
}
