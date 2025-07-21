//
// Copyright (c) 2025 Weston Berg
//
// SPDX-License-Identifier: MIT
//
using System;
using System.Windows;
using System.Windows.Documents;

namespace MovablePanel
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         ContentRendered += MainWindow_ContentRendered;

         InitializeComponent();

         // Add text to the RichTextBox
         MyRichTextBox.Document.Blocks.Clear();
         MyRichTextBox.Document.Blocks.Add(new Paragraph(new Run("RICHTEXTBOX")));
      }

      private void MainWindow_ContentRendered(object sender, EventArgs e)
      {
         // Adorn all elements of the main panel
         RepositionAdornerConfig btnConfig = new RepositionAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyButton).Add(new RepositionAdorner(MyButton, btnConfig));

         RepositionAdornerConfig txtbxConfig = new RepositionAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyTextBox).Add(new RepositionAdorner(MyTextBox, txtbxConfig));

         RepositionAdornerConfig rtxtbxConfig = new RepositionAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyRichTextBox).Add(new RepositionAdorner(MyRichTextBox, rtxtbxConfig));

         RepositionAdornerConfig borderConfig = new RepositionAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyBorder).Add(new RepositionAdorner(MyBorder, borderConfig));
      }
   }
}
