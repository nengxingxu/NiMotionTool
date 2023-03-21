using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiMotion.Model
{
    public class SystemSettingPropertyModel: NotificationBinding
    {
        private bool enableLog;
        [Category("Settings")]
        public bool EnableLog
        {
            get
            {
                return enableLog;
            }
            set
            {
                enableLog = value;
                RaisePropertyChanged("EnableLog");
            }
        }

        private language _language;
        [Category("Settings")]
        public language Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
                RaisePropertyChanged("Language");
            }
        }

        private theme _theme;
        [Category("Settings")]
        public theme Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                RaisePropertyChanged("Theme");
            }
        }

        private int refreshFrequency;
        [Category("Settings")]
        public int RefreshFrequency
        {
            get
            {
                return refreshFrequency;
            }
            set
            {
                refreshFrequency = value;
                RaisePropertyChanged("RefreshFrequency");
            }
        }

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
