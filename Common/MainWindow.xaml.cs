using System;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace isolate_reactive_object_bug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private class TestReactiveObject : ReactiveUI.ReactiveObject
        {
            private string _testProperty;
            public string TestProperty
            {
                get => _testProperty;
                set => this.RaiseAndSetIfChanged(ref _testProperty, value);
            }
        }

        private TestReactiveObject vm = new TestReactiveObject();
        private Button changeButton;
        private int changeCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            changeButton = new Button();
            changeButton.Content = "Change";

            changeButton.Click += UpdateTestProperty;
            IncrementChangeCountOnButtonClick();

            this.Content = changeButton;

            // It works if we don't capture any values
            vm.PropertyChanged += (s, e) =>
            {
                MessageBox.Show("It works if we don't capture any values.");
            };

            // ...but not if the function *captures* anything.
            int someCapturedValue = 0;
            vm.PropertyChanged += (s, e) =>
            {
                MessageBox.Show("It doesn't work if we capture something." + someCapturedValue);
            };
        }

        private void UpdateTestProperty(object sender, EventArgs e)
        {
            vm.TestProperty = "" + changeCount;
        }

        private void IncrementChangeCountOnButtonClick()
        {
            changeButton.Click += (s, e) => changeCount++;

            // Bizarrely, closures work from *this* function...
            int someCapturedValue = 0;
            vm.PropertyChanged += (s, e) =>
            {
                MessageBox.Show("It works if you capture something in the same function that you subscribe to changeButton.Click." + someCapturedValue);
            };
        }

        // Event handlers
    }
}