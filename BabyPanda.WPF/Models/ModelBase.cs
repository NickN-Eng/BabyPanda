using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BabyPanda.WPF
{
    public class DataTable_SidePanel : ModelBase
    {
        public DataTable_SidePanel(DatatableEditor_Model parent)
        {
            Parent = parent;
        }

        public DatatableEditor_Model Parent { get; set; }


    }

    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
