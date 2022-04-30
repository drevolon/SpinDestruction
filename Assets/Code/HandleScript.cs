using UnityEngine;

using UnityEngine.EventSystems;

public class HandleScript : MonoBehaviour, IDragHandler, IBeginDragHandler

{

    Transform green;   // здесь будет ссылка на компонент Transform зеленой панели.

    void Start()

    {

        green = transform.GetChild(0); // получаем ссылку на Transform зеленой панели.

    }

    public void OnBeginDrag(PointerEventData eventData)

    {

        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))

        {

            if (eventData.delta.x > 0) Debug.Log("Right");

            else Debug.Log("Left");

            green.position += new Vector3(eventData.delta.x, 0, 0);

        }

        else

        {

            if (eventData.delta.y > 0) Debug.Log("Up");

            else Debug.Log("Down");

            green.position += new Vector3(0, eventData.delta.y, 0);

        }

    }

    public void OnDrag(PointerEventData eventData)

    {

    }

}