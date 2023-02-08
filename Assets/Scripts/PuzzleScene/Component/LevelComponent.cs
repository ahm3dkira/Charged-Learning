using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{

    public class Quantity
    {
        public int Total { get; private set; }
        public int Used { get; set; }

        public Quantity(int total, int used = 0)
        {
            this.Total = total;
            this.Used = used;
        }
    }
    public class Property
    {
        public bool isRevealed;
        public PureProperty properties { get; private set; }

        public Property(PureProperty pureProperty, bool isRevealed = false)
        {
            this.properties = pureProperty;
            this.isRevealed = isRevealed;
        }
    }

    public class LevelComponent
    {
        public Component Component { get; private set; }

        public static Action<LevelComponent> quantityChanged;

        public Dictionary<string, Property> Properties { get; private set; }

        // public Dictionary<string, Terminal> Terminals { get; private set; }

        public Quantity Quantity { get; private set; }

        public string Name { get; set; }



        public LevelComponent(Component component, Quantity quantity)
        {
            Properties = new Dictionary<string, Property>();
            this.Component = component;
            this.Quantity = quantity;

            foreach (var prop in component.Properties)
            {
                Properties.Add(prop.Value.name, new Property(prop.Value));
            }
            ComponentManager.created += HandleComponentCreated;
            ComponentManager.destroyed += HandleComponentDestroyed;
        }

        // TODO: handle reveal event and qunatity change event
        private void HandleComponentCreated(ComponentBehavior comp)
        {
            if (comp.levelComponent == this)
            {
                if (Quantity.Used < Quantity.Total)
                {
                    Quantity.Used++;
                    quantityChanged.Invoke(this);
                }
                else
                {
                    throw new Exception("error");
                }

            }
        }

        // TODO: handle reveal event and qunatity change event
        private void HandleComponentDestroyed(ComponentBehavior comp)
        {
            if (comp.levelComponent == this)
            {
                if (Quantity.Used >= 1)
                {
                    Quantity.Used--;
                    quantityChanged.Invoke(this);
                }
                else
                {
                    Debug.Log(Quantity);
                    Debug.Log(Quantity.Used);
                    throw new Exception("error");
                }

            }
        }

        // public bool 
    }
}
