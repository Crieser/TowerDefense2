# WpfTD2 MVVM Cheat Sheet

This project was rewritten to follow the same MVVM style as the corrected `TowerDefense2` reference project:

- `Common/ActionCommand.cs` implements `ICommand`.
- `Common/NotifyPropertyChanged.cs` implements `INotifyPropertyChanged`.
- `ViewModels/ViewModelBase.cs` stores the shared `Prism.Events.IEventAggregator`.
- `Events` contains Prism event classes.
- Views are `UserControl` classes in the `Views` folder.
- ViewModels own screen state and commands.
- ViewModels communicate with `EventAggregator.GetEvent<...>().Publish(...)`.
- `MainViewModel` listens with `GetEvent<...>().Subscribe(...)`.
- `App.xaml.cs` creates the `EventAggregator`, `MainWindow`, and `MainViewModel`.
- `MainWindow.xaml` displays the current view with a bound `ContentControl`.

## Added Folders

- `WPFTowerDefense/Common`
  - Contains reusable MVVM helper classes.
- `WPFTowerDefense/ViewModels`
  - Contains one ViewModel for the main window and one ViewModel for each screen/window.
- `WPFTowerDefense/Views`
  - Contains the moved screen XAML files and their small code-behind files.
- `WPFTowerDefense/Events`
  - Contains Prism events used for ViewModel communication.

## Added Files

- `WPFTowerDefense/Common/ActionCommand.cs`
  - Same idea as the reference project.
  - Wraps execute/can-execute methods so buttons can use `Command="{Binding ...}"`.

- `WPFTowerDefense/Common/NotifyPropertyChanged.cs`
  - Same idea as the reference project.
  - Lets ViewModels notify the UI when bound properties change.

- `WPFTowerDefense/ViewModels/ViewModelBase.cs`
  - Base class for all ViewModels.
  - Inherits from `NotifyPropertyChanged`.
  - Stores `IEventAggregator`.

- `WPFTowerDefense/ViewModels/MainViewModel.cs`
  - Owns `CurrentView`.
  - Creates views and their ViewModels.
  - Subscribes to `NavigationEvent`, `GameOverEvent`, and `DimOverlayEvent`.
  - Handles screen changes after those events are published.
  - Owns `IsDimOverlayVisible` for the exit popup overlay.

- `WPFTowerDefense/ViewModels/StartScreenViewModel.cs`
  - Replaces start screen click handlers.
  - Has `PlayCommand`, `OptionsCommand`, `ExitCommand`, and `GameGuideCommand`.
  - Publishes `NavigationEvent` and `DimOverlayEvent`.

- `WPFTowerDefense/ViewModels/SelectLevelScreenViewModel.cs`
  - Replaces level selection click handlers and level index logic.
  - Has `ReturnCommand`, `PlayCommand`, `NextLevelCommand`, and `PreviousLevelCommand`.
  - Owns `LevelPreviewImage`.
  - Publishes `NavigationEvent`.

- `WPFTowerDefense/ViewModels/OptionsScreenViewModel.cs`
  - Replaces slider and return click logic.
  - Owns `VolumeLevel` and `VolumeText`.
  - Saves volume to `Properties.Settings.Default.VolumeLevel`.
  - Publishes `NavigationEvent`.

- `WPFTowerDefense/ViewModels/GameGuideScreenViewModel.cs`
  - Replaces guide return click handler.
  - Publishes `NavigationEvent`.

- `WPFTowerDefense/ViewModels/GameOverScreenViewModel.cs`
  - Owns the game over texts.
  - Loads the highscore text.
  - Has `ContinueCommand`.
  - Publishes `NavigationEvent`.

- `WPFTowerDefense/ViewModels/PlayLevelScreenViewModel.cs`
  - Owns most play-screen game flow state.
  - Loads enemies, effects, towers, and level data.
  - Owns wave, gold, HP, start/return button visibility, and insufficient-gold visibility.
  - Keeps the WPF canvas-based game behavior working.
  - Publishes `GameOverEvent`.

- `WPFTowerDefense/ViewModels/ExitConfirmationWindowViewModel.cs`
  - Replaces exit confirmation button click handlers.
  - Has `YesCommand` and `NoCommand`.

- `WPFTowerDefense/Events/NavigationEvent.cs`
  - Contains `NavigationEvent` and `NavigationRequest`.
  - Used when a ViewModel wants to change screens.

- `WPFTowerDefense/Events/GameOverEvent.cs`
  - Contains `GameOverEvent` and `GameOverData`.
  - Used when the play screen ends and the game over screen should open.

- `WPFTowerDefense/Events/DimOverlayEvent.cs`
  - Used to show/hide the dark overlay behind the exit confirmation window.

## EventAggregator Usage

The corrected MVVM communication now works like the `TowerDefense2` reference project: one ViewModel publishes an event, and another ViewModel subscribes to it.

`MainViewModel` is the subscriber for app-level navigation and overlay state:

```csharp
EventAggregator.GetEvent<NavigationEvent>()
    .Subscribe(OnNavigationRequested, ThreadOption.UIThread);

EventAggregator.GetEvent<GameOverEvent>()
    .Subscribe(OnGameOver, ThreadOption.UIThread);

EventAggregator.GetEvent<DimOverlayEvent>()
    .Subscribe(OnDimOverlayChanged, ThreadOption.UIThread);
```

Screen ViewModels publish events instead of calling `MainViewModel` directly:

```csharp
EventAggregator.GetEvent<NavigationEvent>()
    .Publish(new NavigationRequest("SelectLevel"));

EventAggregator.GetEvent<GameOverEvent>()
    .Publish(new GameOverData(lastWave, _currentLevel.LevelName));

EventAggregator.GetEvent<DimOverlayEvent>()
    .Publish(true);
```

Current event responsibilities:

- `NavigationEvent`
  - Published by `StartScreenViewModel`, `SelectLevelScreenViewModel`, `OptionsScreenViewModel`, `GameGuideScreenViewModel`, and `GameOverScreenViewModel`.
  - Subscribed by `MainViewModel`.
  - Used for screen changes.

- `GameOverEvent`
  - Published by `PlayLevelScreenViewModel`.
  - Subscribed by `MainViewModel`.
  - Used when the game ends and the app should show the game over screen.

- `DimOverlayEvent`
  - Published by `StartScreenViewModel`.
  - Subscribed by `MainViewModel`.
  - Used to show or hide the dark background behind the exit confirmation window.

Why this is better MVVM here:

- Child ViewModels no longer store a `MainViewModel` reference.
- Navigation is not done by code-behind.
- App-level state changes go through Prism events, the same pattern as `AddStudentEvent` and `SelectStudentEvent` in the reference project.

## Moved Files

These screen files were moved from the project root into `WPFTowerDefense/Views`:

- `StartScreen.xaml`
- `StartScreen.xaml.cs`
- `SelectLevelScreen.xaml`
- `SelectLevelScreen.xaml.cs`
- `OptionsScreen.xaml`
- `OptionsScreen.xaml.cs`
- `GameGuideScreen.xaml`
- `GameGuideScreen.xaml.cs`
- `GameOverScreen.xaml`
- `GameOverScreen.xaml.cs`
- `PlayLevelScreen.xaml`
- `PlayLevelScreen.xaml.cs`

Their namespaces changed from `WPFTowerDefense` to `WPFTowerDefense.Views`.

## Removed Files

- `WPFTowerDefense/UIElements.cs`
  - It only created the old start button in code.
  - The play-screen buttons now live in XAML and bind to ViewModel commands.

## Changed Files

- `WPFTowerDefense/App.xaml`
  - Removed `StartupUri`.
  - Startup is now controlled in `App.xaml.cs`, like the reference project.

- `WPFTowerDefense/App.xaml.cs`
  - Creates `EventAggregator`.
  - Creates `MainWindow`.
  - Creates `MainViewModel`.
  - Sets `mainWindow.DataContext = mainViewModel`.

- `WPFTowerDefense/MainWindow.xaml`
  - Removed the `Frame`.
  - Added `ContentControl Content="{Binding CurrentView}"`.
  - Bound the dim overlay to `IsDimOverlayVisible`.

- `WPFTowerDefense/MainWindow.xaml.cs`
  - Removed navigation methods and startup navigation.
  - Now only calls `InitializeComponent()`.

- `WPFTowerDefense/ExitConfirmationWindow.xaml`
  - Replaced `Click` handlers with command bindings.

- `WPFTowerDefense/ExitConfirmationWindow.xaml.cs`
  - Removed button handler methods.
  - Sets `ExitConfirmationWindowViewModel` as `DataContext`.

- `WPFTowerDefense/WPFTowerDefense.csproj`
  - Added `Prism.Events` package reference because the reference project uses it.

## View Changes

- `StartScreen`
  - Is now a `UserControl`.
  - Buttons use command bindings.

- `SelectLevelScreen`
  - Is now a `UserControl`.
  - Level preview image uses `{Binding LevelPreviewImage}`.
  - Return, play, previous, and next use command bindings.

- `OptionsScreen`
  - Is now a `UserControl`.
  - Slider uses two-way binding to `VolumeLevel`.
  - Volume text binds to `VolumeText`.

- `GameGuideScreen`
  - Is now a `UserControl`.
  - Return uses `ReturnCommand`.

- `GameOverScreen`
  - Is now a `UserControl`.
  - Wave/highscore texts are bound properties.
  - Continue uses `ContinueCommand`.

- `PlayLevelScreen`
  - Is now a `UserControl`.
  - Start/return menu buttons and HP/wave/gold texts are declared in XAML.
  - The code-behind only forwards WPF-specific events to the ViewModel:
    - `Loaded`
    - `MouseDown`
    - `MouseMove`
    - `DragOver`
    - `Drop`

## App Flow Now

1. App starts in `App.xaml.cs`.
2. `App` creates one `EventAggregator`.
3. `App` creates `MainWindow`.
4. `App` creates `MainViewModel` and sets it as the `MainWindow.DataContext`.
5. `MainViewModel` creates `StartScreen` and `StartScreenViewModel`.
6. `MainWindow.xaml` shows `CurrentView` inside the `ContentControl`.
7. Buttons call ViewModel commands instead of code-behind click handlers.
8. Screen ViewModels publish events with `EventAggregator.GetEvent<...>().Publish(...)`.
9. `MainViewModel` receives them through `GetEvent<...>().Subscribe(...)`.
10. `MainViewModel` swaps `CurrentView`.
11. When the play screen opens, `PlayLevelScreenViewModel` loads database data, level JSON, and the `GameManager`.
12. The play view still passes canvas drag/drop events to the ViewModel because WPF canvas objects and drag/drop are UI-specific.
13. When the player loses or returns to menu, the play ViewModel saves/loads highscore data and publishes `GameOverEvent`.
14. `MainViewModel` receives `GameOverEvent` and opens `GameOverScreen`.
15. `GameOverScreenViewModel` shows the reached wave and highscore.
16. Continue publishes a navigation event back to the start screen.

## Build Check

`dotnet build WPFTowerDefense/WPFTowerDefense.csproj` succeeds.

There are still nullable warnings, mostly from existing model/game classes and a few helper signatures copied in the same style as the reference project. No build errors remain.
