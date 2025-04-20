using System.ComponentModel;

namespace SKInvalidateMeasure;

public sealed class MainViewModel : INotifyPropertyChanged
{
    double _fontSize = 20;
    bool _enableWorkaround;

    public MainViewModel()
    {
        IncreaseCommand = new Command(() =>
        {
            FontSize += 2;
        });
        DecreaseCommand = new Command(() =>
        {
            FontSize -= 2;
        });
    }

    public bool EnableWorkaround
    {
        get => _enableWorkaround;
        set
        {
            if (_enableWorkaround != value)
            {
                _enableWorkaround = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableWorkaround)));
            }
        }
    }

    public double FontSize 
    { 
        get => _fontSize;
        set
        {
            if (value != _fontSize)
            {
                if (value > 8)
                {
                    _fontSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontSize)));
                }
            }
        }
    }

    public Command IncreaseCommand
    {
        get;
    }

    public Command DecreaseCommand
    {
        get;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
