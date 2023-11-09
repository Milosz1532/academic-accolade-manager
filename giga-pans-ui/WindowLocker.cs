using System;
using System.Windows;


namespace giga_pans_ui
{
    public class WindowLocker
    {
        private readonly Window _mainWindow;

        public WindowLocker(Window mainWindow)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        public void ShowModal(Window childWindow)
        {
            if (childWindow == null)
                throw new ArgumentNullException(nameof(childWindow));

            _mainWindow.IsEnabled = false;

            childWindow.Closed += (s, args) =>
            {
                _mainWindow.IsEnabled = true;
            };

            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            childWindow.ShowDialog();
        }
    }
}
