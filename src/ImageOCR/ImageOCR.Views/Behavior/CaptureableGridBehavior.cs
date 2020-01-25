using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace ImageOCR.Views
{
    public class CaptureableGridBehavior : Behavior<Grid>
    {
        public ICommand ExecuteCommand
        {
            get => (ICommand)GetValue(ExecuteCommandProperty);
            set => SetValue(ExecuteCommandProperty, value);
        }
        public static DependencyProperty ExecuteCommandProperty = DependencyProperty.Register("ExecuteCommand",
                                                                                              typeof(ICommand),
                                                                                              typeof(CaptureableGridBehavior),
                                                                                              new PropertyMetadata(null));

        public Rectangle SelectionRectangle
        {
            get => (Rectangle)GetValue(SelectionRectangleProperty);
            set => SetValue(SelectionRectangleProperty, value);
        }
        public static DependencyProperty SelectionRectangleProperty = DependencyProperty.Register("SelectionRectangle",
                                                                                              typeof(Rectangle),
                                                                                              typeof(CaptureableGridBehavior),
                                                                                              new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var mouseDown = Observable.FromEventPattern<System.Windows.Input.MouseEventArgs>(this.AssociatedObject, "MouseLeftButtonDown");
            var mouseMove = Observable.FromEventPattern<System.Windows.Input.MouseEventArgs>(this.AssociatedObject, "MouseMove");
            var mouseUp = Observable.FromEventPattern<System.Windows.Input.MouseEventArgs>(this.AssociatedObject, "MouseLeftButtonUp");

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            var captureView = Application.Current.Windows.OfType<CaptureWindow>().FirstOrDefault();
            mainWindow.Hide();

            var origin = new Point();
            mouseDown
                .Do(x => origin = x.EventArgs.GetPosition(this.AssociatedObject))
                .SelectMany(mouseMove)
                .TakeUntil(mouseUp)
                .Do(x =>
                {
                    var pos = x.EventArgs.GetPosition(this.AssociatedObject);
                    var rect = boundsRect(origin.X, origin.Y, pos.X, pos.Y);
                    SelectionRectangle.Margin = new Thickness(rect.Left, rect.Top, captureView.Width - rect.Right, captureView.Height - rect.Bottom);
                    SelectionRectangle.Width = rect.Width;
                    SelectionRectangle.Height = rect.Height;
                })
                .LastAsync()
                .Subscribe(x =>
                {
                    var originalPos = captureView.PointToScreen(new Point(origin.X, origin.Y));
                    var currentPos = captureView.PointToScreen(x.EventArgs.GetPosition(this.AssociatedObject));
                    captureView.Hide();
                    var rect = boundsRect(originalPos.X, originalPos.Y, currentPos.X, currentPos.Y);
                    ExecuteCommand?.Execute(Rect.Offset(rect, 0, 0));

                    mainWindow.Show();
                });
        }

        private Rect boundsRect(double left, double top, double right, double bottom)
        {
            return new Rect(Math.Min(left, right), Math.Min(top, bottom), Math.Abs(right - left), Math.Abs(bottom - top));
        }
    }
}
