using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BeaverSoft.Common.UI
{
    public static class XAMLTreeHelper
    {
        public static T FindLogicalParent<T>(DependencyObject childElement) where T : DependencyObject
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(childElement);
            T parentAsT = parent as T;
            if (parent == null)
            {
                return null;
            }
            else if (parentAsT != null)
            {
                return parentAsT;
            }
            return FindLogicalParent<T>(parent);
        }

        public static T FindVisualParent<T>(DependencyObject childElement) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(childElement);
            T parentAsT = parent as T;
            if (parent == null)
            {
                return null;
            }
            else if (parentAsT != null)
            {
                return parentAsT;
            }
            return FindVisualParent<T>(parent);
        }
    }
}
