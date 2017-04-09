using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoNote.Views;
using Windows.UI.Xaml.Controls;

namespace VideoNote.Models
{
    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }

        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Home, Name = "Home", PageType = typeof(HomePage) });
            items.Add(new MenuItem() { Icon = Symbol.Setting, Name = "Setting", PageType = typeof(SettingPage) });
            return items;
        }

        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Help, Name = "About", PageType = typeof(AboutPage) });
            return items;
        }
    }
}
