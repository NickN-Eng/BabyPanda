using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BabyPanda.WPF
{
    /// <summary>
    /// Interaction logic for TextBoxForBPTypes.xaml
    /// </summary>
    public partial class TextBoxForBPTypes : UserControl
    {
        public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register("SourceObject", typeof(object), typeof(TextBoxForBPTypes), new PropertyMetadata(null, OnSourceObjectChanged));

        public static SolidColorBrush DefaultBorderBrush = new SolidColorBrush(Color.FromArgb(255, 171, 173, 179));
        public static SolidColorBrush ErrorBorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        public object SourceObject
        {
            get => GetValue(SourceObjectProperty);
            set => SetValue(SourceObjectProperty, value);
        }
        
        private static void OnSourceObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxForBPTypes source = (TextBoxForBPTypes)d;
            if (source == null) return;
            var tf = source.TypeFunc;
            if (tf == null) return;

            var newValue = e.NewValue;
            if(newValue == null)
            {
                source.TextField = "";
                source.BackgroundBrush = new System.Windows.Media.SolidColorBrush();
                return;
            }

            source.TextField = source.TypeFunc.ToDisplayString(newValue);
            //bool success = source.TypeFunc.TryConvert_Object(newValue, typeof(string), out object result);
            //if (success)
            //{
            //    source.TextField = (string)result;
            //} 
                

            if (tf.IsConvertibleToColor())
            {
                bool colorSuccess = source.TypeFunc.TryGetColourRepresentation(newValue, out System.Drawing.Color colorResult);
                if (colorSuccess)
                {
                    var medColor = System.Windows.Media.Color.FromArgb(colorResult.A, colorResult.R, colorResult.G, colorResult.B);
                    source.BackgroundBrush = new System.Windows.Media.SolidColorBrush(medColor);
                }
                else
                {
                    source.BackgroundBrush = new System.Windows.Media.SolidColorBrush();
                }
            }
            else
            {
                source.BackgroundBrush = new System.Windows.Media.SolidColorBrush();
            }
            


        }

        public static readonly DependencyProperty TypeFuncProperty = DependencyProperty.Register("TypeFunc", typeof(ITypeFunction), typeof(TextBoxForBPTypes), new PropertyMetadata(null, OnTypeFunctionChanged));

        public ITypeFunction TypeFunc
        {
            get => (ITypeFunction)GetValue(TypeFuncProperty);
            set => SetValue(TypeFuncProperty, value);
        }

        private static void OnTypeFunctionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxForBPTypes source = (TextBoxForBPTypes)d;
            var tf = (ITypeFunction)e.NewValue;
            if (tf == null) return;
            source.IsEditableByText = tf.IsConvertibleFromString();
            source.IsEditableByButton = tf.IsCollectableFromModalInput();
            source.EditButtonCommand = new RelayCommand(o => TryGetFromButtonClick(source));
        }

        public static readonly DependencyProperty TextFieldProperty = DependencyProperty.Register("TextField", typeof(string), typeof(TextBoxForBPTypes), new PropertyMetadata("", OnTextFieldChanged));

        public string TextField
        {
            get => (string)GetValue(TextFieldProperty);
            set => SetValue(TextFieldProperty, value);
        }

        private static void OnTextFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxForBPTypes source = (TextBoxForBPTypes)d;
            if (!source.IsEditableByText) return;

            string newValue = (string)e.NewValue;
            if (newValue == "" || newValue == null)
            {
                source.TextBoxBP.BorderBrush = DefaultBorderBrush;
                
            }

            var success = source.TypeFunc.TryConvertFromString(newValue, out object objResult);
            if (success)
            {
                source.SourceObject = objResult;
                source.TextBoxBP.BorderBrush = DefaultBorderBrush;
            }
            else
            {
                source.TextBoxBP.BorderBrush = ErrorBorderBrush;
            }
        }



        public bool IsEditableByText
        {
            get { return (bool)GetValue(IsEditableByTextProperty); }
            set { SetValue(IsEditableByTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditableByText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableByTextProperty =
            DependencyProperty.Register("IsEditableByText", typeof(bool), typeof(TextBoxForBPTypes), new PropertyMetadata(false));



        public bool IsEditableByButton
        {
            get { return (bool)GetValue(IsEditableByButtonProperty); }
            set { SetValue(IsEditableByButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditableByButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableByButtonProperty =
            DependencyProperty.Register("IsEditableByButton", typeof(bool), typeof(TextBoxForBPTypes), new PropertyMetadata(false));

        public ICommand EditButtonCommand
        {
            get { return (ICommand)GetValue(EditButtonCommandProperty); }
            set { SetValue(EditButtonCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditButtonCommandProperty =
            DependencyProperty.Register("EditButtonCommand", typeof(ICommand), typeof(TextBoxForBPTypes), new PropertyMetadata(null));

        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(TextBoxForBPTypes), new PropertyMetadata(new SolidColorBrush()));

        private static void TryGetFromButtonClick(TextBoxForBPTypes source)
        {
            var tf = source.TypeFunc;
            bool success = tf.TryCollectFromModalInput(out object resultObj);
            if(success /*&& resultObj != null*/)
            {
                source.SourceObject = resultObj;
            }
        }

        public TextBoxForBPTypes()
        {
            InitializeComponent();
        }
    }
}
