using Canvas.Source;
using Canvas.Source.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;

namespace Client.WPF
{
  public partial class MainWindow : Window
  {
    public int Count = 0;
    public object sync = new();
    public double CurrentOpen = 0;
    public Timer Clock = new(100);
    public Random Generator = new();
    public DateTime Time = DateTime.UtcNow;
    public IList<IGroupModel> Points = new List<IGroupModel>();

    /// <summary>
    /// Constructor
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

      ViewBars.Composer = new Composer { Name = "Bars" };
      ViewBars.Create();
      ViewBars.Update();

      Clock.Enabled = true;
      Clock.AutoReset = true;
      Clock.Elapsed += (sender, e) =>
      {
        try
        {
          Dispatcher.Invoke(() => Counter(sender, e));
          Clock.Enabled = true;
        }
        catch (Exception) { }
      };
    }

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Counter(object sender, ElapsedEventArgs e)
    {
      if (Points.Count > 150)
      {
        Clock.Stop();
      }

      var point = Generator.Next(100, 1500);
      var barModel = new Model { ["Point"] = point };
      var labelModel = new Model
      {
        ["Y"] = 20,
        ["Size"] = 10,
        ["Point"] = point,
        ["Color"] = SKColors.Red,
        ["Label"] = "Demo : " + point
      };

      if (Points.Count == 0 || IsNextFrame())
      {
        Time = DateTime.UtcNow;
        CurrentOpen = point;
        Points.Add(new GroupModel
        {
          Index = Time.Ticks,
          Groups = new Dictionary<string, IGroupModel>
          {
            ["Bars"] = new GroupModel
            {
              Groups = new Dictionary<string, IGroupModel>
              {
                ["V1"] = new BarGroupModel { Value = barModel },
                ["V2"] = new LabelGroupModel { Value = labelModel }
              }
            }
          }
        });
      }

      var composer = ViewBars.Composer;

      composer.Groups = Points;
      composer.IndexDomain ??= new int[2];
      composer.IndexDomain[0] = composer.Groups.Count - composer.IndexCount;
      composer.IndexDomain[1] = composer.Groups.Count;
      ViewBars.Update();
    }

    /// <summary>
    /// Create new bar when it's time
    /// </summary>
    /// <returns></returns>
    protected bool IsNextFrame()
    {
      return DateTime.UtcNow.Ticks - Time.Ticks >= TimeSpan.FromMilliseconds(100).Ticks;
    }
  }
}
