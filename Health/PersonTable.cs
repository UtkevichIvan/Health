using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Health
{
    public class PersonTable : Person , INotifyPropertyChanged
    {
        [JsonIgnore]
        private Brush MaxBrush { get; set; }
        [JsonIgnore]
        private Brush MinBrush { get; set; }
        [JsonIgnore]
        private List<int> _steps = new List<int>();
        [JsonIgnore]
        private List<int> _rank = new List<int>();
        [JsonIgnore]
        public bool ColorPerson
        {
            get
            {
                if ((double)Max / Average >= 1.2 || (double)Min / Average <= 0.8)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        [JsonIgnore]
        public List<int> Steps
        {
            get
            {
                return _steps;
            }
        }
        [JsonIgnore]
        public List<int> Rank
        {
            get
            {
                return _rank;
            }
        }
        [JsonIgnore]
        public ChartValues<ObservableValue> Points
        {
            get
            {
                var temp = new ChartValues<ObservableValue>();
                for (int i = 0; i < _steps.Count; i++)
                {
                    temp.Add(new ObservableValue(_steps[i]));
                }

                return temp;
            }
        }
        [JsonIgnore]
        public CartesianMapper<ObservableValue> Mapper
        {
            get
            {
                return Mappers.Xy<ObservableValue>()
                .X((item, index) => index)
                .Y(item => item.Value)
                .Fill(item =>
                {
                    if (item.Value >= Max)
                    {
                        return MaxBrush;
                    }
                    else if (item.Value <= Min)
                    {
                        return MinBrush;
                    }
                    else
                    {
                        return null;
                    }
                })
                .Stroke(item =>
                {
                    if (item.Value >= Max)
                    {
                        return MaxBrush;
                    }
                    else if (item.Value <= Min)
                    {
                        return MinBrush;
                    }
                    else
                    {
                        return null;
                    }
                });
            }
        }
        public int Average
        {
            get
            {
                if (_steps.Count == 0)
                {
                    return 0;
                }

                int sum = 0;
                foreach (var step in _steps)
                {
                    sum += step;
                }

                return sum / _steps.Count;
            }
        }
        public int Max
        {
            get
            {
                if (_steps.Count == 0)
                {
                    return 0;
                }

                return _steps.Max();
            }
        }
        public int Min
        {
            get
            {
                if (_steps.Count == 0)
                {
                    return 0;
                }

                return _steps.Min();
            }
        }
        public int RankAverage
        {
            get
            {
                if (_rank.Count == 0)
                {
                    return 0;
                }

                var sum = 0;
                for (int i = 0; i < _rank.Count; i++)
                {
                    sum += _rank[i];
                }

                return sum / _rank.Count;
            }
        }
        public PersonTable()
        {
            MaxBrush = new SolidColorBrush(Color.FromRgb(238, 83, 80));
            MinBrush = new SolidColorBrush(Color.FromRgb(255, 255, 000));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
