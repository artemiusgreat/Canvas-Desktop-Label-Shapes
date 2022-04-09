using Canvas.Source.ModelSpace;
using System;
using System.Collections.Generic;

namespace Canvas.Source.ModelSpace
{
  public class LabelGroupModel : GroupModel, IGroupModel
  {
    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] CreateDomain(int position, string name, IList<IGroupModel> items)
    {
      var currentModel = Composer.GetGroup(position, name, items);

      if (currentModel?.Point is null)
      {
        return null;
      }

      var size = currentModel.Size ?? Size;
      var contentSize = Panel.GetContentMeasure(currentModel.Label, size);
      var posValue = Composer.GetValues(Panel, new PointModel { Index = 0, Value = contentSize.Index });

      return new double[]
      {
        currentModel.Point,
        currentModel.Point + posValue.Value
      };
    }

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int position, string name, IList<IGroupModel> items)
    {
      var currentModel = Composer.GetGroup(position, name, items);

      if (currentModel?.Point is null)
      {
        return;
      }

      Size = currentModel.Size ?? Size;
      Color = currentModel.Color ?? Color;

      var contentSize = Panel.GetContentMeasure(currentModel.Label, Size.Value);
      var points = new IPointModel[]
      {
        Composer.GetPixels(Panel, position, currentModel.Point + currentModel.Y),
        Composer.GetPixels(Panel, position, currentModel.Point + currentModel.Y + contentSize.Index)
      };

      Panel.CreateLabelShape(points, this, currentModel.Label);
    }
  }
}
