using System.Drawing;
using System.Linq;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF
{
    public static class UserUtil
    {
        public static bool UserHasRole(RoleTypes role)
        {
            return Singleton.User.Roles.Any(u => u.Role.RoleDescriptionShort == role.ToString());
        }
    }

    public static class NotifyUtility
    {
        public static void ShowCustomBalloon(string title, string text, int duration)
        {
            try
            {
                var tbi = new TaskbarIcon { ToolTipText = "PinnaFace", Icon = new Icon("Resources\\AppIcon.ico") };

                var balloon = new FancyBalloon { TxtTitle = { Text = title }, TxtText = { Text = text } };

                tbi.ShowCustomBalloon(balloon, PopupAnimation.Slide, duration);
            }
            catch
            {
                
            }

            //tbi.Dispose();
        }
    }
}