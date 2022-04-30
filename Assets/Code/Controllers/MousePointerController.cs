using UnityEngine;

public class MousePointerController : BaseController, IPointerController
{
//    public UIEventController UIEvent { get; set; }


    //public MousePointerController(UIEventController EventClass)
    //{
    //    UIEvent = EventClass;
    //}
    private void Update()
    {
        GetPointerUpdate();
    }

    private void GetPointerUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            EventController.DownPointer(Input.mousePosition);
        }

        if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
        {
            EventController.SwipePointer(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EventController.UpPointer(Input.mousePosition);
        }
    }


  
}
