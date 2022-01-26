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
      }

      private void MainWindow_ContentRendered(object sender, EventArgs e)
      {
         // Adorn all elements of the main panel
         RepositioningAdornerConfig btnConfig = new RepositioningAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyButton).Add(new RepositioningAdorner(MyButton, btnConfig));

         RepositioningAdornerConfig txtbxConfig = new RepositioningAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyTextBox).Add(new RepositioningAdorner(MyTextBox, txtbxConfig));

         RepositioningAdornerConfig rtxtbxConfig = new RepositioningAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyRichTextBox).Add(new RepositioningAdorner(MyRichTextBox, rtxtbxConfig));

         RepositioningAdornerConfig borderConfig = new RepositioningAdornerConfig();
         AdornerLayer.GetAdornerLayer(MyBorder).Add(new RepositioningAdorner(MyBorder, borderConfig));
      }
   }
}
