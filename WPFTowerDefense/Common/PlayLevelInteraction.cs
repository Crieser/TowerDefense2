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

        // Sets whether the canvas should initialize the play view model.
        public static void SetInitializeCanvas(DependencyObject element, bool value)
        {
            element.SetValue(InitializeCanvasProperty, value);
        }

        // Reads whether canvas initialization is enabled.
        public static bool GetInitializeCanvas(DependencyObject element)
        {
            return (bool)element.GetValue(InitializeCanvasProperty);
        }

        // Sets whether tower dropping is enabled for an element.
        public static void SetEnableTowerDrop(DependencyObject element, bool value)
        {
            element.SetValue(EnableTowerDropProperty, value);
        }

        // Reads whether tower dropping is enabled.
        public static bool GetEnableTowerDrop(DependencyObject element)
        {
            return (bool)element.GetValue(EnableTowerDropProperty);
        }

        // Sets whether mouse clicks should clear the upgrade panel.
        public static void SetClearUpgradeOnMouseDown(DependencyObject element, bool value)
        {
            element.SetValue(ClearUpgradeOnMouseDownProperty, value);
        }

        // Reads whether mouse clicks clear the upgrade panel.
        public static bool GetClearUpgradeOnMouseDown(DependencyObject element)
        {
            return (bool)element.GetValue(ClearUpgradeOnMouseDownProperty);
        }

        // Stores the tower id used when dragging a tower image.
        public static void SetTowerDragId(DependencyObject element, int value)
        {
            element.SetValue(TowerDragIdProperty, value);
        }

        // Reads the tower id from a draggable tower image.
        public static int GetTowerDragId(DependencyObject element)
        {
            return (int)element.GetValue(TowerDragIdProperty);
        }

        // Registers canvas initialization when the property is enabled.
        private static void OnInitializeCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Canvas canvas && (bool)e.NewValue)
            {
                canvas.Loaded += InitializeCanvasLoaded;
            }
        }

        // Initializes the play view model after the canvas is loaded.
        private static void InitializeCanvasLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas && canvas.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.Initialize(canvas);
            }
        }

        // Enables drag over and drop handling for tower placement.
        private static void OnEnableTowerDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                element.AllowDrop = true;
                element.DragOver += TowerDropDragOver;
                element.Drop += TowerDrop;
            }
        }

        // Updates the drag effect while a tower is over the play field.
        private static void TowerDropDragOver(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.SetDragEffects(e);
            }
        }

        // Passes the dropped tower data to the play view model.
        private static void TowerDrop(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.DropTower(e);
            }
        }

        // Registers a mouse handler to close the upgrade panel.
        private static void OnClearUpgradeOnMouseDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                element.MouseDown += ClearUpgradeMouseDown;
            }
        }

        // Clears the selected tower when the player clicks outside buttons.
        private static void ClearUpgradeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is PlayLevelScreenViewModel viewModel)
            {
                viewModel.ClearUpgradePanel(e.OriginalSource);
            }
        }

        // Registers mouse movement for draggable tower images.
        private static void OnTowerDragIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.MouseMove += TowerMouseMove;
            }
        }

        // Starts drag and drop when the player drags a tower image.
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
