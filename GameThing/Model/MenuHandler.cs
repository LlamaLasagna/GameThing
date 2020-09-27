using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameThing
{
    public static class MenuHandler
    {
        // PROPERTIES

        public static EventHandler HomeAction;
        public static EventHandler ConfirmAction;
        public static EventHandler CancelAction;
        public static EventHandler UpAction;
        public static EventHandler DownAction;
        public static EventHandler RightAction;
        public static EventHandler LeftAction;


        // METHODS

        private static void InvokeAction(EventHandler action)
        {
            action?.Invoke(null, EventArgs.Empty);
        }


        public static void Home()
        {
            InvokeAction(HomeAction);
        }


        public static void Confirm()
        {
            InvokeAction(ConfirmAction);
        }


        public static void Cancel()
        {
            InvokeAction(CancelAction);
        }


        public static void Up()
        {
            InvokeAction(UpAction);
        }


        public static void Down()
        {
            InvokeAction(DownAction);
        }


        public static void Right()
        {
            InvokeAction(RightAction);
        }


        public static void Left()
        {
            InvokeAction(LeftAction);
        }


    }
}
