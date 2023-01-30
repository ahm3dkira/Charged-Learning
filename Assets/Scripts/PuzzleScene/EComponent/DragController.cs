using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EComponent
{
    public partial class Controller
    {
        public class Drag : Singleton<Drag>
        {
            private void Start()
            {
                EComponent.mouseDown += SetCursorLastSeen;
                EComponent.dragged += MoveSelectedObjectsOnDrag;
            }

            Vector3 lastSeen;

            void SetCursorLastSeen(ComponentBehavior component)
            {
                lastSeen = Utils.GetMouseWorldPosition();
            }

            void MoveSelectedObjectsOnDrag(ComponentBehavior component)
            {
                var currMousePos = Utils.GetMouseWorldPosition();

                foreach (var obj in Selection.GetSelectedComponents())
                {
                    obj.Move(currMousePos - lastSeen);
                }
                lastSeen = currMousePos;
            }
        }
    }
}