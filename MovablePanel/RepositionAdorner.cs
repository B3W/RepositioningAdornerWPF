//
// Copyright (c) 2025 Weston Berg
//
// SPDX-License-Identifier: MIT
//
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
   public class RepositionAdorner : Adorner
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

      /// <summary>
      /// Cache that holds coordinates for repositioning thumbs
      /// </summary>
      private readonly Rect[] _thumbCoordinates;

      #endregion // Fields


      #region Properties

      // Overriding FrameworkElement property as this adorner will contain more than one visual
      // Allows WPF framework to interface with this adorner's visual collection
      protected override int VisualChildrenCount => _visualChildren.Count;

      #endregion // Properties


      #region Methods

      /// <summary>
      /// Constructs a RepositionAdorner attached to the given UIElement
      /// </summary>
      /// <param name="adornedElement">UIElement to adorn. Must be child of Canvas.</param>
      /// <param name="adornerConfig">Configuration of adorner</param>
      public RepositionAdorner(UIElement adornedElement, RepositioningAdornerConfig adornerConfig) : base(adornedElement)
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

         // Create starting coordinates for all thumbs that will be updated in ArrangeOverride
         _thumbCoordinates = new Rect[(int)ThumbIndex.NumThumbs];

         _thumbCoordinates[(int)ThumbIndex.Top]    = new Rect(                         0.0D, -(_config.ThumbThickness / 2),                   0.0D, _config.ThumbThickness);
         _thumbCoordinates[(int)ThumbIndex.Bottom] = new Rect(                         0.0D,                          0.0D,                   0.0D, _config.ThumbThickness);
         _thumbCoordinates[(int)ThumbIndex.Left]   = new Rect(-(_config.ThumbThickness / 2),                          0.0D, _config.ThumbThickness,                   0.0D);
         _thumbCoordinates[(int)ThumbIndex.Right]  = new Rect(                         0.0D,                          0.0D, _config.ThumbThickness,                   0.0D);
      }


      // Overriding FrameworkElement method to place adorners in correct location
      protected override Size ArrangeOverride(Size finalSize)
      {
         // Calculate new coordinates for thumbs
         // NOTE: Use 'finalSize' instead of 'DesiredSize'. 'DesiredSize' does not behave well with zooming
         _thumbCoordinates[(int)ThumbIndex.Top].Width = finalSize.Width;
         _thumbCoordinates[(int)ThumbIndex.Bottom].Y = finalSize.Height - (_config.ThumbThickness / 2);
         _thumbCoordinates[(int)ThumbIndex.Bottom].Width = finalSize.Width;
         _thumbCoordinates[(int)ThumbIndex.Left].Height = finalSize.Height;
         _thumbCoordinates[(int)ThumbIndex.Right].X = finalSize.Width - (_config.ThumbThickness / 2);
         _thumbCoordinates[(int)ThumbIndex.Right].Height = finalSize.Height;

         // Apply new coordinates to thumbs
         _repositionThumbs[(int)ThumbIndex.Top].Arrange(_thumbCoordinates[(int)ThumbIndex.Top]);
         _repositionThumbs[(int)ThumbIndex.Bottom].Arrange(_thumbCoordinates[(int)ThumbIndex.Bottom]);
         _repositionThumbs[(int)ThumbIndex.Left].Arrange(_thumbCoordinates[(int)ThumbIndex.Left]);
         _repositionThumbs[(int)ThumbIndex.Right].Arrange(_thumbCoordinates[(int)ThumbIndex.Right]);

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


      /// <summary>
      /// Handler for Thumb DragStarted event
      /// </summary>
      /// <param name="sender">Thumb which started drag</param>
      /// <param name="e">Information on drag start</param>
      private void OnDragStarted(object sender, DragStartedEventArgs e)
      {
         // Implement as needed
      }


      /// <summary>
      /// Handler for Thumb DragDelta event
      /// </summary>
      /// <param name="sender">Thumb which was dragged</param>
      /// <param name="e">Information on drag</param>
      private void OnDragDelta(object sender, DragDeltaEventArgs e)
      {
         // Apply the change in position
         Canvas.SetTop(AdornedElement, Canvas.GetTop(AdornedElement) + e.VerticalChange);
         Canvas.SetLeft(AdornedElement, Canvas.GetLeft(AdornedElement) + e.HorizontalChange);
      }


      /// <summary>
      /// Handler for Thumb DragCompleted event
      /// </summary>
      /// <param name="sender">Thumb which was dragged</param>
      /// <param name="e">Information on drag completion</param>
      private void OnDragCompleted(object sender, DragCompletedEventArgs e)
      {
         // Implement as needed
      }

      #endregion // Methods
   }
}
