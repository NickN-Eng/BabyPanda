using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BabyPanda.WPF
{
    public class BPDataGrid : DataGrid
    {
        
        public event EventHandler<HeaderButtonClickedEventArgs> HeaderButtonClicked;

        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                var type = e.PropertyType;
                if (type == typeof(string) || type == typeof(double) || type == typeof(int) || type == typeof(bool))
                {
                    base.OnAutoGeneratingColumn(e);
                }
                else
                {
                    var tf = TypeMaster.GetTypeFunction(type);
                    if (tf == null)
                    {
                        base.OnAutoGeneratingColumn(e);
                    }

                    var col = new TypeFunctionCol() { TypeFunc = tf };
                    col.Binding = new Binding(e.PropertyName) { Mode = BindingMode.TwoWay };
                    e.Column = col;
                }
                var propDescr = e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor;

                var headerStack = new StackPanel() { Orientation = Orientation.Horizontal };
                var headerText = new TextBlock() { Text = propDescr.Name };
                var headerSpace = new Grid() { Width = 5d };
                var headerButt = new Button() { /*IsHitTestVisible = true,*/ Width = 15d };

                headerButt.Click += HeaderButt_Click;
                headerStack.Children.Add(headerText);
                headerStack.Children.Add(headerSpace);
                headerStack.Children.Add(headerButt);
                headerStack.DataContext = propDescr.Name; //Not sure if this is correct, but it allows the column name to be retrieved when the header button is clicked

                //e.Column.Header = propDescr.DisplayName;
                e.Column.Header = headerStack;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HeaderButt_Click(object sender, RoutedEventArgs e)
        {
            var butt = (Button)sender;
            string colName = (string)butt.DataContext;

            HeaderButtonClicked?.Invoke(this, new HeaderButtonClickedEventArgs() { HeaderName = colName });

            if (this.HeaderButtonClickedCommand != null)
            {
                ((ICommand)HeaderButtonClickedCommand).Execute(colName);
            }
        }

        public ICommand HeaderButtonClickedCommand
        {
            get { return (ICommand)GetValue(HeaderButtonClickedCommandProperty); }
            set { SetValue(HeaderButtonClickedCommandProperty, value); }
        }

        public static readonly DependencyProperty HeaderButtonClickedCommandProperty =
            DependencyProperty.Register("HeaderButtonClickedCommand", typeof(ICommand), typeof(BPDataGrid), new UIPropertyMetadata(null));

        private static void OnHeaderButtonClickedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (BPDataGrid)d;
        }

        public void RefreshColumns()
        {
            if (AutoGenerateColumns)
            {
                AutoGenerateColumns = false;
                AutoGenerateColumns = true;
            }
        }

        #region Command source implmentation

        //// Make Command a dependency property so it can use databinding.
        //public static readonly DependencyProperty CommandProperty =
        //    DependencyProperty.Register(
        //        "Command",
        //        typeof(ICommand),
        //        typeof(BPDataGrid),
        //        new PropertyMetadata((ICommand)null,
        //        new PropertyChangedCallback(CommandChanged)));

        //public ICommand Command
        //{
        //    get => (ICommand)GetValue(CommandProperty);
        //    set =>SetValue(CommandProperty, value);
        //}

        //public object CommandParameter
        //{
        //    get { return (object)GetValue(CommandParameterProperty); }
        //    set { SetValue(CommandParameterProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CommandParameterProperty =
        //    DependencyProperty.Register("CommandParameter", typeof(object), typeof(BPDataGrid), new UIPropertyMetadata(null));

        //public IInputElement CommandTarget
        //{
        //    get { return (IInputElement)GetValue(CommandTargetProperty); }
        //    set { SetValue(CommandTargetProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CommandTargetProperty =
        //    DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(BPDataGrid), new UIPropertyMetadata(null));

        //// Command dependency property change callback.
        //private static void CommandChanged(DependencyObject d,
        //    DependencyPropertyChangedEventArgs e)
        //{
        //    BPDataGrid dg = (BPDataGrid)d;
        //    dg.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        //}

        //// Add a new command to the Command Property.
        //private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        //{
        //    // If oldCommand is not null, then we need to remove the handlers.
        //    if (oldCommand != null)
        //    {
        //        RemoveCommand(oldCommand, newCommand);
        //    }
        //    AddCommand(oldCommand, newCommand);
        //}

        //// Remove an old command from the Command Property.
        //private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        //{
        //    EventHandler handler = CanExecuteChanged;
        //    oldCommand.CanExecuteChanged -= handler;
        //}

        //// Add the command.
        //private void AddCommand(ICommand oldCommand, ICommand newCommand)
        //{
        //    EventHandler handler = new EventHandler(CanExecuteChanged);
        //    var canExecuteChangedHandler = handler;
        //    if (newCommand != null)
        //    {
        //        newCommand.CanExecuteChanged += canExecuteChangedHandler;
        //    }
        //}

        //private void CanExecuteChanged(object sender, EventArgs e)
        //{

        //    if (this.Command != null)
        //    {
        //        RoutedCommand command = this.Command as RoutedCommand;

        //        // If a RoutedCommand.
        //        if (command != null)
        //        {
        //            if (command.CanExecute(CommandParameter, CommandTarget))
        //                this.IsEnabled = true;
        //            else
        //                this.IsEnabled = false;
        //        }
        //        // If a not RoutedCommand.
        //        else
        //        {
        //            if (Command.CanExecute(CommandParameter))
        //                this.IsEnabled = true;
        //            else
        //                this.IsEnabled = false;
        //        }
        //    }
        //}

        #endregion
    }

    public class HeaderButtonClickedEventArgs : EventArgs
    {
        public string HeaderName { get; set; }
    }

    //public partial class BPDataGrid : UserControl, ICommandSource
    //{
    //    public BPDataGrid()
    //    {
    //        InitializeComponent();
    //    }



    //    public ICommand Command
    //    {
    //        get { return (ICommand)GetValue(CommandProperty); }
    //        set { SetValue(CommandProperty, value); }
    //    }

    //    public static readonly DependencyProperty CommandProperty =
    //        DependencyProperty.Register("Command", typeof(ICommand), typeof(BPDataGrid), new UIPropertyMetadata(null));


    //    public object CommandParameter
    //    {
    //        get { return (object)GetValue(CommandParameterProperty); }
    //        set { SetValue(CommandParameterProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty CommandParameterProperty =
    //        DependencyProperty.Register("CommandParameter", typeof(object), typeof(BPDataGrid), new UIPropertyMetadata(null));

    //    public IInputElement CommandTarget
    //    {
    //        get { return (IInputElement)GetValue(CommandTargetProperty); }
    //        set { SetValue(CommandTargetProperty, value); }
    //    }

    //    // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty CommandTargetProperty =
    //        DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(BPDataGrid), new UIPropertyMetadata(null));


    //    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    //    {
    //        base.OnMouseLeftButtonUp(e);

    //        var command = Command;
    //        var parameter = CommandParameter;
    //        var target = CommandTarget;

    //        var routedCmd = command as RoutedCommand;
    //        if (routedCmd != null && routedCmd.CanExecute(parameter, target))
    //        {
    //            routedCmd.Execute(parameter, target);
    //        }
    //        else if (command != null && command.CanExecute(parameter))
    //        {
    //            command.Execute(parameter);
    //        }
    //    }

    ////}
}
