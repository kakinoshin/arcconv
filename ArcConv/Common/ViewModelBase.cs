using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Modules.Views.Common
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region implementation of notification to parent object
        //=================================================================
        // !!! Owner object should be inherited INotifyPropertyChanged !!!
        //=================================================================
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
