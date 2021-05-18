namespace WindowsInput
{
    internal class InputSimulator : IInputSimulator
    {
        public InputSimulator(IKeyboardSimulator keyboardSimulator, IMouseSimulator mouseSimulator, IInputDeviceStateAdaptor inputDeviceStateAdaptor)
        {
            Keyboard = keyboardSimulator;
            _mouseSimulator = mouseSimulator;
            InputDeviceState = inputDeviceStateAdaptor;
        }

        public InputSimulator()
        {
            Keyboard = new KeyboardSimulator(this);
            _mouseSimulator = new MouseSimulator(this);
            InputDeviceState = new WindowsInputDeviceStateAdaptor();
        }

        public IKeyboardSimulator Keyboard { get; }

        public IMouseSimulator Mouse => _mouseSimulator;

        public IInputDeviceStateAdaptor InputDeviceState { get; }

        private readonly IMouseSimulator _mouseSimulator;
    }
}
