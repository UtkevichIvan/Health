using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Health
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<PersonTable> _write = new List<PersonTable>();
        private List<PersonTable> _writeFile = new List<PersonTable>();
        private PersonTable _selectedPerson;
        public ObservableCollection<PersonTable> Persons { get; set; }

        public PersonTable SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                OnPropertyChanged("SelectedPerson");
            }
        }

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand // Сохранение в проект
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new RelayCommand(obj =>
                  {

                      PersonTable temp = obj as PersonTable;
                      _write.Add(temp);
                      var options = new JsonSerializerOptions
                      {
                          Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                          WriteIndented = true,
                          DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                      };

                      var json = JsonSerializer.Serialize(_write, options);
                      File.WriteAllText("persons.json", json);

                  }));
            }
        }
        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand // Обработка данных из выбранных файлов
        {
            get
            {
                return openFileCommand ??
                  (openFileCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog();
                      openFileDialog.Multiselect = true;
                      openFileDialog.Filter = "Json files (*.json)|*.json";

                      if (openFileDialog.ShowDialog() == true)
                      {
                          var files = openFileDialog.FileNames;
                          List<Person> result = new List<Person>();
                          for (int i = 0; i < files.Length; i++)
                          {
                              using (FileStream fs = new FileStream($"{files[i]}", FileMode.Open))
                              {
                                  try
                                  {
                                      List<PersonJS>? persons = JsonSerializer.Deserialize<List<PersonJS>>(fs);
                                      foreach (PersonJS person in persons)
                                      {
                                          if (result.Contains(person))
                                          {
                                              int index = result.IndexOf(person);
                                              (result[index] as PersonTable).Rank.Add(person.Rank);
                                              (result[index] as PersonTable).Steps.Add(person.Steps);

                                          }
                                          else
                                          {
                                              PersonTable temp = new PersonTable();
                                              temp.Status = person.Status;
                                              temp.User = person.User;
                                              temp.Steps.Add(person.Steps);
                                              temp.Rank.Add(person.Rank);
                                              result.Add(temp);
                                          }
                                      }
                                  }
                                  catch (JsonException e)
                                  {
                                      Console.WriteLine(e.Message);
                                  }

                              }
                          }

                          ObservableCollection<PersonTable> convResult = new ObservableCollection<PersonTable>();
                          foreach (Person person in result)
                          {
                              convResult.Add(person as PersonTable);
                          }

                          Persons.Clear();
                          foreach (var person in convResult)
                          {
                              Persons.Add(person);
                          }
                      }                      
                  }));
            }
        }
        private RelayCommand saveFileCommand;
        public RelayCommand SaveFileCommand // Сохранение в выбранный файл
        {
            get
            {
                return saveFileCommand ??
                  (saveFileCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog();
                      openFileDialog.Filter = "Json files (*.json)|*.json";

                      if (openFileDialog.ShowDialog() == true)
                      {
                          PersonTable temp = obj as PersonTable;
                          _writeFile.Add(temp);
                          var options = new JsonSerializerOptions
                          {
                              Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                              WriteIndented = true,
                              DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                          };

                          var json = JsonSerializer.Serialize(_writeFile, options);
                          File.WriteAllText(openFileDialog.FileName, json);
                      }
                      

                  }));
            }
        }
        public MainViewModel()
        {
            Persons = GetPersons();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            
        }

        private ObservableCollection<PersonTable> GetPersons()
        {
            List<Person> result = new List<Person>();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Data");

            for (int i = 0; i < files.Length; i++)
            {
                using (FileStream fs = new FileStream($"{files[i]}", FileMode.Open))
                {
                    try
                    {
                        List<PersonJS>? persons = JsonSerializer.Deserialize<List<PersonJS>>(fs);
                        foreach (PersonJS person in persons)
                        {
                            if (result.Contains(person))
                            {
                                int index = result.IndexOf(person);
                                (result[index] as PersonTable).Rank.Add(person.Rank);
                                (result[index] as PersonTable).Steps.Add(person.Steps);

                            }
                            else
                            {
                                PersonTable temp = new PersonTable();
                                temp.Status = person.Status;
                                temp.User = person.User;
                                temp.Steps.Add(person.Steps);
                                temp.Rank.Add(person.Rank);
                                result.Add(temp);
                            }
                        }
                    }
                    catch (JsonException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                }
            }

            ObservableCollection<PersonTable> convResult = new ObservableCollection<PersonTable>();
            foreach (Person person in result)
            {
                convResult.Add(person as PersonTable);
            }

            return convResult;
        }
    }
}
