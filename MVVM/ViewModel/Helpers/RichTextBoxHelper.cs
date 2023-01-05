using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;

namespace NotesApp.MVVM.ViewModel.Helpers
{
    public class RichTextBoxHelper : DependencyObject
    {
        private static List<Guid> _recursionProtection = new List<Guid>();

        public static FlowDocument GetDocumentXaml(DependencyObject obj)
        {
            return (FlowDocument)obj.GetValue(DocumentProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            var fw1 = (FrameworkElement)obj;
            if (fw1.Tag == null || (Guid)fw1.Tag == Guid.Empty)
                fw1.Tag = Guid.NewGuid();
            _recursionProtection.Add((Guid)fw1.Tag);
            obj.SetValue(DocumentProperty, value);
            _recursionProtection.Remove((Guid)fw1.Tag);
        }

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(FlowDocument),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SetDocument));


        private static void SetDocument(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            RichTextBox rtb = (RichTextBox)obj;
            FlowDocument document = (FlowDocument)e.NewValue;

            if (document == null)
            {
                rtb.Document = new FlowDocument();
            }
            else
            {
                rtb.Document = document;
            }
        }

    }
}
