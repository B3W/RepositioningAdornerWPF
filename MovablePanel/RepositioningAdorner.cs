using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MovablePanel
{
   /// <summary>
   /// Configuration for the repositioning adorner
   /// </summary>
   public class RepositioningAdornerConfig
   {
      public int ThumbThickness { get; } = 8;

      public double ThumbOpacity { get; } = 0.0D;

      public Brush ThumbColor { get; } = new SolidColorBrush(Colors.LightGray);
   }


   /// <summary>
   /// Adorner that allows for repositioning of the adorned element
   /// </summary>
   public class RepositioningAdorner : Adorner
   {
      /// <summary>
      /// Enumeration defining where each thumb resides in the thumbs array
      /// </summary>
      private enum ThumbIndex
      {
         Top = 0,
         Bottom,
         Left,
         Right,
         NumThumbs
      }


      #region Fields

      /// <summary>
      /// Collection of all visual children managed by adorner
      /// </summary>
      private readonly VisualCollection _visualChildren;

      /// <summary>
      /// Collection of all thumbs that can be used to reposition adorned element
      /// </summary>
      private readonly Thumb[] _repositionThumbs;

      /// <summary>
      /// Configuration of adorner
      /// </summary>
      private readonly RepositioningAdornerConfig _config;

      #endregion // Fields


      #region Properties

      // Overriding FrameworkElement property as this adorner will contain more than one visual
      // Allows WPF framework to interface with this adorner's visual collection
      protected override int VisualChildrenCount => _visualChildren.Count;


      private Rect TopThumbRect => new Rect(0.0D,
                                            -(_config.ThumbThickness / 2),
                                            DesiredSize.Width,
                                            _config.ThumbThickness);

      private Rect BottomThumbRect => new Rect(0.0D,
                                               DesiredSize.Height - (_config.ThumbThickness / 2),
                                               DesiredSize.Width,
                                               _config.ThumbThickness);

      private Rect LeftThumbRect => new Rect(-(_config.ThumbThickness / 2),
                                             0.0D,
                                             _config.ThumbThickness,
                                             DesiredSize.Height);

      private Rect RightThumbRect => new Rect(DesiredSize.Width - (_config.ThumbThickness / 2),
                                              0.0D,
                                              _config.ThumbThickness,
                                              DesiredSize.Height);

      #endregion // Properties


      #region Methods

      public RepositioningAdorner(UIElement adornedElement, RepositioningAdornerConfig adornerConfig) : base(adornedElement)
      {
         _config = adornerConfig;

         if (!((adornedElement as FrameworkElement).Parent is Canvas))
         {
            throw new ArgumentException("Adorned element must be the child of a Canvas", "adornedElement");
         }

         _visualChildren = new VisualCollection(adornedElement);
         _repositionThumbs = new Thumb[(int)ThumbIndex.NumThumbs];

         // Create all resize thumbs
         _repositionThumbs[(int)ThumbIndex.Top] = CreateRepositionThumb(Cursors.SizeAll);
         _repositionThumbs[(int)ThumbIndex.Bottom] = CreateRepositionThumb(Cursors.SizeAll);
         _repositionThumbs[(int)ThumbIndex.Left] = CreateRepositionThumb(Cursors.SizeAll);
         _repositionThumbs[(int)ThumbIndex.Right] = CreateRepositionThumb(Cursors.SizeAll);

         // Add thumbs to the visual children of the adorner
         foreach (Thumb thumb in _repositionThumbs)
         {
            _ = _visualChildren.Add(thumb);
         }
      }



      protected override Size ArrangeOverride(Size finalSize)
      {
         _repositionThumbs[(int)ThumbIndex.Top].Arrange(TopThumbRect);
         _repositionThumbs[(int)ThumbIndex.Bottom].Arrange(BottomThumbRect);
         _repositionThumbs[(int)ThumbIndex.Left].Arrange(LeftThumbRect);
         _repositionThumbs[(int)ThumbIndex.Right].Arrange(RightThumbRect);

         return finalSize;
      }


      // Overriding FrameworkElement property as this adorner will contain more than one visual
      // Allows WPF framework to interface with this adorner's visual collection
      protected override Visual GetVisualChild(int index)
      {
         return _visualChildren[index];
      }


      /// <summary>
      /// Creates a thumb that can be used to reposition adorned element
      /// </summary>
      /// <returns>Thumb that can be used to reposition</returns>
      private RepositionThumb CreateRepositionThumb(Cursor cursor)
      {
         RepositionThumb repositionThumb = new RepositionThumb()
         {
            Cursor = cursor,
            Background = _config.ThumbColor,
            Opacity = _config.ThumbOpacity,
         };

         repositionThumb.DragStarted += OnDragStarted;
         repositionThumb.DragDelta += OnDragDelta;
         repositionThumb.DragCompleted += OnDragCompleted;

         return repositionThumb;
      }



      private void OnDragStarted(object sender, DragStartedEventArgs e)
      {
         // TODO
      }



      private void OnDragDelta(object sender, DragDeltaEventArgs e)
      {
         // Apply the change in position
         Canvas.SetTop(AdornedElement, Canvas.GetTop(AdornedElement) + e.VerticalChange);
         Canvas.SetLeft(AdornedElement, Canvas.GetLeft(AdornedElement) + e.HorizontalChange);
      }



      private void OnDragCompleted(object sender, DragCompletedEventArgs e)
      {
         // TODO
      }

      #endregion // Methods
   }
}
