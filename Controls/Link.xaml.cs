using OneQuick.SysX;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace OneQuick.Controls
{
    public partial class Link : UserControl
    {
        public bool LinkUnderline
        {
            get => (bool)GetValue(LinkUnderlineProperty);
            set => SetValue(LinkUnderlineProperty, value);
        }

        public string UriSeperator
        {
            get => (string)GetValue(UriSeperatorProperty);
            set => SetValue(UriSeperatorProperty, value);
        }

        public int UriMargin
        {
            get => (int)GetValue(UriMarginProperty);
            set => SetValue(UriMarginProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static void TextChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Link link = d as Link;
            link.ClearTexts();
            string text = e.NewValue as string;
            string[] array = text.Split(new string[]
            {
                link.UriSeperator
            }, StringSplitOptions.None);
            link.PreText = array[0];
            if (array.Length > 1)
            {
                link.UriText = array[1];
            }
            int startIndex = link.PreText.Length + link.UriText.Length + link.UriSeperator.Length * 2;
            if (array.Length > 2)
            {
                link.PostText = text.Substring(startIndex);
            }
        }

        private void ClearTexts()
        {
            PreText = UriText = PostText = "";
        }

        public string PreText
        {
            get => (string)GetValue(PreTextProperty);
            set => SetValue(PreTextProperty, value);
        }

        public string UriText
        {
            get => (string)GetValue(UriTextProperty);
            set => SetValue(UriTextProperty, value);
        }

        public Brush UriForeground
        {
            get => (Brush)GetValue(UriForegroundProperty);
            set => SetValue(UriForegroundProperty, value);
        }

        public string PostText
        {
            get => (string)GetValue(PostTextProperty);
            set => SetValue(PostTextProperty, value);
        }

        public static void PostTextChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Link link = d as Link;
            if (string.IsNullOrEmpty(e.NewValue as string))
            {
                link.PostTextVisibility = Visibility.Collapsed;
                return;
            }
            link.PostTextVisibility = Visibility.Visible;
        }

        public Visibility PostTextVisibility
        {
            get => (Visibility)GetValue(PostTextVisibilityProperty);
            set => SetValue(PostTextVisibilityProperty, value);
        }

        public string RunCommand
        {
            get => (string)GetValue(RunCommandProperty);
            set => SetValue(RunCommandProperty, value);
        }

        public string RunArgs
        {
            get => (string)GetValue(RunArgsProperty);
            set => SetValue(RunArgsProperty, value);
        }

        public event RoutedEventHandler UriClick
        {
            add
            {
                AddHandler(UriClickEvent, value);
            }
            remove
            {
                RemoveHandler(UriClickEvent, value);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (!string.IsNullOrEmpty(RunCommand))
            {
                Cmd.Run(RunCommand, RunArgs, true);
            }
            RaiseEvent(new RoutedEventArgs(UriClickEvent));
        }

        public Link()
        {
            InitializeComponent();
        }

        // Note: this type is marked as 'beforefieldinit'.
        static Link()
        {
            UriClickEvent = EventManager.RegisterRoutedEvent("UriClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Link));
        }

        public static RoutedEvent UriClickEvent { get; set; }

        public static readonly DependencyProperty LinkUnderlineProperty = DependencyProperty.Register("LinkUnderline", typeof(bool), typeof(Link), new PropertyMetadata(false));

        public static readonly DependencyProperty UriSeperatorProperty = DependencyProperty.Register("UriSeperator", typeof(string), typeof(Link), new PropertyMetadata("__"));

        public static readonly DependencyProperty UriMarginProperty = DependencyProperty.Register("UriMargin", typeof(int), typeof(Link), new PropertyMetadata(0));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Link), new PropertyMetadata("", new PropertyChangedCallback(TextChangedStatic)));

        public static readonly DependencyProperty PreTextProperty = DependencyProperty.Register("PreText", typeof(string), typeof(Link), new PropertyMetadata(""));

        public static readonly DependencyProperty UriTextProperty = DependencyProperty.Register("UriText", typeof(string), typeof(Link), new PropertyMetadata(""));

        public static readonly DependencyProperty UriForegroundProperty = DependencyProperty.Register("UriForeground", typeof(Brush), typeof(Link), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 102, 204))));

        public static readonly DependencyProperty PostTextProperty = DependencyProperty.Register("PostText", typeof(string), typeof(Link), new PropertyMetadata("", new PropertyChangedCallback(PostTextChangedStatic)));

        public static readonly DependencyProperty PostTextVisibilityProperty = DependencyProperty.Register("PostTextVisibility", typeof(Visibility), typeof(Link), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty RunCommandProperty = DependencyProperty.Register("RunCommand", typeof(string), typeof(Link), new PropertyMetadata(""));

        public static readonly DependencyProperty RunArgsProperty = DependencyProperty.Register("RunArgs", typeof(string), typeof(Link), new PropertyMetadata(""));
    }
}
