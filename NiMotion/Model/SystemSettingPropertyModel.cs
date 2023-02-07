using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiMotion.Model
{
    public class SystemSettingPropertyModel
    {
        [Category("Settings")]
        public bool EnableLog { get; set; }

        [Category("Settings")]
        public language Language { get; set; }

        [Category("Settings")]
        public theme Theme { get; set; }

        [Category("Settings")]
        public int RefreshFrequency {get; set;}

        public enum language
        {
            Chinese,
            English
        };

        public enum theme
        {
            White,
            Dark
        };
    }
}
