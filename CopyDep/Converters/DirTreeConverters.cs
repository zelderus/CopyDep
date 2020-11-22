using CopyDep.Models;
using CopyDep.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CopyDep.Converters
{
    class DirTreeConverters
    {
    }

    public class DirTreeConverter_Icon : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return DependencyProperty.UnsetValue; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Boolean)value) ? StringUtils.AssetImage("folder.png") : StringUtils.AssetImage("file.png");
        }
    }


    public class DirTreeConverter_FontWeight : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return DependencyProperty.UnsetValue; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Boolean)value) ? "SemiBold" : String.Empty;
        }
    }


    public class DirTreeConverter_ExtraText : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return DependencyProperty.UnsetValue; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Boolean)value) ? "(+)" : String.Empty;
        }
    }



    public class DirTreeConverter_FontColorByStatus : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return DependencyProperty.UnsetValue; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = ((DirTreeViewItemModelStatus)value);
            if (status == DirTreeViewItemModelStatus.Success) return "Green";
            if (status == DirTreeViewItemModelStatus.Error) return "Red";
            return "Black";
        }
    }



}
