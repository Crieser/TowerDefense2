using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFTowerDefense.ViewModels;

namespace WPFTowerDefense.Common
{
    public static class PlayLevelInteraction
    {
        public static readonly DependencyProperty InitializeCanvasProperty =
            DependencyProperty.RegisterAttached(
                "InitializeCanvas",
                typeof(bool),
                typeof(PlayLevelInteraction),
                new PropertyMetadata(false, OnInitializeCanvasChanged));

        public static readonly DependencyProperty EnableTowerDropProperty =
            DependencyProperty.RegisterAttached(
                "EnableTowerDrop",
                typeof(bool),
                typeof(PlayLevelInteraction),
                new PropertyMetadata(false, OnEnableTowerDropChanged));

        public static readonly DependencyProperty ClearUpgradeOnMouseDownProperty =
            DependencyProperty.RegisterAttached(
                "ClearUpgradeOnMouseDown",
                typeof(bool),
                typeof(PlayLevelInteraction),
                new PropertyMetadata(false, OnClearUpgradeOnMouseDownChanged));

        public static readonly DependencyProperty TowerDragIdProperty =
            DependencyProperty.RegisterAttached(
                "TowerDragId",
                typeof(int),
                typeof(PlayLevelInteraction),
                new PropertyMetadata(0, OnTowerDragIdChanged));

        public static void SetInitializeCanvas(DependencyObject element, bool value)
        {
            element.SetValue(InitializeCanvasProperty, value);
        }

        public static bool GetInitializeCanvas(DependencyObject element)
        {
            return (bool)element.GetValue(InitializeCanvasProperty);
        }

        public static void SetEnableTowerDrop(DependencyObject element, bool value)
        {
            element.SetValue(EnableTowerDropProperty, value);
        }

        public static bool GetEnableTowerDrop(DependencyObject element)
        {
            return (bool)element.GetValue(EnableTowerDropProperty);
        }

        public static void SetClearUpgradeOnMouseDown(DependencyObject element, bool value)
        {
            element.SetValue(ClearUpgradeOnMouseDownProperty, value);
        }

        public static bool GetClearUpgradeOnMouseDown(DependencyObject element)
        {
            return (bool)element.GetValue(ClearUpgradeOnMouseDownProperty);
        }

        public static void SetTowerDragId(DependencyObject element, int value)
        {
            element.SetValue(TowerDragIdProperty, value);
        }

        public static int GetTowerDragId(DependencyObject element)
        {
            return (int)element.GetValue(TowerDragIdProperty);
        }

        private static void OnInitializeCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Canvas canvas && (bool)e.NewValue)
            {
                canvas.Loaded += InitializeCanvasLoaded;
            }
        }

        private static void InitializeCanvasLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas && canvas.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.Initialize(canvas);
            }
        }

        private static void OnEnableTowerDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                element.AllowDrop = true;
                element.DragOver += TowerDropDragOver;
                element.Drop += TowerDrop;
            }
        }

        private static void TowerDropDragOver(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.SetDragEffects(e);
            }
        }

        private static void TowerDrop(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.DropTower(e);
            }
        }

        private static void OnClearUpgradeOnMouseDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                element.MouseDown += ClearUpgradeMouseDown;
            }
        }

        private static void ClearUpgradeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.ClearUpgradePanel(e.OriginalSource);
            }
        }

        private static void OnTowerDragIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseMove += TowerMouseMove;
            }
        }

        private static void TowerMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || sender is not DependencyObject element)
            {
                return;
            }

            int towerId = GetTowerDragId(element);
            if (towerId == 0)
            {
                return;
            }

            var data = new DataObject();
            data.SetData("TowerID", towerId);
            DragDrop.DoDragDrop(element, data, DragDropEffects.Copy);
        }
    }
}
