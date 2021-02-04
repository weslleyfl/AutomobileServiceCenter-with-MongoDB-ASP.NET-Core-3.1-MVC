using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Utilities.Navigation
{
    /// <summary>
    /// Classe NavigationMenuItem que se assemelha/espelha à estrutura JSON
    /// </summary>
    public class NavigationMenuItem
    {
        public string DisplayName { get; set; }
        public string MaterialIcon { get; set; }
        public string Link { get; set; }
        public bool IsNested { get; set; }
        public int Sequence { get; set; }
        public List<string> UserRoles { get; set; }
        public List<NavigationMenuItem> NestedItems { get; set; }
    }
}
