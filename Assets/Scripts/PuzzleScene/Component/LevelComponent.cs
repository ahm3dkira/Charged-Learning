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
        public PureProperty pureProperty { get; private set; }

        public Property(PureProperty pureProperty, bool isRevealed = false)
        {
            this.pureProperty = pureProperty;
            this.isRevealed = isRevealed;
        }
    }

    public class LevelComponent
    {

        public Component Component { get; private set; }
        public static Action<LevelComponent> created;

        public static Action<LevelComponent> quantityChanged, propertyRevealed;

        public Dictionary<PropertyType, Property> Properties { get; private set; }

        // public Dictionary<string, Terminal> Terminals { get; private set; }

        public Quantity Quantity { get; private set; }

        public string Name { get; set; }



        public LevelComponent(Component component, Quantity quantity, string name)
        {
            Properties = new Dictionary<PropertyType, Property>();
            this.Component = component;
            this.Quantity = quantity;
            this.Name = name;

            foreach (var prop in component.Properties)
            {
                Properties.Add(prop.Value.propertyType, new Property(prop.Value));
            }
            LiveComponent.created += HandleComponentCreated;
            LiveComponent.destroyed += HandleComponentDestroyed;
            created?.Invoke(this);
        }

        public void AddProperty(PureProperty pureProperty){
            Component.AddProperty(pureProperty);
            Properties.Add(pureProperty.propertyType, new Property(pureProperty));
        }

        // TODO: handle reveal event and qunatity change event
        private void HandleComponentCreated(LiveComponent comp)
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
        private void HandleComponentDestroyed(EditorBehaviour comp)
        {
            if (comp.GetComponent<LiveComponent>().levelComponent == this)
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

        public void RevealProperty(PropertyType propertyType)
        {
            var propertyName = Enum.GetName(typeof(PropertyType), propertyType);
            if(!Properties.ContainsKey(propertyType)){
                Debug.Log($"{propertyName} property doesn't exist for this component");
                return;
            }

            var propertyIsRevealed = Properties[propertyType].isRevealed;
            if (propertyIsRevealed){
                Debug.Log($"{propertyName} has been already revealed");
                return;
            }
            Properties[propertyType].isRevealed = true;
            Debug.Log("property revealed!");
            propertyRevealed?.Invoke(this);
        }
    }
}
