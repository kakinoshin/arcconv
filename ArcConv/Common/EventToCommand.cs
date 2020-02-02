using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ArcConv.Common
{
    public interface IEventArgsConverter
    {
        object Convert(object value, object parameter);
    }

    public class EventToCommand : TriggerAction<DependencyObject>
    {
        //
        // Dependency properties
        //

        #region implementation of CommandParameter property
        // 依存プロパティ
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",                         // プロパティ名
                typeof(object),                             // プロパティの型
                typeof(EventToCommand),                     // コントロールの型
                new FrameworkPropertyMetadata(              // メタデータ
                    null,
                    new PropertyChangedCallback(OnCommandParameterChanged)));

        // 依存プロパティが変更されたときの処理
        private static void OnCommandParameterChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            EventToCommand parent = obj as EventToCommand;

            if (parent != null && e.NewValue != null)
            {
                if (parent.AssociatedObject == null)
                {
                    return;
                }

                parent.EnableDisableElement();
            }
        }

        // 依存プロパティのラッパー
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        #endregion
        
        #region implementation of Command property
        // 依存プロパティ
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",                              // プロパティ名
                typeof(ICommand),                       // プロパティの型
                typeof(EventToCommand),                 // コントロールの型
                new FrameworkPropertyMetadata(          // メタデータ
                    null,
                    new PropertyChangedCallback(OnCommandChanged)));

        // 依存プロパティが変更されたときの処理
        private static void OnCommandChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            OnCommandChanged(obj as EventToCommand, e);
        }

        // 依存プロパティのラッパー
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion
        
        #region implementation of MustToggleIsEnabled property
        // 依存プロパティ
        public static readonly DependencyProperty MustToggleIsEnabledProperty =
            DependencyProperty.Register(
                "MustToggleIsEnabled",              // プロパティ名
                typeof(bool),                       // プロパティの型
                typeof(EventToCommand),             // コントロールの型
                new FrameworkPropertyMetadata(      // メタデータ
                    false,
                    new PropertyChangedCallback(OnMustToggleIsEnabledChanged)));

        // 依存プロパティが変更されたときの処理
        private static void OnMustToggleIsEnabledChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            EventToCommand parent = obj as EventToCommand;

            if (parent != null && e.NewValue != null)
            {
                if (parent.AssociatedObject == null)
                {
                    return;
                }

                parent.EnableDisableElement();
            }
        }

        // 依存プロパティのラッパー
        public bool MustToggleIsEnabled
        {
            get { return (bool)GetValue(MustToggleIsEnabledProperty); }
            set { SetValue(MustToggleIsEnabledProperty, value); }
        }
        #endregion

        #region implementation of EventArgsConverterParameter property
        // 依存プロパティ
        public static readonly DependencyProperty EventArgsConverterParameterProperty =
            DependencyProperty.Register(
                "EventArgsConverterParameter",          // プロパティ名
                typeof(object),                         // プロパティの型
                typeof(EventToCommand),                 // コントロールの型
                new PropertyMetadata(null));

        // 依存プロパティのラッパー
        public object EventArgsConverterParameter
        {
            get { return (object)GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }
        #endregion
        

        private object _commandParameterValue;
        public object CommandParameterValue
        {
            get
            {
                return _commandParameterValue ?? CommandParameter;
            }

            set
            {
                _commandParameterValue = value;
                EnableDisableElement();
            }
        }

        private bool? _mustToggleValue;
        public bool MustToggleIsEnabledValue
        {
            get
            {
                return _mustToggleValue == null
                           ? MustToggleIsEnabled
                           : _mustToggleValue.Value;
            }

            set
            {
                _mustToggleValue = value;
                EnableDisableElement();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }

        private FrameworkElement GetAssociatedObject()
        {
            return AssociatedObject as FrameworkElement;
        }

        private ICommand GetCommand()
        {
            return Command;
        }

        public bool PassEventArgsToCommand { get; set; }
        public IEventArgsConverter EventArgsConverter { get; set; }

        public void Invoke()
        {
            Invoke(null);
        }

        protected override void Invoke(object parameter)
        {
            if (AssociatedElementIsDisabled())
            {
                return;
            }

            var command = GetCommand();
            var commandParameter = CommandParameterValue;

            if (commandParameter == null
                && PassEventArgsToCommand)
            {
                commandParameter = EventArgsConverter == null
                    ? parameter
                    : EventArgsConverter.Convert(parameter, EventArgsConverterParameter);
            }

            if (command != null
                && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        private static void OnCommandChanged(
            EventToCommand element,
            DependencyPropertyChangedEventArgs e)
        {
            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)e.NewValue;

            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }

            element.EnableDisableElement();
        }

        private bool AssociatedElementIsDisabled()
        {
            var element = GetAssociatedObject();

            return AssociatedObject == null
                || (element != null
                   && !element.IsEnabled);
        }

        private void EnableDisableElement()
        {
            var element = GetAssociatedObject();

            if (element == null)
            {
                return;
            }

            var command = GetCommand();

            if (MustToggleIsEnabledValue
                && command != null)
            {
                element.IsEnabled = command.CanExecute(CommandParameterValue);
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
    }
}
